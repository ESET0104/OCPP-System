using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendAPI.Data.Entities
{
    public class Charger
    {
        [Key] public string Id { get; set; }
        [Required] public string Status { get; set; }
        [Required] public DateTime LastSeen { get; set; }

        [Required]
        [ForeignKey(nameof(Location))]
        public string LocationId { get; set; }
        public Location Location { get; set; }

    }

    public static class ChargerStatus
    {
        public const string Available = "Available";
        public const string Preparing = "Preparing";
        public const string Engaged = "Engaged";
        public const string Faulted = "Faulted";
    }

}
