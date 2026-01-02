using System.ComponentModel.DataAnnotations;

namespace BackendAPI.Data.Entities
{
    public class Location
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Address { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }
    }
}
