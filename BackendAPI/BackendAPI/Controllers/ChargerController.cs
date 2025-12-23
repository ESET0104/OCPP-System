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
            var chargers = await _db.Chargers.ToListAsync();
            return Ok(chargers);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddCharger()
        {
            var charger = await _chargerService.RegisterAsync();
            return Ok(charger);
        }
    }

}
