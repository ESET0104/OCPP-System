using BackendAPI.Data;
using BackendAPI.DTO.Auth_DTO;
using BackendAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/auth")]
[EnableRateLimiting("AuthPolicy")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public AuthController(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        if (string.IsNullOrEmpty(request.Role))
            return BadRequest("Role is required");

        switch (request.Role.ToLower())
        {
            case "admin":
                return await HandleLogin(
                    _context.Admins,
                    request.Username,
                    request.Password,
                    "Admin"
                );

            case "manager":
                return await HandleLogin(
                    _context.Managers,
                    request.Username,
                    request.Password,
                    "Manager"
                );

            case "supervisor":
                return await HandleLogin(
                    _context.Supervisors,
                    request.Username,
                    request.Password,
                    "Supervisor"
                );

            default:
                return BadRequest("Invalid role");
        }
    }

    private async Task<IActionResult> HandleLogin<T>(
        DbSet<T> table,
        string username,
        string password,
        string role
    ) where T : class
    {
        dynamic user = await table
            .FirstOrDefaultAsync(u => EF.Property<string>(u, "Username") == username);

        if (user == null || !PasswordHasher.Verify(password, user.Password))
            return Unauthorized("Invalid credentials");

        // Update last login + token time
        user.LastActiveAt = DateTime.UtcNow;
        user.Tokenat = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        await _context.SaveChangesAsync();

        var token = GenerateToken(user.Id, user.Username, role);

        return Ok(new
        {
            token,
            username = user.Username,
            role
        });
    }

    private string GenerateToken(string userId, string username, string role)
    {
        var jwt = _config.GetSection("Jwt");

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role),
            new Claim("role", role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwt["Key"])
        );

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                Convert.ToDouble(jwt["DurationInMinutes"])
            ),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }   
    }

}
