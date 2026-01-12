using BackendAPI.DTO.Notification;
using BackendAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendAPI.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    [Authorize(Roles = "Admin")]
    public class NotificationsController : ControllerBase
    {
        private readonly NotificationService _service;

        public NotificationsController(NotificationService service)
        {
            _service = service;
        }

        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] CreateNotificationDto dto)
        {
            var result = await _service.CreateAndSendAsync(dto);
            return Ok(result);
        }

        [HttpGet("scheduled")]
        public async Task<IActionResult> GetScheduled()
        {
            return Ok(await _service.GetScheduledAsync());
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistory()
        {
            return Ok(await _service.GetHistoryAsync());
        }
    }

}
