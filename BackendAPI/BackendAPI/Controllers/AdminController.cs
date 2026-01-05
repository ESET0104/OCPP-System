using BackendAPI.Data.Entities.Users;
using BackendAPI.DTO;
using BackendAPI.DTO.Auth;
using BackendAPI.Repositories;
using BackendAPI.Services.UserServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendAPI.Controllers
{
    [ApiController]
    [Route("api/admins")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly AdminService _service;

        public AdminController(AdminService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
            => (await _service.GetByIdAsync(id)) is { } u ? Ok(u) : NotFound();

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserDto dto)
            => Ok(await _service.CreateAsync(dto));

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, UpdateUserDto dto)
            => (await _service.UpdateAsync(id, dto)) is { } u ? Ok(u) : NotFound();

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(string id, UpdateUserDto dto)
            => (await _service.PatchAsync(id, dto)) is { } u ? Ok(u) : NotFound();

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
            => await _service.DeleteAsync(id) ? Ok("Admin deleted") : NotFound();
    }
}
