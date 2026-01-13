using BackendAPI.DTO.Charger;
using BackendAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackendAPI.DTO;

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

        [HttpGet("Available")]
        public async Task<IActionResult> GetAvailableChargers()
        {
            try
            {
                var chargers = await _chargerService.GetAvailableChargers();
                return Ok(chargers);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //[HttpPost("Add")]
        //public async Task<IActionResult> AddCharger()
        //{
        //    var charger = await _chargerService.RegisterAsync();
        //    return Ok(charger);
        //}

        [HttpPost]
        public async Task<IActionResult> AddCharger([FromBody] CreateChargerRequest request)
        {
            var charger = await _chargerService.RegisterAsync(
                request.LocationId,
                request.Status
            );
            return Ok(charger);
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
