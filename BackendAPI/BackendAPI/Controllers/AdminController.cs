using BackendAPI.Data.Entities.Users;
using BackendAPI.Repositories;
using BackendAPI.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BackendAPI.DTO.Auth;

namespace BackendAPI.Controllers
{
    [ApiController]
    [Route("api/admins")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IUserRepository<Admin> _adminRepo;

        public AdminController(IUserRepository<Admin> adminRepo)
        {
            _adminRepo = adminRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var admins = await _adminRepo.GetAllAsync();

            var result = admins.Select(a => new UserDTO
            {
                Id = a.Id,
                Username = a.Username,
                Email = a.Email,
                Status = a.Status,
                Company = a.Company,
                Department = a.Department,
                CreatedAt = a.CreatedAt,
                LastActiveAt = a.LastActiveAt
            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var admin = await _adminRepo.GetByIdAsync(id);
            if (admin == null) return NotFound();

            return Ok(new UserDTO
            {
                Id = admin.Id,
                Username = admin.Username,
                Email = admin.Email,
                Status = admin.Status,
                Company = admin.Company,
                Department = admin.Department,
                CreatedAt = admin.CreatedAt,
                LastActiveAt = admin.LastActiveAt
            });
        }

        

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var admin = await _adminRepo.GetByIdAsync(id);
            if (admin == null) return NotFound();

            await _adminRepo.DeleteAsync(admin);
            return Ok("Admin deleted");
        }

        [HttpPost("admins")]
        public async Task<IActionResult> CreateAdmin([FromBody] CreateUserDto dto)
        {
            var admin = new Admin
            {
                Id = Guid.NewGuid().ToString(),
                Username = dto.Username,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password), // hash password
                Status = dto.Status,
                Company = dto.Company,
                Department = dto.Department,
                CreatedAt = DateTime.UtcNow,
                Tokenat = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };

            var created = await _adminRepo.CreateAsync(admin);

            // Return safe response (no password)
            var response = new UserDTO
            {
                Id = created.Id,
                Username = created.Username,
                Email = created.Email,
                Status = created.Status,
                Company = created.Company,
                Department = created.Department,
                CreatedAt = created.CreatedAt,
                LastActiveAt = created.LastActiveAt
            };

            return Ok(response);
        }

        [HttpPut("admins/{id}")]
        public async Task<IActionResult> UpdateAdmin(string id, [FromBody] UpdateUserDto dto)
        {
            var admin = await _adminRepo.GetByIdAsync(id);
            if (admin == null) return NotFound();

            admin.Username = dto.Username ?? admin.Username;
            admin.Email = dto.Email ?? admin.Email;
            admin.Status = dto.Status ?? admin.Status;
            admin.Company = dto.Company ?? admin.Company;
            admin.Department = dto.Department ?? admin.Department;
            admin.UpdatedAt = DateTime.UtcNow;

            await _adminRepo.UpdateAsync(admin);

            return Ok(new
            {
                admin.Id,
                admin.Username,
                admin.Email,
                admin.Status,
                admin.Company,
                admin.Department,
                admin.UpdatedAt
            });
        }

        [HttpPatch("admins/{id}")]
        public async Task<IActionResult> PatchAdmin(string id, [FromBody] UpdateUserDto dto)
        {
            var admin = await _adminRepo.GetByIdAsync(id);
            if (admin == null) return NotFound();

            if (!string.IsNullOrEmpty(dto.Username)) admin.Username = dto.Username;
            if (!string.IsNullOrEmpty(dto.Email)) admin.Email = dto.Email;
            if (!string.IsNullOrEmpty(dto.Status)) admin.Status = dto.Status;
            if (!string.IsNullOrEmpty(dto.Company)) admin.Company = dto.Company;
            if (!string.IsNullOrEmpty(dto.Department)) admin.Department = dto.Department;

            admin.UpdatedAt = DateTime.UtcNow;

            await _adminRepo.UpdateAsync(admin);

            return Ok(admin);
        }


    }
}
