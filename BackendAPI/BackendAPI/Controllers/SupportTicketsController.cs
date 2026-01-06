using BackendAPI.Data;
using BackendAPI.DTO;
using BackendAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Controllers
{
    [ApiController]
    [Route("api/support-tickets")]
    public class SupportTicketsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly TicketService _service;

        public SupportTicketsController(AppDbContext context, TicketService service)
        {
            _context = context;
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTicketDto dto)
        {
            var ticket = await _service.Create(dto);
            return Ok(ticket);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _context.SupportTickets
                .Include(x => x.Screenshots)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var ticket = await _context.SupportTickets
                .Include(x => x.Screenshots)
                .FirstOrDefaultAsync(x => x.Id == id);

            return ticket == null ? NotFound() : Ok(ticket);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(string id, [FromBody] string status)
        {
            var ticket = await _context.SupportTickets.FindAsync(id);
            if (ticket == null) return NotFound();

            ticket.Status = status;
            ticket.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return Ok(ticket);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var ticket = await _context.SupportTickets.FindAsync(id);
            if (ticket == null) return NotFound();

            var screenshots = _context.TicketScreenshots.Where(x => x.SupportTicketId == id);
            _context.TicketScreenshots.RemoveRange(screenshots);
            _context.SupportTickets.Remove(ticket);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
