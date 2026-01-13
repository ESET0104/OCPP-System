using BackendAPI.Data;
using BackendAPI.Data.Entities;
using BackendAPI.DTO;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Services
{
    public class ReservationService
    {
        private readonly AppDbContext _context;

        public ReservationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Reservation> Create(CreateReservationDto dto)
        {
            var reservation = new Reservation
            {
                ChargerId = dto.ChargerId,
                DriverId = dto.DriverId,
                ConnectorId = dto.ConnectorId,
                CreatedBy = dto.CreatedBy,
                Status = "Active",
                StartTime = DateTime.UtcNow
            };

            _context.Reservations.Add(reservation);

            _context.Logs.Add(new LogEntry
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow,
                Source = "reservation",
                EventType = "RESERVATION_CREATED",
                Message = $"Reservation created for charger {dto.ChargerId}",
                ChargerId = dto.ChargerId,
                DriverId = dto.DriverId
            });

            await _context.SaveChangesAsync();
            return reservation;
        }



        public async Task<Reservation?> Cancel(string id, CancelReservationDto dto)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null) return null;

            reservation.Status = "Cancelled";
            reservation.CancelledBy = dto.CancelledBy;
            reservation.EndTime = DateTime.UtcNow;

            _context.Logs.Add(new LogEntry
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow,
                Source = "reservation",
                EventType = "RESERVATION_CANCELLED",
                Message = $"Reservation {id} cancelled by {dto.CancelledBy}",
                ChargerId = reservation.ChargerId,
                DriverId = reservation.DriverId
            });

            await _context.SaveChangesAsync();
            return reservation;
        }


        public async Task<List<object>> GetAll()
        {
            return await _context.Reservations
                .OrderByDescending(x => x.StartTime)
                .Select(x => new
                {
                    x.Status,
                    x.ChargerId,
                    x.DriverId,
                    x.StartTime,
                    x.EndTime,
                    x.ConnectorId,
                    x.CreatedBy,
                    x.CancelledBy
                })
                .ToListAsync<object>();
        }
    }
}
