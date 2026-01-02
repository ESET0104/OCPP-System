using System.ComponentModel.DataAnnotations;

namespace BackendAPI.Data.Entities.Users
{
    public class Admin : IUser
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Status { get; set; }

        public string Company { get; set; }

        public string Department { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public DateTime? LastActiveAt { get; set; }

        [Required]
        public long Tokenat { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

    }
}