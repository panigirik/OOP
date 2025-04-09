using ConsoleWord.Application.DocumentUseCases;
using ConsoleWord.Application.Services;
using ConsoleWord.Core.Entities;
using ConsoleWord.Infrastracture.CloudStorage.Services;
using ConsoleWord.Infrastracture.LocalStorage.Interfaces;
using Spectre.Console;

namespace ConsoleWord
{
    public class Menu
    {
        private readonly DocumentService _documentService;
        private readonly IStorageProvider _localStorage;
        private readonly CloudFileStorage _cloudStorage;
        private readonly AuthenticationService _authenticationService;
        private readonly NotificationService _notificationService;
        private readonly DocumentStorageService _documentStorageService;
        private readonly DocumentEditor _documentEditor;
        private readonly UndoRedoService _undoRedoService;

        string notificationFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "notifications.txt");
        
        private User _currentUser;
        
        public Menu(DocumentService documentService, 
            IStorageProvider localStorage,
            CloudFileStorage cloudStorage,
            AuthenticationService authenticationService,
            NotificationService notificationService,
            DocumentStorageService documentStorageService,
            DocumentEditor documentEditor,
            UndoRedoService undoRedoService)
        {
            _documentService = documentService;
            _localStorage = localStorage;
            _cloudStorage = cloudStorage;
            _authenticationService = authenticationService;
            _notificationService = notificationService;
            _documentStorageService = documentStorageService;
            _documentEditor = documentEditor;
            _undoRedoService = undoRedoService;
        }

