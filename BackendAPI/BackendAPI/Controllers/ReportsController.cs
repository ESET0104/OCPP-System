using BackendAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackendAPI.Controllers
{
    [ApiController]
    [Route("api/reports")]
    public class ReportsController : ControllerBase
    {
        private readonly ReportsService _service;

        public ReportsController(ReportsService service)
        {
            _service = service;
        }

        [HttpGet("analytics")]
        public async Task<IActionResult> Analytics(
            DateTime start,
            DateTime end,
            string? locationId = null)
        {
            return Ok(await _service.GetAnalytics(start, end, locationId));
        }

        [HttpGet("summary")]
        public async Task<IActionResult> Summary(
            DateTime start,
            DateTime end,
            string? locationId = null)
        {
            return Ok(await _service.GetSummary(start, end, locationId));
        }

        [HttpGet("peak-load")]
        public async Task<IActionResult> PeakLoad(
            DateTime start,
            DateTime end)
        {
            return Ok(await _service.GetPeakLoad(start, end));
        }

        [HttpGet("queue-time")]
        public async Task<IActionResult> QueueTime(
            DateTime start,
            DateTime end)
        {
            return Ok(await _service.GetQueueTime(start, end));
        }
    }
}
