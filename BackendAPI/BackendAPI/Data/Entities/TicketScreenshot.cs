using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BackendAPI.Data.Entities
{
    public class TicketScreenshot
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string ImageUrl { get; set; }

        [Required]
        [ForeignKey("SupportTicket")]
        public string SupportTicketId { get; set; }

        [JsonIgnore]
        public SupportTicket SupportTicket { get; set; }
    }
}
