using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendAPI.Data.Entities.Users
{
    public class DriverAuth
    {
        [Key]
        public string Id { get; set; } = default!;

        [Required]
        public string FullName { get; set; } = default!;

        [Required]
        public string Email { get; set; } = default!;

        [Required]
        public string Password { get; set; } = default!;

        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }

        [Required]
        public string Status { get; set; } = "ACTIVE";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? LastActiveAt { get; set; }

        public string? VehicleId { get; set; }
        public Vehicle? Vehicle { get; set; }
    }
}
