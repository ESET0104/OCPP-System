using BackendAPI.Data.Entities.Users;
using BackendAPI.DTO;
using BackendAPI.DTO.Auth;
using BackendAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendAPI.Controllers
{
    [ApiController]
    [Route("api/supervisors")]
    [Authorize(Roles = "Admin,Manager")]
    public class SupervisorController : ControllerBase
    {
        private readonly IUserRepository<Supervisor> _supervisorRepo;

        public SupervisorController(IUserRepository<Supervisor> supervisorRepo)
        {
            _supervisorRepo = supervisorRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetSupervisors()
        {
            var supervisors = await _supervisorRepo.GetAllAsync();

            var result = supervisors.Select(s => new UserDTO
            {
                Id = s.Id,
                Username = s.Username,
                Email = s.Email,
                Status = s.Status,
                Company = s.Company,
                Department = s.Department,
                CreatedAt = s.CreatedAt,
                LastActiveAt = s.LastActiveAt
            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSupervisor(string id)
        {
            var supervisor = await _supervisorRepo.GetByIdAsync(id);
            if (supervisor == null) return NotFound();

            return Ok(new UserDTO
            {
                Id = supervisor.Id,
                Username = supervisor.Username,
                Email = supervisor.Email,
                Status = supervisor.Status,
                Company = supervisor.Company,
                Department = supervisor.Department,
                CreatedAt = supervisor.CreatedAt,
                LastActiveAt = supervisor.LastActiveAt
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateSupervisor([FromBody] Supervisor supervisor)
        {
            var created = await _supervisorRepo.CreateAsync(supervisor);
            return Ok(created);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSupervisor(string id)
        {
            var supervisor = await _supervisorRepo.GetByIdAsync(id);
            if (supervisor == null) return NotFound();

            await _supervisorRepo.DeleteAsync(supervisor);
            return Ok("Supervisor deleted");
        }

        [HttpPost("supervisor")]
        public async Task<IActionResult> CreateSupervisor([FromBody] CreateUserDto dto)
        {
            var supervisor = new Supervisor
            {
                Id = Guid.NewGuid().ToString(),
                Username = dto.Username,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Status = dto.Status,
                Company = dto.Company,
                Department = dto.Department,
                CreatedAt = DateTime.UtcNow,
                Tokenat = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };

            var created = await _supervisorRepo.CreateAsync(supervisor);

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


        // ======================= SUPERVISOR =======================

        [HttpPut("supervisors/{id}")]
        public async Task<IActionResult> UpdateSupervisor(string id, [FromBody] UpdateUserDto dto)
        {
            var supervisor = await _supervisorRepo.GetByIdAsync(id);
            if (supervisor == null) return NotFound();

            supervisor.Username = dto.Username ?? supervisor.Username;
            supervisor.Email = dto.Email ?? supervisor.Email;
            supervisor.Status = dto.Status ?? supervisor.Status;
            supervisor.Company = dto.Company ?? supervisor.Company;
            supervisor.Department = dto.Department ?? supervisor.Department;
            supervisor.UpdatedAt = DateTime.UtcNow;

            await _supervisorRepo.UpdateAsync(supervisor);

            return Ok(supervisor);
        }

        [HttpPatch("supervisors/{id}")]
        public async Task<IActionResult> PatchSupervisor(string id, [FromBody] UpdateUserDto dto)
        {
            var supervisor = await _supervisorRepo.GetByIdAsync(id);
            if (supervisor == null) return NotFound();

            if (!string.IsNullOrEmpty(dto.Username)) supervisor.Username = dto.Username;
            if (!string.IsNullOrEmpty(dto.Email)) supervisor.Email = dto.Email;
            if (!string.IsNullOrEmpty(dto.Status)) supervisor.Status = dto.Status;
            if (!string.IsNullOrEmpty(dto.Company)) supervisor.Company = dto.Company;
            if (!string.IsNullOrEmpty(dto.Department)) supervisor.Department = dto.Department;

            supervisor.UpdatedAt = DateTime.UtcNow;

            await _supervisorRepo.UpdateAsync(supervisor);

            return Ok(supervisor);
        }


    }
}
