using ConsoleWord.Application.Services;
using ConsoleWord.Core.Entities;
using ConsoleWord.Infrastracture.CloudStorage.Interfaces;
using ConsoleWord.Infrastracture.LocalStorage.Interfaces;
using Spectre.Console;

namespace ConsoleWord
{
    public class Menu
    {
        private readonly DocumentService _documentService;
        private readonly IStorageProvider _localStorage;
        private readonly ICloudStorageProvider _cloudStorage;
        private readonly AuthenticationService _authenticationService;

        private User _currentUser;

        public Menu(DocumentService documentService, IStorageProvider localStorage, ICloudStorageProvider cloudStorage, AuthenticationService authenticationService)
        {
            _documentService = documentService;
            _localStorage = localStorage;
            _cloudStorage = cloudStorage;
            _authenticationService = authenticationService;
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

                // Only users with 'Edit' permission see this
                if (_currentUser.Role.HasPermission("Edit"))
                    options.Add("Create new document");

                // 'Read' permission allows viewing or editing depending on role
                if (_currentUser.Role.HasPermission("Read"))
                    options.Add("Open document");

                if (_currentUser.Role.HasPermission("Edit"))
                    options.Add("Edit document");

                options.Add("Logout");
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
                        break;

                    case "Open document":
                        _documentService.OpenAndEditDocument(_currentUser);
                        break;

                    case "Edit document":
                        _documentService.OpenAndEditDocument(_currentUser);
                        break;

                    case "Logout":
                        Logout();
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
    }
}
