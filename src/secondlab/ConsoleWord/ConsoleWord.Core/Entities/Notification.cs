namespace ConsoleWord.Core.Entities;

public class Notification
{
    public string UserName { get; set; }

    public string Message { get; set; } = "notification messange";

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}