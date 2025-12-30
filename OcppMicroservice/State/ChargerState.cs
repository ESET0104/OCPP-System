namespace OcppMicroservice.State
{
    public class ChargerState
    {
        public string ChargePointId { get; set; }
        public bool IsConnected { get; set; }
        public string? ActiveSessionId { get; set; }
        public string? ActiveTransactionId { get; set; }
        public bool IsFaulted { get; set; }
    }

}
