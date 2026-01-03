using BackendAPI.Data;
using BackendAPI.Data.Entities;
using BackendAPI.DTO;

namespace BackendAPI.Services
{
    public class TicketService
    {
        private readonly AppDbContext _context;

        public TicketService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<SupportTicket> Create(CreateTicketDto dto)
        {
            var ticket = new SupportTicket
            {
                Status = dto.Status,
                Category = dto.Category,
                Priority = dto.Priority,
                Title = dto.Title,
                Description = dto.Description,
                ChargerId = dto.ChargerId,
                VehicleId = dto.VehicleId,
                CreatedByUserId = dto.CreatedByUserId,
                CreatedByName = dto.CreatedByName
            };

            _context.SupportTickets.Add(ticket);
            await _context.SaveChangesAsync();

            if (dto.Screenshots != null)
            {
                foreach (var img in dto.Screenshots)
                {
                    _context.TicketScreenshots.Add(new TicketScreenshot
                    {
                        SupportTicketId = ticket.Id,
                        ImageUrl = img

                    });
                }
                await _context.SaveChangesAsync();
            }

            return ticket;
        }
    }
}
