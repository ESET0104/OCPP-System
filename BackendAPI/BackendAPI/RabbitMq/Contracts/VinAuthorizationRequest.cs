namespace BackendAPI.RabbitMq.Contracts
{
    public class VinAuthorizationRequest
    {
        public string MessageId { get; set; }
        public string ChargerId { get; set; }
        public string Vin { get; set; }
    }
}
