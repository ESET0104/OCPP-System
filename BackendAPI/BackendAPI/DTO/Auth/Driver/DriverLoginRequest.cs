namespace BackendAPI.DTO.Auth.Driver
{
    public class DriverLoginRequest
    {
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}
