namespace BackendAPI.DTO
{
    public class CreateTicketDto
    {
        public string Status { get; set; }
        public string Category { get; set; }
        public string Priority { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? ChargerId { get; set; }
        public string? VehicleId { get; set; }
        public string CreatedByUserId { get; set; }
        public string CreatedByName { get; set; }
        public List<string>? Screenshots { get; set; }
    }
}
