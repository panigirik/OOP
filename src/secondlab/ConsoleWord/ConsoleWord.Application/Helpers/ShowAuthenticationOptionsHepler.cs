using ConsoleWord.Application.Services;
using ConsoleWord.Core.Entities;
using Spectre.Console;

namespace ConsoleWord.Application.Helpers;

public class ShowAuthenticationOptionsHepler
{
    private readonly NotificationService _notificationService;
    private User _currentUser;
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
    
}