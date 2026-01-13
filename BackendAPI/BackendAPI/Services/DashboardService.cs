using BackendAPI.Data;
using BackendAPI.Data.Entities;
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
            var totalChargers = await _context.Chargers.CountAsync();

            var activeChargers = await _context.Chargers
                .CountAsync(c => c.Status == "ACTIVE");

            var inactiveChargers = await _context.Chargers
                .CountAsync(c => c.Status != "ACTIVE");

            var liveSessions = await _context.ChargingSessions
                .CountAsync(s => s.Status == "ONGOING");

            var drivers = await _context.Drivers.CountAsync();

            var energyWh = await _context.ChargingSessions
                .Where(s => s.EnergyConsumedKwh != null)
                .SumAsync(s => s.EnergyConsumedKwh.Value);

            var energyMwh = energyWh / 1000;

            return new DashboardSummaryDto
            {
                TotalChargePoints = totalChargers,
                ActiveChargePoints = activeChargers,
                InactiveChargePoints = inactiveChargers,
                LiveSessions = liveSessions,
                Drivers = drivers,
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
            var fromUtc = DateTime.SpecifyKind(from, DateTimeKind.Utc);
            var toUtc = DateTime.SpecifyKind(to, DateTimeKind.Utc);

            return await _context.ChargingSessions
                .Where(s =>
                    s.StartTime >= fromUtc &&
                    s.StartTime <= toUtc)
                .GroupBy(s => s.StartTime.Date)
                .Select(g => new SessionTrendDto
                {
                    Date = g.Key,
                    Count = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToListAsync();
        }

        public async Task<List<EnergyTrendDto>> GetEnergyTrend(DateTime from, DateTime to)
        {
            var fromUtc = DateTime.SpecifyKind(from, DateTimeKind.Utc);
            var toUtc = DateTime.SpecifyKind(to, DateTimeKind.Utc);

            return await _context.ChargingSessions
                .Where(s =>
                    s.StartTime >= fromUtc &&
                    s.StartTime <= toUtc &&
                    s.EnergyConsumedKwh != null)
                .GroupBy(s => s.StartTime.Date)
                .Select(g => new EnergyTrendDto
                {
                    Date = g.Key,
                    EnergyMwh = Math.Round(
                        g.Sum(x => x.EnergyConsumedKwh.Value) / 1000, 2
                    )
                })
                .OrderBy(x => x.Date)
                .ToListAsync();
        }

        public async Task<List<CostTrendDto>> GetCostTrend(DateTime from, DateTime to)
        {
            const decimal tariffPerKwh = 10;

            var fromUtc = DateTime.SpecifyKind(from, DateTimeKind.Utc);
            var toUtc = DateTime.SpecifyKind(to, DateTimeKind.Utc);

            return await _context.ChargingSessions
                .Where(s =>
                    s.StartTime >= fromUtc &&
                    s.StartTime <= toUtc &&
                    s.EnergyConsumedKwh != null
                )
                .GroupBy(s => s.StartTime.Date)
                .Select(g => new CostTrendDto
                {
                    Date = g.Key,
                    Cost = Math.Round(
                        g.Sum(x => x.EnergyConsumedKwh.Value * tariffPerKwh), 2
                    )
                })
                .OrderBy(x => x.Date)
                .ToListAsync();
        }


        public async Task<List<Co2TrendDto>> GetCo2Trend(DateTime from, DateTime to)
        {
            const decimal factor = 0.7m;

            var fromUtc = DateTime.SpecifyKind(from, DateTimeKind.Utc);
            var toUtc = DateTime.SpecifyKind(to, DateTimeKind.Utc);

            return await _context.ChargingSessions
                .Where(s =>
                    s.StartTime >= fromUtc &&
                    s.StartTime <= toUtc &&
                    s.EnergyConsumedKwh != null)
                .GroupBy(s => s.StartTime.Date)
                .Select(g => new Co2TrendDto
                {
                    Date = g.Key,
                    Co2Tonnes = Math.Round(
                        (g.Sum(x => x.EnergyConsumedKwh.Value) / 1000) * factor, 2)
                })
                .OrderBy(x => x.Date)
                .ToListAsync();
        }


        public async Task<List<ChargerStatusDto>> GetChargerStatusDistribution()
        {
            return await _context.Chargers
                .GroupBy(c => c.Status)
                .Select(g => new ChargerStatusDto
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();
        }

        public async Task<int> GetLiveSessionsCount()
        {
            return await _context.ChargingSessions
                .CountAsync(s => s.Status == SessionStatus.Active);
        }



    }
}
