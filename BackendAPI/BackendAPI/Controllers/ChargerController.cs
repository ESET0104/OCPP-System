using BackendAPI.DTO.Charger;
using BackendAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackendAPI.Controllers
{
    [ApiController]
    [Route("api/chargers")]
    public class ChargerController : ControllerBase
    {
        private readonly ChargerService _chargerService;

        public ChargerController(ChargerService chargerService)
        {
            _chargerService = chargerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _chargerService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                return Ok(await _chargerService.GetByIdAsync(id));
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailable()
        {
            return Ok(await _chargerService.GetAvailableAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateChargerDto dto)
        {
            try
            {
                var charger = await _chargerService.RegisterAsync(dto.LocationId);
                return CreatedAtAction(nameof(GetById), new { id = charger.Id }, charger);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(string id,[FromBody] UpdateChargerStatusDto dto)
        {
            try
            {
                var updatedCharger =
                    await _chargerService.UpdateStatusAsync(id, dto.Status);

                return Ok(updatedCharger);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _chargerService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
