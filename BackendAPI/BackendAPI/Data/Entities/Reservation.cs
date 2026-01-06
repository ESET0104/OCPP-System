using System.ComponentModel.DataAnnotations;

namespace BackendAPI.Data.Entities
{
    public class Reservation
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string ChargerId { get; set; }
       

        [Required]
        public string DriverId { get; set; }
       
        public int ConnectorId { get; set; }

        [Required]
        public string Status { get; set; }   

        [Required]
        public string CreatedBy { get; set; }   

        public string? CancelledBy { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
