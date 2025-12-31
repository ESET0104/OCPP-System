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
            return CreatedAtAction(nameof(GetById), new { driverId = driver.DriverId }, driver);
        }

 
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _driverService.GetDriversAsync());
        }

     
        [HttpGet("{driverId}")]
        public async Task<IActionResult> GetById(string driverId)
        {
            return Ok(await _driverService.GetDriverByIdAsync(driverId));
        }

       
        [HttpPatch("{driverId}/status")]
        public async Task<IActionResult> UpdateStatus(
            string driverId,
            [FromBody] UpdateDriverStatusDto dto)
        {
            await _driverService.UpdateStatusAsync(driverId, dto.Status);
            return NoContent();
        }
    }
}

