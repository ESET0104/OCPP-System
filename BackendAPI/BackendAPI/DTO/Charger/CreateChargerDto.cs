using System.ComponentModel.DataAnnotations;

namespace BackendAPI.DTO.Charger
{
    public class CreateChargerDto
    {
        [Required]
        public string LocationId { get; set; }
    }
}
