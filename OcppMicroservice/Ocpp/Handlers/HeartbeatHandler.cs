using OcppMicroservice.State;
using System.Text.Json;

namespace OcppMicroservice.Ocpp.Handlers
{
    public static class HeartbeatHandler
    {
        public static Task Handle(
            JsonElement payload,
            string chargePointId)
        {
            HeartbeatStore.Update(chargePointId);
            return Task.CompletedTask;
        }
    }
}