        public void Show()
        {
            ShowAuthenticationOptions();

            while (true)
            {
                AnsiConsole.Clear();
                AnsiConsole.Write(new FigletText("Doc Editor").Centered().Color(Color.Blue));
                AnsiConsole.MarkupLine($"[bold green]Welcome, {_currentUser.Username} ({_currentUser.Role.RoleName})[/]");

                var options = new List<string>();

                
                if (_currentUser.Role.HasPermission("Edit"))
                    options.Add("Create new document");
                    options.Add("Delete Document");
                    
               
                if (_currentUser.Role.HasPermission("Read"))
                    options.Add("Open document");
                    options.Add("Undo");
                    options.Add("Redo");
                
                if (_currentUser.Role.HasPermission("Edit"))
                    options.Add("Edit document");

                
                options.Add("Logout");
                options.Add("Subscribe to user");
                options.Add("Exit");
                
                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[yellow]Choose an option:[/]")
                        .PageSize(5)
                        .AddChoices(options));

                switch (choice)
                {
                    case "Create new document":
                        _documentService.CreateAndSaveDocument();
                        NotifySubscribers(_currentUser.Username, $"{_currentUser.Username} created a new document.");
                        break;

                    case "Open document":
                        _documentService.OpenAndEditDocument(_currentUser);
                        NotifySubscribers(_currentUser.Username, $"{_currentUser.Username} view document.");
                        break;

                    case "Edit document":
                        _documentService.OpenAndEditDocument(_currentUser);
                        NotifySubscribers(_currentUser.Username, $"{_currentUser.Username} edit document.");
                        break;

                    case "Delete Document":
                        _documentService.DeleteDocumentByPath();
                        NotifySubscribers(_currentUser.Username, $"{_currentUser.Username} edit document.");
                        break;
                    
                    case "Undo":
                        AnsiConsole.MarkupLine("[yellow]Last action undone.[/]");
                        _undoRedoService.Undo();
                        break;
                    
                    case "Redo":
                        AnsiConsole.MarkupLine("[yellow]Last undone action redone.[/]");
                        _undoRedoService.Redo();
                        break;
                    
                    case "Save document to cloud":
                        SaveDocumentToCloud();
                        NotifySubscribers(_currentUser.Username, $"{_currentUser.Username} saved a document to the cloud.");
                        break;


                    
                    case "Logout":
                        Logout();
                        break;
                    
                    case "Subscribe to user":
                        SubscribeToUser();
                        break;


                    case "Exit":
                        return;
                }

                AnsiConsole.MarkupLine("\n[gray]Press any key to return to menu...[/]");
                Console.ReadKey(true);
            }
        }

        private void ShowAuthenticationOptions()
        {
            while (true)
            {
                AnsiConsole.Clear();
                AnsiConsole.Write(new FigletText("Welcome").Centered().Color(Color.Orange1));

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[yellow]Authentication Required[/]")
                        .AddChoices("Login", "Register"));

                if (choice == "Login")
                {
                    AuthenticateUser();
                    break;
                }
                else if (choice == "Register")
                {
                    RegisterUser();
                    break;
                }
            }
        }

        private void SaveDocumentToCloud()
        {
            // Получаем путь к файлу
            var filePath = AnsiConsole.Ask<string>("Enter the [green]path[/] to the .docx file you want to upload:");

            if (!File.Exists(filePath))
            {
                AnsiConsole.MarkupLine("[red]File not found.[/]");
                return;
            }

            // Получаем формат файла (например, "docx")
            var format = AnsiConsole.Ask<string>("Enter the [green]format[/] for the document (e.g., 'docx'):");

            // Здесь предполагаем, что ты можешь создать объект Document из файла
            Document document = _documentEditor.LoadDocument(filePath); // или другая логика загрузки документа

            _documentStorageService.SaveDocumentToCloud(document, format); // передаем документ и формат

            AnsiConsole.MarkupLine("[green]Document saved to cloud successfully.[/]");
        }

        
        private void AuthenticateUser()
        {
            AnsiConsole.MarkupLine("[bold]Please log in to continue.[/]");

            while (true)
            {
                string username = AnsiConsole.Ask<string>("Enter [green]Username[/]:");
                string password = AnsiConsole.Prompt(
                    new TextPrompt<string>("Enter [green]Password[/]:")
                        .PromptStyle("red")
                        .Secret());

                _currentUser = _authenticationService.Authenticate(username, password);

                if (_currentUser != null)
                {
                    AnsiConsole.MarkupLine($"[green]Welcome, {_currentUser.Username}![/] Logged in as [blue]{_currentUser.Role.RoleName}[/].");


                    var notifications = _notificationService.GetUserNotifications(_currentUser.Username);

                    if (notifications.Any())
                    {
                        AnsiConsole.MarkupLine($"\n[bold underline fuchsia]You have {notifications.Count} new notification(s):[/]");

                        foreach (var note in notifications)
                        {
                            AnsiConsole.Write(
                                new Panel($"[white]{note}[/]")
                                    .Border(BoxBorder.Double)
                                    .BorderStyle(new Style(foreground: Color.Fuchsia))
                                    .Padding(1, 0, 1, 0)
                            );
                        }

                        _notificationService.ClearUserNotifications(_currentUser.Username);
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("\n[gray]No new notifications.[/]");
                    }

                    break;
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]Invalid credentials. Try again.[/]");
                }
            }
        }

        private void RegisterUser()
        {
            AnsiConsole.MarkupLine("[bold]Please register to create a new account.[/]");

            string username = AnsiConsole.Ask<string>("Enter [green]Username[/]:");
            string password = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter [green]Password[/]:")
                    .PromptStyle("red")
                    .Secret());

            string role = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Choose your [green]role[/]:")
                    .AddChoices("Admin", "Editor", "Viewer"));

            _authenticationService.Register(username, password, role);

            AnsiConsole.MarkupLine("[green]Registration successful![/]");
            AuthenticateUser();
        }

        private void Logout()
        {
            _currentUser = null;
            AnsiConsole.MarkupLine("[gray]You have been logged out.[/]");
            ShowAuthenticationOptions();
        }
        
        private void SubscribeToUser()
        {
            var allUsers = File.ReadAllLines("users.txt")
                .Select(line => line.Split(":")[0])
                .Where(username => username != _currentUser.Username)
                .ToList();

            if (!allUsers.Any())
            {
                AnsiConsole.MarkupLine("[red]No other users to subscribe to.[/]");
                return;
            }

            var selectedUser = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Choose a user to [green]subscribe[/] to:")
                    .AddChoices(allUsers));

            var line = $"{_currentUser.Username}:{selectedUser}";
            var subscriptionFile = "subscriptions.txt";

            if (!File.Exists(subscriptionFile))
                File.Create(subscriptionFile).Dispose();

            var existingSubscriptions = File.ReadAllLines(subscriptionFile).ToList();

            if (!existingSubscriptions.Contains(line))
            {
                existingSubscriptions.Add(line);
                File.WriteAllLines(subscriptionFile, existingSubscriptions);
                AnsiConsole.MarkupLine($"[green]Subscribed to {selectedUser}![/]");
            }
            else
            {
                AnsiConsole.MarkupLine($"[yellow]You are already subscribed to {selectedUser}.[/]");
            }
        }
        
        private void NotifySubscribers(string actorUsername, string message)
        {
            var subscriptionsFile = "subscriptions.txt";

            if (!File.Exists(subscriptionsFile)) return;

            var lines = File.ReadAllLines(subscriptionsFile);
            var subscribers = lines
                .Where(line => line.Split(":")[1] == actorUsername)
                .Select(line => line.Split(":")[0]);

            foreach (var subscriber in subscribers)
            {
                _notificationService.SendNotification(subscriber, message);
            }
        }
        
    }
}
