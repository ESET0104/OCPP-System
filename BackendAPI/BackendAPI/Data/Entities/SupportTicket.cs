using System.ComponentModel.DataAnnotations;

namespace BackendAPI.Data.Entities
{
    public class SupportTicket
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string Status { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        public string Priority { get; set; }

        [Required]
        public string Title { get; set; }

        public string? Description { get; set; }

        public string? ChargerId { get; set; }
        //public Charger? Charger { get; set; }

        public string? VehicleId { get; set; }
        //public Vehicle? Vehicle { get; set; }

        [Required]
        public string CreatedByUserId { get; set; }

        [Required]
        public string CreatedByName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<TicketScreenshot> Screenshots { get; set; }

    }
}
