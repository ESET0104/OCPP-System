using BackendAPI.Data.Entities;

namespace BackendAPI.DTO.Driver
{
    public class DriverResponseDto
    {
        public string DriverId { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DriverStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
