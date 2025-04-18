using Hangfire;
using Spectre.Console;

public class NotificationService
{
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly string _notificationFile;

    public NotificationService(IBackgroundJobClient backgroundJobClient, string notificationFile)
    {
        _backgroundJobClient = backgroundJobClient;
        _notificationFile = notificationFile;
    }
    

    public void Send(string username, string message)
    {
        Console.WriteLine($"[DEBUG] Send executed: {username}:{message}");

        AnsiConsole.MarkupLine($"\n[bold fuchsia] Notification for [underline]{username}[/]:[/]");

        var line = $"{username}:{message}";
        
        lock (_notificationFile)
        {
            try
            {
                if (!File.Exists(_notificationFile))
                {
                    File.Create(_notificationFile).Dispose();
                }
                
                File.AppendAllLines(_notificationFile, new[] { line });
                Console.WriteLine("[DEBUG] Write successful.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to write to file: {ex.Message}");
            }
        }
    }
    
    public List<string> GetUserNotifications(string username)
    {
        if (!File.Exists(_notificationFile))
            return new List<string>();
        
        var notifications = File.ReadAllLines(_notificationFile)
            .Where(line => line.StartsWith(username + ":"))
            .Select(line => line.Substring(username.Length + 1)) 
            .ToList();

        return notifications;
    }
    
    public void ClearUserNotifications(string username)
    {
        if (!File.Exists(_notificationFile))
            return;

        var allLines = File.ReadAllLines(_notificationFile);
        var remaining = allLines.Where(line => !line.StartsWith(username + ":")).ToList();

        File.WriteAllLines(_notificationFile, remaining);
    }
}
