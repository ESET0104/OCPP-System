using BackendAPI.DTO;
using BackendAPI.DTO.Auth;
using BackendAPI.Services;
using BackendAPI.Services.UserServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendAPI.Controllers
{
    [ApiController]
    [Route("api/managers")]
    // [Authorize(Roles = "Admin")]
    public class ManagerController : ControllerBase
    {
        private readonly ManagerService _service;

        public ManagerController(ManagerService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
            => (await _service.GetByIdAsync(id)) is { } m ? Ok(m) : NotFound();

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserDto dto)
            => Ok(await _service.CreateAsync(dto));

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, UpdateUserDto dto)
            => (await _service.UpdateAsync(id, dto)) is { } m ? Ok(m) : NotFound();

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(string id, UpdateUserDto dto)
            => (await _service.PatchAsync(id, dto)) is { } m ? Ok(m) : NotFound();

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
            => await _service.DeleteAsync(id)
                ? Ok("Manager deleted successfully")
                : NotFound();
    }
}
