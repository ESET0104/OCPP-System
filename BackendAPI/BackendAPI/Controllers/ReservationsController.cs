using BackendAPI.DTO;
using BackendAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackendAPI.Controllers
{
    [ApiController]
    [Route("api/reservations")]
    public class ReservationsController : ControllerBase
    {
        private readonly ReservationService _service;

        public ReservationsController(ReservationService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReservationDto dto)
        {
            var reservation = await _service.Create(dto);
            return Ok(reservation);
        }

       
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> Cancel(string id,[FromBody] CancelReservationDto dto)
        {
            var reservation = await _service.Cancel(id, dto);
            if (reservation == null) return NotFound();
            return Ok(reservation);
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.GetAll();
            return Ok(data);
        }
    }
}
