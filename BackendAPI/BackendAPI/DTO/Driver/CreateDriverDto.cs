namespace BackendAPI.DTO.Driver
{
    public class CreateDriverDto
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? VehicleId { get; set; }
    }
}
