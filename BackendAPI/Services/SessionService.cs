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

            if (session.Status == SessionStatus.Completed)
                return;

            if (session.ChargerId != evt.ChargerId)
                return;

            session.Status = SessionStatus.Completed;
            session.EndTime = evt.StopTime;
            session.EnergyConsumedKwh = evt.EnergyConsumedKwh;

            var charger = await _db.Chargers
                .FirstOrDefaultAsync(c => c.Id == evt.ChargerId);

            if (charger != null)
                charger.Status = ChargerStatus.Available;

            await _db.SaveChangesAsync();
            _logger.LogInformation(
                "Session stopped: Session={SessionId}, FinalEnergy={Energy}",
                evt.SessionId,
                evt.EnergyConsumedKwh
            );
        }



    }

}
