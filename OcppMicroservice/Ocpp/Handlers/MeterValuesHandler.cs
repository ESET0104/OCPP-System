using System.Text.Json;
using OcppMicroservice.Messaging;
using OcppMicroservice.State;

namespace OcppMicroservice.Ocpp.Handlers
{
    public static class MeterValuesHandler
    {
        public static async Task Handle(
            JsonElement payload,
            string chargePointId)
        {
            var state = ChargerStateStore.Get(chargePointId);
            if (state.ActiveSessionId == null)
                return;

            foreach (var meterValue in payload
                .GetProperty("meterValue")
                .EnumerateArray())
            {
                var timestamp = meterValue
                    .GetProperty("timestamp")
                    .GetDateTime();

                foreach (var sampledValue in meterValue
                    .GetProperty("sampledValue")
                    .EnumerateArray())
                {
                    var measurand = sampledValue
                        .GetProperty("measurand")
                        .GetString();

                    if (measurand != "Energy.Active.Import.Register")
                        continue;

                    var energyKwh = double.Parse(
                        sampledValue.GetProperty("value").GetString()!
                    );

                    await RabbitMqEventPublisher.PublishAsync(
                        "event.meter.value",
                        new
                        {
                            SessionId = state.ActiveSessionId,
                            ChargerId = chargePointId,
                            Timestamp = timestamp,
                            EnergyKwh = energyKwh
                        }
                    );
                }
            }
        }
    }
}