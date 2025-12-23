using System.Net.WebSockets;
using System.Text;
using OcppMicroservice.Ocpp;
namespace OcppMicroservice.Ocpp.Handlers
{
    public static class AuthorizeHandler
    {
        public static async Task Handle(
            string messageId,
            WebSocket socket)
        {
            var responsePayload = new
            {
                idTokenInfo = new
                {
                    status = "Accepted"
                }
            };

            var json = OcppMessage.CreateCallResult(messageId, responsePayload);
            var bytes = Encoding.UTF8.GetBytes(json);

            await socket.SendAsync(
                bytes,
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }
    }
}
