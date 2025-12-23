using System.Text.Json;
using OcppMicroservice.Messaging;
namespace OcppMicroservice.Ocpp.Handlers
{
    public static class StatusNotificationHandler
    {
        public static void Handle(
            JsonElement payload,
            string chargePointId)
        {
            var connectorId = payload.GetProperty("connectorId").GetInt32();
            var status = payload.GetProperty("connectorStatus").GetString();

            RabbitMqEventPublisher.PublishAsync("connector.status", new
            {
                chargePointId,
                connectorId,
                status,
                timestamp = DateTime.UtcNow
            });
        }
    }
}
