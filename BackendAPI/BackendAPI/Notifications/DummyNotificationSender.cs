using BackendAPI.Data.Entities;
using BackendAPI.Notifications;

public class DummyNotificationSender : INotificationSender
{
    public Task SendAsync(BackendAPI.Data.Entities.Notification notification)
    {
        // Simulate sending a notification
        Console.WriteLine("==== DUMMY PUSH NOTIFICATION ====");
        Console.WriteLine($"Title       : {notification.Title}");
        Console.WriteLine($"Description : {notification.Description}");
        Console.WriteLine($"TargetType  : {notification.TargetType}");
        Console.WriteLine($"TargetUser  : {notification.TargetUserId}");
        Console.WriteLine("================================");

        return Task.CompletedTask;
    }
}

