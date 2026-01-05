using BackendAPI.Data;
using BackendAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly LogsService _logsService;

        public LogsController(AppDbContext db, LogsService logsService)
        {
            _db = db;
            _logsService = logsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetallLogs()
        {
            try
            {
                var result = await _logsService.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Charger{id}")]
        public async Task<IActionResult> GetLogsByCharger(string id)
        {
            try
            {
                var result = await _logsService.GetLogsByChargerAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
