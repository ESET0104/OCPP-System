namespace BackendAPI.RabbitMq.Contracts
{
    public class MeterValueEvent
    {
        public string SessionId { get; set; }
        public string ChargerId { get; set; }
        public DateTime Timestamp { get; set; }
        public decimal EnergyKwh { get; set; }
        public int SOC {  get; set; }
    }
}
