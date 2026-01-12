using BackendAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendAPI.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : ControllerBase
    {
        private readonly DashboardService _dashboardService;

        public DashboardController(DashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var result = await _dashboardService.GetSummaryAsync();
            return Ok(result);
        }

        [HttpGet("map")]
        public async Task<IActionResult> GetMap()
        {
            var result = await _dashboardService.GetMapAsync();
            return Ok(result);
        }

        [HttpGet("sessions/trend")]
        public async Task<IActionResult> GetSessionTrend(
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            if (from == default || to == default)
                return BadRequest("from and to dates are required");

            var result = await _dashboardService.GetSessionTrend(from, to);
            return Ok(result);
        }

    }
}
