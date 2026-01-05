namespace BackendAPI.Data.Entities
{
    public class LogEntry
    {
        public Guid Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Source { get; set; } = null!;  // ocpp | charger
        public string EventType { get; set; } = null!;
        public string Message { get; set; } = null!;
        public string? ChargerId { get; set; }
        public string? SessionId { get; set; }
        public string? DriverId { get; set; }

    }
}
