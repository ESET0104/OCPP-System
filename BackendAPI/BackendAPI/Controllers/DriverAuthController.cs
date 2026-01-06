using BackendAPI.Data;

using BackendAPI.DTO.Auth.Driver;
using BackendAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BackendAPI.Controllers
{
    [ApiController]
    [Route("api/driver/auth")]
    public class DriverAuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public DriverAuthController(
            AppDbContext context,
            IConfiguration config,
            PasswordHasher hasher)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] DriverLoginRequest request)
        {
            var driver = await _context.Drivers
                .SingleOrDefaultAsync(d => d.Email == request.Email);

            if (driver == null)
            {
                return Unauthorized("Invalid credentials");
            }

            // 2️⃣ Check if stored password is valid
            if (string.IsNullOrWhiteSpace(driver.Password))
            {
                return Unauthorized("Invalid credentials");
            }

            // 3️⃣ Verify password (hashed vs plain)
            if (!PasswordHasher.Verify(request.Password, driver.Password))
            {
                return Unauthorized("Invalid credentials");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, driver.Id),
                new Claim(ClaimTypes.Email, driver.Email),
                new Claim("user_type", "DRIVER")
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
            );

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(12),
                signingCredentials: new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256
                )
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }
    }
}
