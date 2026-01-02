using BackendAPI.Data.Entities.Users;
using BackendAPI.DTO;
using BackendAPI.DTO.Auth;
using BackendAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendAPI.Controllers
{
    [ApiController]
    [Route("api/managers")]
    [Authorize(Roles = "Admin")]
    public class ManagerController : ControllerBase
    {
        private readonly IUserRepository<Manager> _managerRepo;

        public ManagerController(IUserRepository<Manager> managerRepo)
        {
            _managerRepo = managerRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetManagers()
        {
            var managers = await _managerRepo.GetAllAsync();

            var result = managers.Select(m => new UserDTO
            {
                Id = m.Id,
                Username = m.Username,
                Email = m.Email,
                Status = m.Status,
                Company = m.Company,
                Department = m.Department,
                CreatedAt = m.CreatedAt,
                LastActiveAt = m.LastActiveAt
            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetManager(string id)
        {
            var manager = await _managerRepo.GetByIdAsync(id);
            if (manager == null) return NotFound();

            return Ok(new UserDTO
            {
                Id = manager.Id,
                Username = manager.Username,
                Email = manager.Email,
                Status = manager.Status,
                Company = manager.Company,
                Department = manager.Department,
                CreatedAt = manager.CreatedAt,
                LastActiveAt = manager.LastActiveAt
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateManager([FromBody] Manager manager)
        {
            var created = await _managerRepo.CreateAsync(manager);
            return Ok(created);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteManager(string id)
        {
            var manager = await _managerRepo.GetByIdAsync(id);
            if (manager == null) return NotFound();

            await _managerRepo.DeleteAsync(manager);
            return Ok("Manager deleted successfully");
        }

        [HttpPost("manager")]
        public async Task<IActionResult> CreateManager([FromBody] CreateUserDto dto)
        {
            var manager = new Manager
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

            var created = await _managerRepo.CreateAsync(manager);

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


        [HttpPut("managers/{id}")]
        public async Task<IActionResult> UpdateManager(string id, [FromBody] UpdateUserDto dto)
        {
            var manager = await _managerRepo.GetByIdAsync(id);
            if (manager == null) return NotFound();

            manager.Username = dto.Username ?? manager.Username;
            manager.Email = dto.Email ?? manager.Email;
            manager.Status = dto.Status ?? manager.Status;
            manager.Company = dto.Company ?? manager.Company;
            manager.Department = dto.Department ?? manager.Department;
            manager.UpdatedAt = DateTime.UtcNow;

            await _managerRepo.UpdateAsync(manager);

            return Ok(new
            {
                manager.Id,
                manager.Username,
                manager.Email,
                manager.Status,
                manager.Company,
                manager.Department,
                manager.UpdatedAt
            });
        }

        [HttpPatch("managers/{id}")]
        public async Task<IActionResult> PatchManager(string id, [FromBody] UpdateUserDto dto)
        {
            var manager = await _managerRepo.GetByIdAsync(id);
            if (manager == null) return NotFound();

            if (!string.IsNullOrEmpty(dto.Username)) manager.Username = dto.Username;
            if (!string.IsNullOrEmpty(dto.Email)) manager.Email = dto.Email;
            if (!string.IsNullOrEmpty(dto.Status)) manager.Status = dto.Status;
            if (!string.IsNullOrEmpty(dto.Company)) manager.Company = dto.Company;
            if (!string.IsNullOrEmpty(dto.Department)) manager.Department = dto.Department;

            manager.UpdatedAt = DateTime.UtcNow;

            await _managerRepo.UpdateAsync(manager);

            return Ok(manager);
        }
    }
}
