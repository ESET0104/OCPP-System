using BackendAPI.DTO.Vehicle;
using BackendAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackendAPI.Controllers
{
    [ApiController]
    [Route("api/vehicles")]
    public class VehiclesController : ControllerBase
    {
        private readonly VehicleService _vehicleService;

        public VehiclesController(VehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

       
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateVehicleDto dto)
        {
            var vehicle = await _vehicleService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { vehicleId = vehicle.VehicleId }, vehicle);
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _vehicleService.GetAllAsync());
        }

        
        [HttpGet("{vehicleId}")]
        public async Task<IActionResult> GetById(string vehicleId)
        {
            return Ok(await _vehicleService.GetByIdAsync(vehicleId));
        }

        
        [HttpDelete("{vehicleId}")]
        public async Task<IActionResult> Delete(string vehicleId)
        {
            await _vehicleService.DeleteAsync(vehicleId);
            return NoContent();
        }
    }
}
