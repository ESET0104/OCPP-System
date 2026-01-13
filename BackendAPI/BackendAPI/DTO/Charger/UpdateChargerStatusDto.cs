using System.ComponentModel.DataAnnotations;

namespace BackendAPI.DTO.Charger
{
    public class UpdateChargerStatusDto
    {
        [Required]
        public string Status { get; set; }
    }
}
