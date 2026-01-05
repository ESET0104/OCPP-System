using OcppMicroservice.Messaging;
using OcppMicroservice.Ocpp;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
namespace OcppMicroservice.Ocpp.Handlers
{
    public static class AuthorizeHandler
    {
        public static async Task Handle(
            string messageId,
        JsonElement payload,
        string chargePointId,
        WebSocket socket)
        {
            var vin = payload
       .GetProperty("idToken")
       .GetProperty("value")
       .GetString();

            Console.WriteLine($"VIN authorization request: {vin}");

            //
            VinAuthorizationStore.Register(messageId, socket);
            //

            await RabbitMqEventPublisher.PublishAsync(
                "vin.authorization.request",
                new
                {
                    MessageId = messageId,
                    ChargerId = chargePointId,
                    Vin = vin,
                    Timestamp = DateTime.UtcNow
                }
            );


    //        var accepted = await VinAuthorizationStore.Wait(messageId);

    //        var responsePayload = new
    //        {
    //            idTokenInfo = new
    //            {
    //                status = accepted ? "Accepted" : "Rejected"
    //            }
    //        };

    //        var json = OcppMessage.CreateCallResult(messageId, responsePayload);
    //        var bytes = Encoding.UTF8.GetBytes(json);

    //        await socket.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
