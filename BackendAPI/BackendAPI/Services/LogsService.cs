using BackendAPI.Data;
using BackendAPI.Data.Entities;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Services
{
    public class LogsService
    {
        private readonly AppDbContext _db;

        public LogsService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<LogEntry>> GetAllAsync()
        {
            var logs = await _db.Logs.ToListAsync();
            return logs;
        }

        public async Task<IEnumerable<LogEntry>> GetLogsByChargerAsync(string ChargerId)
        {
            return await _db.Logs.Where(l => l.ChargerId == ChargerId).ToListAsync();
        }
    }
}
