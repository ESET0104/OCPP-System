using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendAPI.Data.Entities
{
    public class ChargingSession
    {
        [Key] public string Id { get; set; }
        [Required][ForeignKey("Charger")] public string ChargerId { get; set; }
        public Charger Charger { get; set; }
        [Required][ForeignKey("User")] public string UserId { get; set; }
        public User User { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public decimal? EnergyConsumedKwh {  get; set; }
        [Required] public string Status { get; set; }
        public DateTime LastMeterUpdate { get; set; }
    
    }

    public static class SessionStatus
    {
        public const string Pending = "Pending";
        public const string Active = "Active";
        public const string Stopping = "Stopping";
        public const string Completed = "Completed";
        public const string Faulted = "Faulted";
    }
}
