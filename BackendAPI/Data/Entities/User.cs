using System.ComponentModel.DataAnnotations;

namespace BackendAPI.Data.Entities
{
    public class User
    {
        [Key] public string Id { get; set; }
        [Required] public string Name { get; set; }
    }
}
