using BackendApi.RabbitMq;
using BackendAPI.DTO;

namespace BackendAPI.Services
{
    public class CommandService
    {
        private readonly Publisher _publisher;

        public CommandService(Publisher publisher)
        {
            _publisher = publisher;
        }

        public async Task SendStartChargingCommand(string chargerId,string sessionId,string userId)
        {
            var command = new StartChargingCommand
            {
                ChargerId = chargerId,
                SessionId = sessionId,
                UserId = userId,
                Timestamp = DateTime.UtcNow
            };

            await _publisher.PublishAsync("command.start", command);
        }

        public async Task SendStopChargingCommand(string chargerId,string sessionId)
        {
            var command = new StopChargingCommand
            {
                ChargerId = chargerId,
                SessionId = sessionId,
                Timestamp = DateTime.UtcNow
            };

            await _publisher.PublishAsync("command.stop", command);
        }
    }


}
