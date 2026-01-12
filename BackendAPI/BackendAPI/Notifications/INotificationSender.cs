using BackendAPI.Data.Entities;

namespace BackendAPI.Notifications
{
    public interface INotificationSender
    {
        Task SendAsync(BackendAPI.Data.Entities.Notification notification);
    }
}

