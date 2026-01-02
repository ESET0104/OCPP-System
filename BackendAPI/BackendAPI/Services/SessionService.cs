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

 
        public async Task<string> StartSessionAsync(
            string chargerId,
            string driverId)
        {
            var charger = await _db.Chargers
                .FirstOrDefaultAsync(c => c.Id == chargerId);

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
                "Starting session for Charger={ChargerId}, DriverId={DriverId}",
                charger.Id,
                driver.Id
            );

            var session = new ChargingSession
            {
                Id = Nanoid.Generate(size: 10),
                ChargerId = charger.Id,
                DriverId = driver.Id,
                Status = SessionStatus.Pending
            };

            _db.ChargingSessions.Add(session);
            charger.Status = ChargerStatus.Preparing;

            await SaveLogAsync(
                source: "backend-api",
                eventType: "SESSION_REQUESTED",
                message: "Start charging session requested",
                chargerId: charger.Id,
                sessionId: session.Id,
                driverId: driver.Id
            );

            await _db.SaveChangesAsync();

            await _commandService.SendStartChargingCommand(
                charger.Id,
                session.Id,
                driver.Id
            );

            return session.Id;
        }


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

            var charger = await _db.Chargers
                .FirstOrDefaultAsync(c => c.Id == evt.ChargerId);

            if (charger != null)
                charger.Status = ChargerStatus.Engaged;

            await SaveLogAsync(
                source: "charger",
                eventType: "SESSION_STARTED",
                message: "Charging session started",
                chargerId: evt.ChargerId,
                sessionId: evt.SessionId,
                driverId: session.DriverId
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

            if (session.LastMeterUpdate != null &&
                evt.Timestamp <= session.LastMeterUpdate)
                return;

            session.LastMeterUpdate = evt.Timestamp;
            session.EnergyConsumedKwh = evt.EnergyKwh;

            await SaveLogAsync(
                source: "charger",
                eventType: "METER_VALUE",
                message: $"Energy consumed updated: {evt.EnergyKwh} kWh",
                chargerId: evt.ChargerId,
                sessionId: evt.SessionId,
                driverId: session.DriverId
            );

            await _db.SaveChangesAsync();

            _logger.LogInformation(
                "MeterValue received: Session={SessionId}, Energy={Energy} kWh",
                evt.SessionId,
                evt.EnergyKwh
            );
        }


        public async Task StopSessionAsync(string sessionId)
        {
            var session = await _db.ChargingSessions
                .FirstOrDefaultAsync(s => s.Id == sessionId);

            if (session == null)
                throw new Exception("Session not found");

            if (session.Status != SessionStatus.Active)
                throw new Exception("Session not active");

            session.Status = SessionStatus.Stopping;

            await SaveLogAsync(
                source: "backend-api",
                eventType: "SESSION_STOP_REQUESTED",
                message: "Stop charging session requested",
                chargerId: session.ChargerId,
                sessionId: session.Id,
                driverId: session.DriverId
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
                _logger.LogWarning(
                    "Stop event due to fault for Session={SessionId}",
                    evt.SessionId
                );
                return;
            }

            session.Status = SessionStatus.Completed;
            session.EndTime = evt.StopTime;

            var charger = await _db.Chargers
                .FirstOrDefaultAsync(c => c.Id == evt.ChargerId);

            if (charger != null)
                charger.Status = ChargerStatus.Available;

            await SaveLogAsync(
                source: "charger",
                eventType: "SESSION_ENDED",
                message: "Charging session completed",
                chargerId: evt.ChargerId,
                sessionId: evt.SessionId,
                driverId: session.DriverId
            );

            await _db.SaveChangesAsync();
        }


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
                    (s.Status == SessionStatus.Active ||
                     s.Status == SessionStatus.Pending ||
                     s.Status == SessionStatus.Stopping));

            if (activeSession != null)
            {
                activeSession.Status = SessionStatus.Faulted;
                activeSession.EndTime = evt.Timestamp;
            }

            await SaveLogAsync(
                source: "ocpp",
                eventType: "CHARGER_FAULTED",
                message: evt.FaultCode,
                chargerId: evt.ChargerId,
                sessionId: activeSession?.Id,
                driverId: activeSession?.DriverId
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
                source: "ocpp",
                eventType: "CHARGER_RECOVERED",
                message: "Charger recovered",
                chargerId: evt.ChargerId
            );

            await _db.SaveChangesAsync();
        }


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
