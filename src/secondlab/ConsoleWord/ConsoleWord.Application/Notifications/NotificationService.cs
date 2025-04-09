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

    public void SendNotification(string username, string message)
    {
        _backgroundJobClient.Enqueue<NotificationService>(service => 
            service.Send(username, message));
    }

    public void Send(string username, string message)
    {
        Console.WriteLine($"[DEBUG] Send executed: {username}:{message}");

        AnsiConsole.MarkupLine($"\n[bold fuchsia]🔔 Notification for [underline]{username}[/]:[/]");

        var line = $"{username}:{message}";

        // Запись уведомления в файл
        lock (_notificationFile)
        {
            try
            {
                // Создаем файл, если его нет
                if (!File.Exists(_notificationFile))
                {
                    File.Create(_notificationFile).Dispose();
                }

                // Записываем строку в файл
                File.AppendAllLines(_notificationFile, new[] { line });
                Console.WriteLine("[DEBUG] Write successful.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to write to file: {ex.Message}");
            }
        }
    }

    // Метод для получения уведомлений для конкретного пользователя
    public List<string> GetUserNotifications(string username)
    {
        if (!File.Exists(_notificationFile))
            return new List<string>();

        // Читаем все строки из файла и фильтруем по имени пользователя
        var notifications = File.ReadAllLines(_notificationFile)
            .Where(line => line.StartsWith(username + ":"))
            .Select(line => line.Substring(username.Length + 1)) // убираем имя пользователя
            .ToList();

        return notifications;
    }

    // Метод для очистки уведомлений конкретного пользователя
    public void ClearUserNotifications(string username)
    {
        if (!File.Exists(_notificationFile))
            return;

        var allLines = File.ReadAllLines(_notificationFile);
        var remaining = allLines.Where(line => !line.StartsWith(username + ":")).ToList();

        File.WriteAllLines(_notificationFile, remaining);
    }
}
