using BackendAPI.Data;
using BackendAPI.Data.Entities;
using BackendAPI.DTO.Notification;
using BackendAPI.Notifications;
using Microsoft.EntityFrameworkCore;
using NanoidDotNet;

namespace BackendAPI.Services
{
    public class NotificationService
    {
        private readonly AppDbContext _db;
        private readonly INotificationSender _sender;


        public NotificationService(AppDbContext db, INotificationSender sender)
        {
            _db = db;
            _sender = sender;
        }


        public async Task<NotificationResponseDto> CreateAndSendAsync(CreateNotificationDto dto)
        {
            var notification = new Notification
            {
                Id = Nanoid.Generate(size: 10),
                Title = dto.Title,
                Description = dto.Description,
                TargetType = dto.TargetType,
                TargetUserId = dto.TargetUserId,
                Status = dto.ScheduledAt == null ? "Pending" : "Scheduled",
                ScheduledAt = dto.ScheduledAt,
                CreatedBy = "admin", // later from JWT
                CreatedAt = DateTime.UtcNow
            };

            _db.Notifications.Add(notification);
            await _db.SaveChangesAsync();

            if (dto.ScheduledAt == null)
            {
                await SendNotificationAsync(notification);
            }

            return Map(notification);
        }

        public async Task SendNotificationAsync(Notification notification)
        {
            try
            {
                await _sender.SendAsync(notification);

                notification.Status = "Sent";
                notification.SentAt = DateTime.UtcNow;
            }
            catch
            {
                notification.Status = "Failed";
            }

            await _db.SaveChangesAsync();
        }

        public async Task<List<NotificationResponseDto>> GetScheduledAsync()
        {
            var notifications = await _db.Notifications
    .Where(n => n.Status == "Scheduled")
    .OrderBy(n => n.ScheduledAt)
    .ToListAsync();

            return notifications.Select(Map).ToList();
        }

        public async Task<List<NotificationResponseDto>> GetHistoryAsync()
        {
            var notifications = await _db.Notifications
                        .Where(n => n.Status == "Sent")
                        .OrderByDescending(n => n.SentAt)
                        .ToListAsync();

            return notifications.Select(Map).ToList();
        }

        private static NotificationResponseDto Map(Notification n)
        {
            return new NotificationResponseDto
            {
                Id = n.Id,
                Title = n.Title,
                Description = n.Description,
                Status = n.Status,
                CreatedAt = n.CreatedAt,
                SentAt = n.SentAt
            };
        }
    }

}
