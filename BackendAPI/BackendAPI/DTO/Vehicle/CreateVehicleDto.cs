namespace BackendAPI.DTO.Vehicle
{
    public class CreateVehicleDto
    {
        public string VehicleName { get; set; } = null!;
        public string Type { get; set; } = null!; 
        public string Make { get; set; } = null!;
        public string Model { get; set; } = null!;
        public string Variant { get; set; } = null!;
        public string RegistrationNumber { get; set; } = null!;
        public string VIN { get; set; } = null!;
        public int? RangeKm { get; set; }
    }
}
