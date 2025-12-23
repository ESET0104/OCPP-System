namespace BackendAPI.DTO
{
    public class StopChargingCommand
    {
        public string ChargerId { get; set; }
        public string SessionId { get; set; }
        public string UserId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
