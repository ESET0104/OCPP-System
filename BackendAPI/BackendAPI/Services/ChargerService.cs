using BackendAPI.Data;
using BackendAPI.Data.Entities;
using Microsoft.EntityFrameworkCore;
using NanoidDotNet;
using System.Collections;

namespace BackendAPI.Services
{
    public class ChargerService
    {
        private readonly AppDbContext _db;

        public ChargerService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Charger>> GetallChargers()
        {
            return await _db.Chargers.ToListAsync();

        }

        public async Task<Charger> Getcharger(string id)
        {
            return await _db.Chargers.FirstOrDefaultAsync(_ => _.Id == id);
        }

        public async Task<IEnumerable<Charger>> GetAvailableChargers()
        {
            return await _db.Chargers.Where(c => c.Status == "Available").ToListAsync();
        }

        public async Task<Charger> RegisterAsync()
        {
            var charger = new Charger
            {
                Id = Nanoid.Generate(size: 10),
                Status = "Available",
                LastSeen = DateTime.UtcNow
            };
            _db.Chargers.Add(charger);

            await _db.SaveChangesAsync();
            return charger;
        }

        public async Task<Charger> UpdateAsync(string id, string Status)
        {
            var charger = await _db.Chargers.FirstOrDefaultAsync(c => c.Id == id);
            if (charger == null)
            {
                throw new Exception("No charger found");
            }
            charger.Status = Status;
            await _db.SaveChangesAsync();
            return charger;
            
        }

        public async Task DeleteAsync(string id)
        {
            var charger = await _db.Chargers.FirstOrDefaultAsync(_ => _.Id == id);
            if (charger == null)
            {
                throw new Exception("No charger found");
            }
            _db.Chargers.Remove(charger);
            await _db.SaveChangesAsync();
        }

    }

}
