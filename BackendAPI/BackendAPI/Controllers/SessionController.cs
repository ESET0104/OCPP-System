using BackendAPI.Data;
using BackendAPI.Data.Entities;
using BackendAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Controllers
{
    [Route("api/sessions")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly SessionService _sessionService;

        public SessionController(AppDbContext db, SessionService sessionService)
        {
            _db = db;
            _sessionService = sessionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetSessions()
        {
            return Ok(await _sessionService.GetAllSessions());
        }

        [HttpGet("Driver{DriverId}")]
        public async Task<IActionResult> GetSessionsPerDriverAsync(string DriverId)
        {
            try
            {
                var sessions = await _sessionService.GetSessionsByDriver(DriverId);
                return Ok(sessions);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Charger{ChargerId}")]
        public async Task<IActionResult> GetSessionsPerChargerAsync(string ChargerId)
        {
            try
            {
                var sessions = await _sessionService.GetSessionsByCharger(ChargerId);
                return Ok(sessions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Info{SessionId}")]
        public async Task<IActionResult> GetSessionInfoAsync(string SessionId)
        {
            try
            {
                var result = await _sessionService.GetSessionInfo(SessionId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartSession([FromBody] StartSessionReq req)
        {
            try
            {
                var sessionId = await _sessionService
                .StartSessionAsync(req.ChargerId, req.DriverId);
                return Ok(new { sessionId });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            

            
        }

        [HttpPost("stop")]
        public async Task<IActionResult> StopSession([FromBody] StopSessionReq req)
        {
            try
            {
                await _sessionService.StopSessionAsync(req.SessionId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }
    }

    public class StartSessionReq
    {
        public string ChargerId { get; set; }
        public string DriverId { get; set; }
    }

    public class StopSessionReq
    {
        public string SessionId { get; set; }
    }
}
