using BackendAPI.Data;
using BackendAPI.DTO.Dashboard;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Services
{
    public class DashboardService
    {
        private readonly AppDbContext _context;

        public DashboardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardSummaryDto> GetSummaryAsync()
        {
            var totalChargersTask = _context.Chargers.CountAsync();
            var activeChargersTask = _context.Chargers
                .CountAsync(c => c.Status == "ACTIVE");

            var inactiveChargersTask = _context.Chargers
                .CountAsync(c => c.Status != "ACTIVE");

            var liveSessionsTask = _context.ChargingSessions
                .CountAsync(s => s.Status == "ONGOING");

            var driversTask = _context.Drivers.CountAsync();

            var energyTask = _context.ChargingSessions
                .Where(s => s.EnergyConsumedKwh != null)
                .SumAsync(s => s.EnergyConsumedKwh.Value);

            await Task.WhenAll(
                totalChargersTask,
                activeChargersTask,
                inactiveChargersTask,
                liveSessionsTask,
                driversTask,
                energyTask
            );

            var energyMwh = energyTask.Result / 1000; // assuming Wh stored

            return new DashboardSummaryDto
            {
                TotalChargePoints = totalChargersTask.Result,
                ActiveChargePoints = activeChargersTask.Result,
                InactiveChargePoints = inactiveChargersTask.Result,
                LiveSessions = liveSessionsTask.Result,
                Drivers = driversTask.Result,
                EnergyMwh = Math.Round(energyMwh, 2),
                Co2SavedTonnes = CalculateCo2(energyMwh)
            };
        }

        private decimal CalculateCo2(decimal energyMwh)
        {
            // Industry standard approx: 0.7 ton CO2 per MWh
            const decimal factor = 0.7m;
            return Math.Round(energyMwh * factor, 2);
        }

        public async Task<List<MapChargerDto>> GetMapAsync()
        {
            return await _context.Chargers
                .Include(c => c.Location)
                .Select(c => new MapChargerDto
                {
                    ChargerId = c.Id,
                    Status = c.Status,
                    LocationName = c.Location.Name,
                    Latitude = c.Location.Latitude,
                    Longitude = c.Location.Longitude
                })
                .ToListAsync();
        }

        public async Task<List<SessionTrendDto>> GetSessionTrend(DateTime from, DateTime to)
        {
            return await _context.ChargingSessions
                .Where(s => s.StartTime.Date >= from.Date &&
                            s.StartTime.Date <= to.Date)
                .GroupBy(s => s.StartTime.Date)
                .Select(g => new SessionTrendDto
                {
                    Date = g.Key,
                    Count = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToListAsync();
        }
    }
}
