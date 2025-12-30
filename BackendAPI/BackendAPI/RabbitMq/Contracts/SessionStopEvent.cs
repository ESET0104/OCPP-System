namespace BackendAPI.RabbitMq.Contracts
{
    public class SessionStopEvent
    {
        public string SessionId { get; set; }
        public string ChargerId { get; set; }
        public DateTime StopTime { get; set; }
        public decimal EnergyConsumedKwh { get; set; }
        public string triggerReason { get; set; }
    }
}
