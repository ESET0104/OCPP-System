using BackendAPI.Data;
using BackendAPI.Data.Entities;
using BackendAPI.RabbitMq.Contracts;
using Microsoft.EntityFrameworkCore;
using NanoidDotNet;

namespace BackendAPI.Services
{
    public class SessionService
    {
        private readonly AppDbContext _db;
        private readonly CommandService _commandService;
        private readonly ILogger<SessionService> _logger;

        public SessionService(
            AppDbContext db,
            CommandService commandService,
            ILogger<SessionService> logger)
        {
            _db = db;
            _commandService = commandService;
            _logger = logger;
        }

        public async Task<IEnumerable<ChargingSession>> GetAllSessions()
        {
            return await _db.ChargingSessions.ToListAsync();
        }

        public async Task<IEnumerable<ChargingSession>> GetSessionsByDriver(string driverId)
        {
            return await _db.ChargingSessions
                .Where(s => s.DriverId == driverId)
                .ToListAsync();
        }

        public async Task<IEnumerable<ChargingSession>> GetSessionsByCharger(string chargerId)
        {
            return await _db.ChargingSessions
                .Where(s => s.ChargerId == chargerId)
                .ToListAsync();
        }

        public async Task<object> GetSessionInfo(string sessionId)
        {
            var session = await _db.ChargingSessions
                .FirstOrDefaultAsync(s => s.Id == sessionId);

            if (session == null)
                throw new Exception("Session not found");

            return new
            {
                SessionId = session.Id,
                Status = session.Status,
                InitialCharge = session.InitialCharge,
                FinalCharge = session.SOC,
                EnergyConsumed = session.EnergyConsumedKwh,
                StartTime = session.StartTime,
                EndTime = session.EndTime
            };
        }

        public async Task<IEnumerable<ChargingSession>> GetLiveSessions()
        {
            return await _db.ChargingSessions
                .Where(s => s.Status == SessionStatus.Active)
                .ToListAsync();
        }

        // ------------------- START SESSION -------------------

        public async Task<string> StartSessionAsync(string chargerId, string driverId)
        {
            using var tx = await _db.Database.BeginTransactionAsync();

            var charger = await _db.Chargers
                .FirstOrDefaultAsync(c => c.Id == chargerId);

            if (charger == null)
                throw new Exception("Charger not found");

            //if (charger.Status != ChargerStatus.Preparing)
            //    throw new Exception("vehicle not authorized or charger not ready");

            if (charger == null || charger.Status != ChargerStatus.Available)
                throw new Exception("Charger not available");

            var driver = await _db.Drivers
                .FirstOrDefaultAsync(d => d.Id == driverId);

            if (driver == null)
                throw new Exception("Driver not authorized");

            var existingSession = await _db.ChargingSessions
                .FirstOrDefaultAsync(s =>
                    s.ChargerId == charger.Id &&
                    s.Status == SessionStatus.Active);

            if (existingSession != null)
                throw new Exception("Charger already in use");

            _logger.LogInformation(
                "Starting session for Charger={ChargerId}, Driver={DriverId}",
                charger.Id,
                driver.Id
            );

            var session = new ChargingSession
            {
                Id = Nanoid.Generate(size: 10),
                ChargerId = charger.Id,
                DriverId = driver.Id,
                Status = SessionStatus.Pending,
                StartTime = DateTime.UtcNow
            };

            _db.ChargingSessions.Add(session);

            // Reserve charger
            charger.Status = ChargerStatus.Preparing;

            await SaveLogAsync(
                "backend-api",
                "SESSION_REQUESTED",
                "Start charging session requested",
                charger.Id,
                session.Id,
                driver.Id
            );

            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            await _commandService.SendStartChargingCommand(
                charger.Id,
                session.Id,
                driver.Id
            );

            return session.Id;
        }

        // ------------------- SESSION EVENTS -------------------

        public async Task HandleSessionStarted(SessionStartEvent evt)
        {
            var session = await _db.ChargingSessions
                .FirstOrDefaultAsync(s => s.Id == evt.SessionId);

            if (session == null || session.Status != SessionStatus.Pending)
                return;

            if (session.ChargerId != evt.ChargerId)
            {
                session.Status = SessionStatus.Faulted;
                await _db.SaveChangesAsync();
                return;
            }

            session.Status = SessionStatus.Active;
            session.StartTime = evt.StartTime;
            session.SOC = evt.SOC;

            var charger = await _db.Chargers
                .FirstOrDefaultAsync(c => c.Id == evt.ChargerId);

            if (charger != null)
                charger.Status = ChargerStatus.Engaged;

            await SaveLogAsync(
                "charger",
                "SESSION_STARTED",
                "Charging session started",
                evt.ChargerId,
                evt.SessionId,
                session.DriverId
            );

            await _db.SaveChangesAsync();
        }

