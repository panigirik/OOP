using ConsoleWord.Application.Helpers;
using ConsoleWord.Application.Services;
using ConsoleWord.Core.Entities;
using Spectre.Console;

namespace ConsoleWord
{
    public class Menu
    {
        private readonly DocumentService _documentService;
        private readonly SaveDocumentToCloudHelper _documentToCloudHelper;
        private readonly UndoRedoService _undoRedoService;
        private readonly NotifySubscribersHelper _notifySubscribersHelper;
        public User _currentUser;
        private readonly ShowAuthenticationOptionsHepler _showAuthentication;
        private readonly PermissionManager _permissionManager;
        private readonly EditorSettings _editorSettings;
        
        public Menu(DocumentService documentService, 
            UndoRedoService undoRedoService,
            NotifySubscribersHelper notifySubscribersHelper,
            ShowAuthenticationOptionsHepler showAuthentication,
            PermissionManager permissionManager,
            SaveDocumentToCloudHelper documentToCloudHelper,
            EditorSettings editorSettings)
        {
            _documentService = documentService;
            _undoRedoService = undoRedoService;
            _notifySubscribersHelper = notifySubscribersHelper;
            _showAuthentication = showAuthentication;
            _permissionManager = permissionManager;
            _documentToCloudHelper = documentToCloudHelper;
            _editorSettings = editorSettings;
        }

        public void Show()
        {
            _currentUser = _showAuthentication.ShowAuthenticationOptions();

            if (_currentUser == null)
            {
                AnsiConsole.MarkupLine("[bold red]Error: Authentication failed. User is null.[/]");
                return;
            }

            if (_currentUser.Role == null)
            {
                AnsiConsole.MarkupLine($"[bold yellow]Warning: User '{_currentUser.Username}' has no role assigned.[/]");
                return;
            } 

            while (true)
            {
                AnsiConsole.Clear();
                AnsiConsole.Write(new FigletText("Doc Editor").Centered().Color(Color.Blue));
                AnsiConsole.MarkupLine($"[bold green]Welcome, {_currentUser.Username} ({_currentUser.Role.RoleName})[/]");

                var options = new List<string>();

                
                if (_currentUser.Role.RoleName == "Admin" || _currentUser.Role.HasPermission("Create"))
                {
                    options.Add("Create new document");
                    options.Add("deleteUserByUsername");
                    options.Add("ManagePermissions");
                }

                    
                if (_currentUser.Role.HasPermission("Delete"))
                    options.Add("Delete Document");

                if (_currentUser.Role.HasPermission("Read"))
                {
                    options.Add("Open document");
                    options.Add("searchText");
                }

                if (_currentUser.Role.HasPermission("Edit"))
                {
                    options.Add("Edit document");
                    options.Add("searchText");
                    options.Add("Undo");
                    options.Add("Redo");
                }


                options.Add("Editor Settings");
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
                        _notifySubscribersHelper.NotifySubscribers(_currentUser.Username, $"{_currentUser.Username} created a new document.");
                        break;

                    case "Open document":
                        _documentService.OpenAndEditDocument(_currentUser);
                        _notifySubscribersHelper.NotifySubscribers(_currentUser.Username, $"{_currentUser.Username} view document.");
                        break;
                    
                    case "searchText":
                        _documentService.SearchTextInDocument(_currentUser);
                        break;

                    case "deleteUserByUsername":
                        _showAuthentication.DeleteUserByUsername();
                        _notifySubscribersHelper.NotifySubscribers(_currentUser.Username, $"{_currentUser.Username} delete some users.");
                        break;
                    
                    case "ManagePermissions":
                        _permissionManager.ManagePermissions();
                        break;
                    
                    case "Edit document":
                        _documentService.OpenAndEditDocument(_currentUser);
                        _notifySubscribersHelper.NotifySubscribers(_currentUser.Username, $"{_currentUser.Username} edit document.");
                        break;

                    case "Delete Document":
                        _documentService.DeleteDocumentByPath();
                        _notifySubscribersHelper.NotifySubscribers(_currentUser.Username, $"{_currentUser.Username} delete document.");
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
                        _documentToCloudHelper.SaveDocumentToCloud();
                        _notifySubscribersHelper.NotifySubscribers(_currentUser.Username, $"{_currentUser.Username} saved a document to the cloud.");
                        break;

                    case "Editor Settings":
                        _editorSettings.ConfigureEditorSettings();
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





        private void Logout()
        {
            _currentUser = _showAuthentication.ShowAuthenticationOptions();
            AnsiConsole.MarkupLine("[gray]You have been logged out.[/]");
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
        

    }
}
