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

        public SessionService(AppDbContext db, CommandService commandService, ILogger<SessionService> logger)
        {
            _db = db;
            _commandService = commandService;
            _logger = logger;
        }

        public async Task<string> StartSessionAsync(
            string chargerId,
            string userId)
        {
            var charger = await _db.Chargers
                .FirstOrDefaultAsync(c => c.Id == chargerId);
            if(charger == null || charger.Status != ChargerStatus.Available)
            {
                throw new Exception("charger not available");
            }

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if(user == null)
            {
                throw new Exception("user not Authorized");
            }

            var existingSession = await _db.ChargingSessions
            .FirstOrDefaultAsync(s =>
                s.ChargerId == charger.Id &&
                s.Status == SessionStatus.Active);

            if (existingSession != null)
                throw new Exception("Charger already in use");

            var session = new ChargingSession
            {
                Id = Nanoid.Generate(size:10),
                ChargerId = charger.Id,
                UserId = userId,
                //StartTime = DateTime.UtcNow,
                Status = SessionStatus.Pending,

            };

            _db.ChargingSessions.Add(session);
            charger.Status = ChargerStatus.Preparing;

            await SaveLogAsync(
                source: "backend-api",
                eventType: "SESSION_REQUESTED",
                message: "Start charging session requested",
                chargerId: chargerId,
                sessionId: session.Id,
                userId: userId
            );

            await _db.SaveChangesAsync();

            await _commandService.SendStartChargingCommand(
            charger.Id,
            session.Id,
            userId
        );

            return session.Id;
        }

        public async Task HandleSessionStarted(SessionStartEvent evt)
        {
            var session = await _db.ChargingSessions
                .FirstOrDefaultAsync(s => s.Id == evt.SessionId);

            if (session == null)
            {
                return;
            }
            if (session.Status != SessionStatus.Pending)
            {
                return;
            }
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
            {
                charger.Status = ChargerStatus.Engaged;
            }

            await SaveLogAsync(
                source: "charger",
                eventType: "SESSION_STARTED",
                message: "Charging session started",
                chargerId: evt.ChargerId,
                sessionId: evt.SessionId,
                userId: session.UserId
            );

            await _db.SaveChangesAsync();
        }

        public async Task HandleMeterValue(MeterValueEvent evt)
        {
            var session = await _db.ChargingSessions
                .FirstOrDefaultAsync(s => s.Id == evt.SessionId);

            if (session == null)
                return;

            if (session.Status != SessionStatus.Active)
                return;

            if (session.ChargerId != evt.ChargerId)
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
                userId: session.UserId
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
                userId: session.UserId
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

            if (session == null)
                return;

            if (evt.triggerReason == "Fault")
            {
                _logger.LogWarning(
                    "Stop event due to fault for Session={SessionId}, ignoring completion",
                    evt.SessionId
                );
                return;
            }

            if (session.Status == SessionStatus.Faulted)
                return;
            

            if (session.Status == SessionStatus.Completed)
                return;

            if (session.ChargerId != evt.ChargerId)
                return;

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
                userId: session.UserId
            );

            await _db.SaveChangesAsync();

            _logger.LogInformation(
                "Session stopped: Session={SessionId}, FinalEnergy={Energy}",
                evt.SessionId,
                evt.EnergyConsumedKwh
            );
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
                userId: activeSession?.UserId
            );

            await _db.SaveChangesAsync();

            _logger.LogWarning(
                "Charger faulted: Charger={ChargerId}, Code={FaultCode}",
                evt.ChargerId,
                evt.FaultCode
            );
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
                message: "charger is recovered",
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
            string? userId = null)
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
                UserId = userId
            };

            _db.Logs.Add(log);
        }

    }

}
