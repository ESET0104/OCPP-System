using BackendAPI.Data;
using BackendAPI.Data.Entities;
using Microsoft.EntityFrameworkCore;
using NanoidDotNet;

namespace BackendAPI.Services
{
    public class ChargerService
    {
        private readonly AppDbContext _db;

        public ChargerService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<Charger>> GetAllAsync()
        {
            return await _db.Chargers
                .Include(c => c.Location)
                .ToListAsync();
        }

        public async Task<Charger> GetByIdAsync(string id)
        {
            var charger = await _db.Chargers
                .Include(c => c.Location)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (charger == null)
                throw new Exception("Charger not found");

            return charger;
        }

        public async Task<List<Charger>> GetAvailableAsync()
        {
            return await _db.Chargers
                .Where(c => c.Status == ChargerStatus.Available)
                .ToListAsync();
        }
        public async Task<Charger> RegisterAsync(string locationId)
        {
            var locationExists = await _db.Locations
                .AnyAsync(l => l.Id == locationId);

            if (!locationExists)
                throw new Exception("Location not found");

            var charger = new Charger
            {
                Id = Nanoid.Generate(size: 10),
                LocationId = locationId,
                Status = ChargerStatus.Available,
                LastSeen = DateTime.UtcNow
            };

            _db.Chargers.Add(charger);

            AddChargerLog(
                charger.Id,
                "CHARGER_CREATED",
                $"Charger created at location {locationId}"
            );

            await _db.SaveChangesAsync();

            return await _db.Chargers
                .Include(c => c.Location)
                .FirstAsync(c => c.Id == charger.Id);
        }



        public async Task<Charger> UpdateStatusAsync(string id, string status)
        {
            if (!IsValidStatus(status))
                throw new Exception("Invalid charger status");

            var charger = await _db.Chargers.FirstOrDefaultAsync(c => c.Id == id);
            if (charger == null)
                throw new Exception("Charger not found");

            var oldStatus = charger.Status;

            charger.Status = status;
            charger.LastSeen = DateTime.UtcNow;

            AddChargerLog(
                charger.Id,
                "CHARGER_STATUS_CHANGED",
                $"Status changed from {oldStatus} to {status}"
            );

            await _db.SaveChangesAsync();
            return charger;
        }



        public async Task DeleteAsync(string id)
        {
            var charger = await _db.Chargers.FirstOrDefaultAsync(c => c.Id == id);
            if (charger == null)
                throw new Exception("Charger not found");

            AddChargerLog(
                charger.Id,
                "CHARGER_DELETED",
                "Charger removed from system"
            );

            _db.Chargers.Remove(charger);
            await _db.SaveChangesAsync();
        }


        private void AddChargerLog(string chargerId,string eventType,string message)
        {
            _db.Logs.Add(new LogEntry
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow,
                Source = "charger",
                EventType = eventType,
                Message = message,
                ChargerId = chargerId
            });
        }


        private static bool IsValidStatus(string status) =>
            status == ChargerStatus.Available ||
            status == ChargerStatus.Preparing ||
            status == ChargerStatus.Engaged ||
            status == ChargerStatus.Faulted;
    }
}
