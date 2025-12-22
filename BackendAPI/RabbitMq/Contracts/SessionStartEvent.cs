namespace BackendAPI.RabbitMq.Contracts
{
    public class SessionStartEvent
    {
        public string SessionId { get; set; }
        public string ChargerId { get; set; }
        public string UserId { get; set; }
        public DateTime StartTime { get; set; }
    }
}
