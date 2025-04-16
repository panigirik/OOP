using ConsoleWord.Application.Services;
using ConsoleWord.Core.Entities;
using ConsoleWord.Core.Roles;
using Spectre.Console;

namespace ConsoleWord.Application.Helpers;

public class ShowAuthenticationOptionsHepler
{
    private readonly NotificationService _notificationService;
    public User _currentUser;
    private readonly AuthenticationService _authenticationService;
    
    public ShowAuthenticationOptionsHepler(NotificationService notificationService,
        AuthenticationService authenticationService)
    {
        _notificationService = notificationService;
        _authenticationService = authenticationService;
    }
    
    public User ShowAuthenticationOptions()
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
                return _currentUser;
            }
            else if (choice == "Register")
            {
                RegisterUser();
                return _currentUser;
            }
        }
    }

    
private void AuthenticateUser()
{
    AnsiConsole.MarkupLine("[bold]Please log in to continue.[/]");

    int maxAttempts = 3;  // Define a maximum number of attempts
    int attempts = 0;

    while (attempts < maxAttempts)
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
            attempts++;
            AnsiConsole.MarkupLine("[red]Invalid credentials. Please try again.[/]");

            if (attempts == maxAttempts)
            {
                AnsiConsole.MarkupLine("[red]Maximum login attempts reached. Exiting authentication.[/]");
                break;  // Exit the loop after max attempts
            }
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
    
    
    
    public void DeleteUserByUsername()
    {
        string filePath = "users.txt"; // путь к файлу с пользователями
        if (!File.Exists(filePath))
        {
            AnsiConsole.MarkupLine("[red]User file not found.[/]");
            return;
        }

        string usernameToDelete = AnsiConsole.Ask<string>("Enter the [red]username[/] of the user to delete:");

        var lines = File.ReadAllLines(filePath).ToList();
        int originalCount = lines.Count;

        // Удаляем строки, в которых имя пользователя совпадает с введённым
        lines = lines.Where(line =>
        {
            var parts = line.Split(':');
            return parts.Length != 3 || !parts[0].Equals(usernameToDelete, StringComparison.OrdinalIgnoreCase);
        }).ToList();

        if (lines.Count == originalCount)
        {
            AnsiConsole.MarkupLine($"[yellow]No user found with username '{usernameToDelete}'.[/]");
        }
        else
        {
            File.WriteAllLines(filePath, lines);
            AnsiConsole.MarkupLine($"[green]User '{usernameToDelete}' has been deleted successfully.[/]");
        }
    }

            


    
}