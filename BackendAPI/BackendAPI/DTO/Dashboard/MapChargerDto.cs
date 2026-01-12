namespace BackendAPI.DTO.Dashboard
{
    public class MapChargerDto
    {
        public string ChargerId { get; set; }
        public string Status { get; set; }
        public string LocationName { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
}
