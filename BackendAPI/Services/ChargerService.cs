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

        public async Task<Charger> RegisterAsync()
        {
            //var charger = await _db.Chargers
            //    .FirstOrDefaultAsync(c => c.Id == chargerId);

            //if (charger == null)
            //{
            var charger = new Charger
            {
                Id = Nanoid.Generate(size: 10),
                //ChargerId = chargerId,
                Status = "Available",
                LastSeen = DateTime.UtcNow
            };
            _db.Chargers.Add(charger);
            //}
            //else
            //{
            //    charger.LastSeen = DateTime.UtcNow;
            //}

            await _db.SaveChangesAsync();
            return charger;
        }

        public async Task SetStatusAsync(string chargerId, string status)
        {
            var charger = await _db.Chargers
                .FirstAsync(c => c.Id == chargerId);

            charger.Status = status;
            await _db.SaveChangesAsync();
        }

        public async Task UpdateHeartbeat(string chargerId, DateTime Timestamp)
        {
            var charger = await _db.Chargers.FirstOrDefaultAsync(c => c.Id == chargerId);

            charger.LastSeen = Timestamp;
            await _db.SaveChangesAsync();
        }


    }

}
