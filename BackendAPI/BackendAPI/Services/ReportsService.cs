using BackendAPI.Data;
using BackendAPI.Data.Entities;
using BackendAPI.DTO.Reports;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Services
{
    public class ReportsService
    {
        private readonly AppDbContext _db;
        private const double PRICE_PER_KWH = 0.50;
        private const double CO2_PER_KWH = 0.40;

        public ReportsService(AppDbContext db)
        {
            _db = db;
        }

        private IQueryable<ChargingSession> FilterSessions(DateTime start, DateTime end, string? locationId)
        {
            return _db.ChargingSessions
                .Include(s => s.Charger)
                .ThenInclude(c => c.Location)
                .Where(s =>
                    s.Status == "Completed" &&
                    s.EndTime != null &&
                    s.StartTime >= start &&
                    s.EndTime <= end &&
                    (locationId == null || s.Charger.LocationId == locationId));
        }

        public async Task<AnalyticsSummaryDto> GetAnalytics(DateTime start, DateTime end, string? locationId)
        {
            start = DateTime.SpecifyKind(start, DateTimeKind.Utc);
            end = DateTime.SpecifyKind(end, DateTimeKind.Utc);

            var sessions = await FilterSessions(start, end, locationId).ToListAsync();
            if (!sessions.Any()) return new AnalyticsSummaryDto();

            var totalMinutes = sessions.Sum(s =>
                (s.EndTime!.Value - s.StartTime).TotalMinutes);

            var totalEnergy = sessions.Sum(s => (double)(s.EnergyConsumedKwh ?? 0));

            return new AnalyticsSummaryDto
            {
                TotalSessions = sessions.Count,
                AvgDurationMinutes = sessions.Average(s =>
                    (s.EndTime!.Value - s.StartTime).TotalMinutes),
                UtilizationPercent = (totalMinutes / (24 * 60)) * 100,

                AvgSOCStart = sessions.Average(s => s.InitialCharge),
                AvgSOCEnd = sessions.Average(s => s.SOC),

                TotalEnergyKwh = totalEnergy,
                AvgEnergyPerSession = totalEnergy / sessions.Count,

                TotalCost = totalEnergy * PRICE_PER_KWH,
                AvgCostPerSession = (totalEnergy * PRICE_PER_KWH) / sessions.Count,
                CostPerKwh = PRICE_PER_KWH,

                Co2SavedKg = totalEnergy * CO2_PER_KWH
            };
        }

        public async Task<List<ReportRowDto>> GetSummary(DateTime start, DateTime end, string? locationId)
        {
            var sessions = await FilterSessions(start, end, locationId).ToListAsync();

            return sessions
                .GroupBy(s => s.ChargerId)
                .Select(g =>
                {
                    var totalMinutes = g.Sum(s =>
                        (s.EndTime!.Value - s.StartTime).TotalMinutes);

                    var energy = g.Sum(s => (double)(s.EnergyConsumedKwh ?? 0));

                    return new ReportRowDto
                    {
                        LocationName = g.First().Charger.Location.Name,
                        ChargerId = g.Key,
                        TotalSessions = g.Count(),
                        EnergyKwh = energy,
                        ChargingDurationMinutes = totalMinutes,
                        UtilizationPercent = (totalMinutes / (24 * 60)) * 100,
                        UptimePercent = 98,
                        DowntimePercent = 2,
                        Co2SavedKg = energy * CO2_PER_KWH,
                        Cost = energy * PRICE_PER_KWH
                    };
                })
                .ToList();
        }

        public async Task<List<PeakLoadDto>> GetPeakLoad(DateTime start, DateTime end)
        {
            var sessions = await _db.ChargingSessions
                .Where(s => s.StartTime >= start && s.EndTime <= end)
                .ToListAsync();

            return sessions
                .GroupBy(s => s.StartTime.Hour)
                .Select(g => new PeakLoadDto
                {
                    Hour = g.Key,
                    EnergyKwh = g.Sum(s => (double)(s.EnergyConsumedKwh ?? 0))
                })
                .OrderByDescending(x => x.EnergyKwh)
                .ToList();
        }

        public async Task<List<QueueTimeDto>> GetQueueTime(DateTime start, DateTime end)
        {
            var sessions = await _db.ChargingSessions
                .Include(s => s.Charger).ThenInclude(c => c.Location)
                .Where(s => s.StartTime >= start && s.EndTime <= end)
                .ToListAsync();

            return sessions
                .GroupBy(s => s.ChargerId)
                .Select(g =>
                {
                    var location = g.First().Charger.Location.Name;
                    var hours = (end - start).TotalHours;
                    var throughput = hours > 0 ? g.Count() / hours : 0;

                    var avgDuration = g.Average(s =>
                        (s.EndTime!.Value - s.StartTime).TotalMinutes);

                    double avgQueueLength = Math.Max(0, (g.Count() - 1) / 2.0);
                    double maxQueueLength = Math.Max(0, g.Count() - 1);

                    return new QueueTimeDto
                    {
                        LocationName = location,
                        ChargerId = g.Key,
                        AvgWaitMinutes = avgQueueLength * avgDuration,
                        MaxWaitMinutes = maxQueueLength * avgDuration,
                        AvgQueueLength = avgQueueLength,
                        ThroughputPerHour = throughput
                    };
                })
                .ToList();
        }
    }
}
