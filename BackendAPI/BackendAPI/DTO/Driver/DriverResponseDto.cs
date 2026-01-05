using BackendAPI.Data.Entities;

namespace BackendAPI.DTO.Driver
{
    public class DriverResponseDto
    {
        public string Id { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
