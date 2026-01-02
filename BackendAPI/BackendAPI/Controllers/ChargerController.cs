using BackendAPI.Data;
using BackendAPI.Data.Entities;
using BackendAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Controllers
{
    [Route("api/chargers")]
    [ApiController]
    public class ChargerController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ChargerService _chargerService;

        public ChargerController(AppDbContext db, ChargerService chargerService)
        {
            _db = db;
            _chargerService = chargerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var chargers = await _chargerService.GetallChargers();
            return Ok(chargers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var charger = _chargerService.Getcharger(id);
            if (charger == null)
            {
                return BadRequest("Charger not found");
            }
            return Ok(charger);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddCharger()
        {
            var charger = await _chargerService.RegisterAsync();
            return Ok(charger);
        }

        [HttpPut("Update{id}")]
        public async Task<IActionResult> UpdateCharger(string id, [FromBody] string Status)
        {
            try
            {
                var res = await _chargerService.UpdateAsync(id, Status);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCharger(string id)
        {
            try
            {
                await _chargerService.DeleteAsync(id);
                return Ok("charger removed");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

}
