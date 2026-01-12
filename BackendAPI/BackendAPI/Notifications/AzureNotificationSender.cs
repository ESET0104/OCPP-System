using BackendAPI.Data.Entities;
using BackendAPI.Notifications;
using Microsoft.Azure.NotificationHubs;

public class AzureNotificationSender : INotificationSender
{
    private readonly NotificationHubClient _hub;

    public AzureNotificationSender(IConfiguration config)
    {
        _hub = NotificationHubClient.CreateClientFromConnectionString(
            config["AzureNotificationHub:ConnectionString"],
            config["AzureNotificationHub:HubName"]
        );
    }

    public async Task SendAsync(BackendAPI.Data.Entities.Notification notification)
    {
        var payload = new
        {
            notification = new
            {
                title = notification.Title,
                body = notification.Description
            }
        };

        var jsonPayload = System.Text.Json.JsonSerializer.Serialize(payload);

        if (notification.TargetType == "All")
        {
            await _hub.SendFcmNativeNotificationAsync(jsonPayload);
        }
        else
        {
            // Target single user via tag
            await _hub.SendFcmNativeNotificationAsync(
                jsonPayload,
                notification.TargetUserId
            );
        }
    }
}
