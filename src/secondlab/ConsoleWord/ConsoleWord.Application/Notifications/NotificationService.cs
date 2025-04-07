using ConsoleWord.Core.Entities;
using Hangfire;

namespace ConsoleWord.Application.Notifications;

public class NotificationService
{
    private readonly IBackgroundJobClient _backgroundJobClient;

    public NotificationService(IBackgroundJobClient backgroundJobClient)
    {
        _backgroundJobClient = backgroundJobClient;
    }

    public void SendNotification(Guid userId, string message)
    {
        _backgroundJobClient.Enqueue(() => SendAsync(userId, message));
    }

    public async Task SendAsync(Guid userId, string message)
    {
        // Логика отправки уведомления
        Console.WriteLine($"User {userId} received notification: {message}");
    }
}
