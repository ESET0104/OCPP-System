namespace BackendAPI.RabbitMq.Contracts
{
    public class ChargerFaultEvent
    {
        public string ChargerId { get; set; }
        public string FaultCode { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
