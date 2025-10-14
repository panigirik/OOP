namespace ConsoleWord.Application.Helpers;

public class NotifySubscribersHelper
{
    private readonly NotificationService _notificationService;

    public NotifySubscribersHelper(NotificationService notificationService)
    {
        _notificationService = notificationService;
    }
    
    public void NotifySubscribers(string actorUsername, string message)
    {
        var subscriptionsFile = "subscriptions.txt";

        if (!File.Exists(subscriptionsFile)) return;

        var lines = File.ReadAllLines(subscriptionsFile);
        var subscribers = lines
            .Where(line => line.Split(":")[1] == actorUsername)
            .Select(line => line.Split(":")[0]);

        foreach (var subscriber in subscribers)
        {
            _notificationService.Send(subscriber, message);
        }
    }

}