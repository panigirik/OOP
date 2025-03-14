namespace ConsoleWord.Application.Notifications;

public class ConsoleNotification : INotificationService
{
    public void Notify(string message)
    {
        Console.WriteLine($"[Notification]: {message}");
    }
}
