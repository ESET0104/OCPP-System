using BackendAPI.Data.Entities.Users;
using BackendAPI.Repositories;
using BackendAPI.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BackendAPI.DTO.Auth;

namespace BackendAPI.Controllers
{
    [ApiController]
    [Route("api/users")]
    //[Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository<Admin> _adminRepo;
        private readonly IUserRepository<Manager> _managerRepo;
        private readonly IUserRepository<Supervisor> _supervisorRepo;

        public UserController(
            IUserRepository<Admin> adminRepo,
            IUserRepository<Manager> managerRepo,
            IUserRepository<Supervisor> supervisorRepo)
        {
            _adminRepo = adminRepo;
            _managerRepo = managerRepo;
            _supervisorRepo = supervisorRepo;
        }

        // ---------------- ADMIN ----------------

        [HttpGet("admins")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAdmins()
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


        [HttpGet("admins/{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAdminById(string id)
        {
            var admin = await _adminRepo.GetByIdAsync(id);
            if (admin == null) return NotFound();

            var result = new UserDTO
            {
                Id = admin.Id,
                Username = admin.Username,
                Email = admin.Email,
                Status = admin.Status,
                Company = admin.Company,
                Department = admin.Department,
                CreatedAt = admin.CreatedAt,
                LastActiveAt = admin.LastActiveAt
            };

            return Ok(result);
        }

        [HttpDelete("admins/{id}")]
        public async Task<IActionResult> DeleteAdmin(string id)
        {
            var admin = await _adminRepo.GetByIdAsync(id);
            if (admin == null) return NotFound();

            await _adminRepo.DeleteAsync(admin);
            return Ok("Admin deleted");
        }

        // ---------------- MANAGER ----------------

        [HttpGet("managers")]
        //[Authorize(Roles = "Admin")]
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

        // ---------------- SUPERVISOR ----------------

        [HttpGet("supervisors")]
        //[Authorize(Roles = "Admin,Manager")]
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
    }
}
