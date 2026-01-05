namespace BackendAPI.DTO.Vehicle
{
    public class VehicleResponseDto
    {
        public string VehicleId { get; set; } = null!;
        public string DriverId { get; set; } = null!;
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