        public async Task HandleMeterValue(MeterValueEvent evt)
        {
            var session = await _db.ChargingSessions
                .FirstOrDefaultAsync(s => s.Id == evt.SessionId);

            if (session == null ||
                session.Status != SessionStatus.Active ||
                session.ChargerId != evt.ChargerId)
                return;

            if (session.LastMeterUpdate != default &&
                evt.Timestamp <= session.LastMeterUpdate)
                return;


            session.LastMeterUpdate = evt.Timestamp;
            session.EnergyConsumedKwh = evt.EnergyKwh;
            session.SOC = evt.SOC;

            await SaveLogAsync(
                "charger",
                "METER_VALUE",
                $"Energy updated: {evt.EnergyKwh} kWh",
                evt.ChargerId,
                evt.SessionId,
                session.DriverId
            );

            await _db.SaveChangesAsync();

            _logger.LogInformation(
                "MeterValue: Session={SessionId}, Energy={Energy}",
                evt.SessionId,
                evt.EnergyKwh
            );
        }

        // ------------------- STOP SESSION -------------------

        public async Task StopSessionAsync(string sessionId)
        {
            var session = await _db.ChargingSessions
                .FirstOrDefaultAsync(s => s.Id == sessionId);

            if (session == null)
                throw new Exception("Session not found");

            if (session.Status != SessionStatus.Active)
                throw new Exception("Session not active");

            session.Status = SessionStatus.Stopping;

            var charger = await _db.Chargers
                .FirstOrDefaultAsync(c => c.Id == session.ChargerId);

            if (charger != null)
                charger.Status = ChargerStatus.Preparing;

            await SaveLogAsync(
                "backend-api",
                "SESSION_STOP_REQUESTED",
                "Stop charging session requested",
                session.ChargerId,
                session.Id,
                session.DriverId
            );

            await _db.SaveChangesAsync();

            await _commandService.SendStopChargingCommand(
                session.ChargerId,
                session.Id
            );
        }

        public async Task HandleSessionStopped(SessionStopEvent evt)
        {
            var session = await _db.ChargingSessions
                .FirstOrDefaultAsync(s => s.Id == evt.SessionId);

            if (session == null ||
                session.Status == SessionStatus.Completed ||
                session.ChargerId != evt.ChargerId)
                return;

            if (evt.triggerReason == "Fault")
            {
                session.Status = SessionStatus.Faulted;
                session.EndTime = evt.StopTime;
                await _db.SaveChangesAsync();
                return;
            }

            session.Status = SessionStatus.Completed;
            session.EndTime = evt.StopTime;

            var charger = await _db.Chargers
                .FirstOrDefaultAsync(c => c.Id == evt.ChargerId);

            if (charger != null)
                charger.Status = ChargerStatus.Available;

            await SaveLogAsync(
                "charger",
                "SESSION_ENDED",
                "Charging session completed",
                evt.ChargerId,
                evt.SessionId,
                session.DriverId
            );

            await _db.SaveChangesAsync();
        }

        // ------------------- FAULT HANDLING -------------------

        public async Task HandleChargerFault(ChargerFaultEvent evt)
        {
            var fault = new Fault
            {
                Id = Nanoid.Generate(size: 10),
                ChargerId = evt.ChargerId,
                FaultCode = evt.FaultCode,
                Timestamp = evt.Timestamp
            };

            _db.Faults.Add(fault);

            var charger = await _db.Chargers
                .FirstOrDefaultAsync(c => c.Id == evt.ChargerId);

            if (charger != null)
            {
                charger.Status = ChargerStatus.Faulted;
                charger.LastSeen = DateTime.UtcNow;
            }

            var activeSession = await _db.ChargingSessions
                .FirstOrDefaultAsync(s =>
                    s.ChargerId == evt.ChargerId &&
                    s.Status != SessionStatus.Completed);

            if (activeSession != null)
            {
                activeSession.Status = SessionStatus.Faulted;
                activeSession.EndTime = evt.Timestamp;
            }

            await SaveLogAsync(
                "ocpp",
                "CHARGER_FAULTED",
                evt.FaultCode,
                evt.ChargerId,
                activeSession?.Id,
                activeSession?.DriverId
            );

            await _db.SaveChangesAsync();
        }

        public async Task HandleChargerRecovered(ChargerRecoverEvent evt)
        {
            var charger = await _db.Chargers
                .FirstOrDefaultAsync(c => c.Id == evt.ChargerId);

            if (charger != null)
            {
                charger.Status = ChargerStatus.Available;
                charger.LastSeen = evt.Timestamp;
            }

            await SaveLogAsync(
                "ocpp",
                "CHARGER_RECOVERED",
                "Charger recovered",
                evt.ChargerId
            );

            await _db.SaveChangesAsync();
        }

        // ------------------- LOGGING -------------------

        private async Task SaveLogAsync(
            string source,
            string eventType,
            string message,
            string? chargerId = null,
            string? sessionId = null,
            string? driverId = null)
        {
            var log = new LogEntry
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow,
                Source = source,
                EventType = eventType,
                Message = message,
                ChargerId = chargerId,
                SessionId = sessionId,
                DriverId = driverId
            };

            _db.Logs.Add(log);
        }
    }
}
