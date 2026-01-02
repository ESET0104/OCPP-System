namespace BackendAPI.DTO.Auth
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public string Company { get; set; }
        public string Department { get; set; }
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public DateTime? LastActiveAt { get; set; }
    }

}
