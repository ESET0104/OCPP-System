namespace BackendAPI.DTO
{
    public class CreateReservationDto
    {
        public string ChargerId { get; set; }
        public string DriverId { get; set; }
        public int ConnectorId { get; set; }
        public string CreatedBy { get; set; }
    }

}
