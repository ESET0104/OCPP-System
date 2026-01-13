namespace BackendAPI.DTO
{
    public class CreateChargerRequest
    {
        public string LocationId { get; set; }
        public string Status { get; set; } = "Available";
    }
}
