using BackendAPI.DTO;
using BackendAPI.DTO.Auth;
using BackendAPI.Services;
using BackendAPI.Services.UserServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendAPI.Controllers
{
    [ApiController]
    [Route("api/supervisors")]
    // [Authorize(Roles = "Admin,Manager")]
    public class SupervisorController : ControllerBase
    {
        private readonly SupervisorService _service;

        public SupervisorController(SupervisorService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
            => (await _service.GetByIdAsync(id)) is { } s ? Ok(s) : NotFound();

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserDto dto)
            => Ok(await _service.CreateAsync(dto));

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, UpdateUserDto dto)
            => (await _service.UpdateAsync(id, dto)) is { } s ? Ok(s) : NotFound();

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(string id, UpdateUserDto dto)
            => (await _service.PatchAsync(id, dto)) is { } s ? Ok(s) : NotFound();

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
            => await _service.DeleteAsync(id)
                ? Ok("Supervisor deleted")
                : NotFound();
    }
}
