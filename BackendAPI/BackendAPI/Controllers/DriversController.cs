using BackendAPI.DTO.Driver;
using BackendAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackendAPI.Controllers
{
    [ApiController]
    [Route("api/drivers")]
    public class DriversController : ControllerBase
    {
        private readonly DriverService _driverService;

        public DriversController(DriverService driverService)
        {
            _driverService = driverService;
        }

     
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDriverDto dto)
        {
            var driver = await _driverService.CreateDriverAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = driver.Id },
                driver
            );
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _driverService.GetDriversAsync());
        }

   
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            return Ok(await _driverService.GetDriverByIdAsync(id));
        }

      
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(
            string id,
            [FromBody] UpdateDriverStatusDto dto)
        {
            await _driverService.UpdateStatusAsync(id, dto.Status);
            return NoContent();
        }
    }
}
