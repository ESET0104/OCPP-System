using OcppMicroservice.State;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace OcppMicroservice.Ocpp.Handlers
{
    
    public static class BootNotificationHandler
    {
        public static async Task Handle(
            string chargePointId,
            string messageId,
            WebSocket socket)
        {
            HeartbeatStore.Update(chargePointId);
            var response = new object[]
            {
                3,
                messageId,
                new
                {
                    status = "Accepted",
                    currentTime = DateTime.UtcNow,
                    interval = 5
                }
            };

            var json = JsonSerializer.Serialize(response);
            var bytes = Encoding.UTF8.GetBytes(json);

            await socket.SendAsync(
                bytes,
                WebSocketMessageType.Text,
                true,
                CancellationToken.None
            );

            
        }
    }

}
