namespace BackendAPI.DTO.Notification
{
    public class CreateNotificationDto
    {
        public string Title { get; set; }
        public string Description { get; set; }

        // All | Single
        public string TargetType { get; set; }
        public string? TargetUserId { get; set; }

        // Null = send now
        public DateTime? ScheduledAt { get; set; }
    }

}
