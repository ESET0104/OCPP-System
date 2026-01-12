namespace BackendAPI.Data.Entities
{
    public class Notification
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        // All / Single
        public string TargetType { get; set; } // "All", "Single"
        public string? TargetUserId { get; set; }

        // Status
        public string Status { get; set; } // Pending, Scheduled, Sent, Failed

        // Scheduling
        public DateTime? ScheduledAt { get; set; }

        // Audit
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? SentAt { get; set; }
    }

}
