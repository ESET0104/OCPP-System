using System.Text.Json;
using OcppMicroservice.Messaging;
using OcppMicroservice.State;

namespace OcppMicroservice.Ocpp.Handlers
{
    public static class TransactionEventHandler
    {
        public static async Task Handle(
            JsonElement payload,
            string chargePointId)
        {
            var eventType = payload.GetProperty("eventType").GetString();
            var timestamp = payload.GetProperty("timestamp").GetDateTime();

            var transactionInfo = payload.GetProperty("transactionInfo");
            var sessionId = transactionInfo
                .GetProperty("sessionId")
                .GetString();

            var state = ChargerStateStore.Get(chargePointId);

            if (eventType == "Started")
            {
                state.ActiveSessionId = sessionId;

                await RabbitMqEventPublisher.PublishAsync(
                    "event.session.started",
                    new
                    {
                        SessionId = sessionId,
                        ChargerId = chargePointId,
                        StartTime = timestamp
                    });
            }
            else if (eventType == "Ended")
            {
                Console.WriteLine("stop event received at ocpp");

                await RabbitMqEventPublisher.PublishAsync(
                    "event.session.stopped",
                    new
                    {
                        SessionId = sessionId,
                        ChargerId = chargePointId,
                        StopTime = timestamp
                    });

                state.ActiveSessionId = null;
            }
        }
    }
}

