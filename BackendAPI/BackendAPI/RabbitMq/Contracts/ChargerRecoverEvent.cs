namespace BackendAPI.RabbitMq.Contracts
{
    public class ChargerRecoverEvent
    {
        public string ChargerId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
