namespace BackendAPI.Data.Entities.Users
{
    public interface IUser
    {
        string Id { get; set; }
        string Username { get; set; }
        string Email { get; set; }

        string Password { get; set; }
        string Status { get; set; }
        string Company { get; set; }
        string Department { get; set; }
        DateTime CreatedAt { get; set; }
        DateTime? UpdatedAt { get; set; }
        DateTime? LastActiveAt { get; set; }

        long Tokenat { get; set; }
    }

}
