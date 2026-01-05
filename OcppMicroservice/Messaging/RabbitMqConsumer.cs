using System.Text;
using System.Text.Json;
using System.Net.WebSockets;
using OcppMicroservice.State;
using OcppMicroservice.WebSockets;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using OcppMicroservice.Ocpp;

namespace OcppMicroservice.Messaging
{
    public class RabbitMqConsumer
    {
        private readonly IChannel _channel;

        public RabbitMqConsumer(IChannel channel)
        {
            _channel = channel;
        }

        public void Start()
        {
            const string exchangeName = "charging_commands_ex";
            const string queueName = "charging.commands";

            _channel.ExchangeDeclareAsync(
                exchange: exchangeName,
                type: ExchangeType.Direct,
                durable: true
            );

            _channel.QueueDeclareAsync(
                queue: queueName,
                durable: false,
                exclusive: false,
                autoDelete: false
            );

            _channel.QueueBindAsync(
            queue: queueName,
            exchange: exchangeName,
            routingKey: "vin.authorization.result"
            );

            _channel.QueueBindAsync(
                queue: queueName,
                exchange: exchangeName,
                routingKey: "command.start"
            );

            _channel.QueueBindAsync(
                queue: queueName,
                exchange: exchangeName,
                routingKey: "command.stop"
            );

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += HandleMessage;

            _channel.BasicConsumeAsync(
                queue: queueName,
                autoAck: true,
                consumer: consumer
            );
        }

        private async Task HandleMessage(object sender, BasicDeliverEventArgs ea)
        {
            var json = Encoding.UTF8.GetString(ea.Body.ToArray());
            var root = JsonDocument.Parse(json).RootElement;

            var routingKey = ea.RoutingKey;

            if (routingKey == "vin.authorization.result")
            {
                var messageId = root.GetProperty("MessageId").GetString();
                var accepted = root.GetProperty("Accepted").GetBoolean();

                await VinAuthorizationStore.Resolve(messageId!, accepted);
                Console.WriteLine($"VIN AUTH RESULT: {messageId} is {(accepted ? "ACCEPTED" : "REJECTED")}");
                return;
            }

            var chargerId = root.GetProperty("ChargerId").GetString();
            var sessionId = root.GetProperty("SessionId").GetString();
            var userId = root.TryGetProperty("UserId", out var u)
                ? u.GetString()
                : null;

            if (chargerId == null || sessionId == null)
                return;

            var socket = ChargerConnectionManager.GetSocket(chargerId);
            if (socket == null || socket.State != WebSocketState.Open)
                return;

            switch (routingKey)
            {
                case "command.start":
                    await HandleStartCommand(socket, chargerId, sessionId, userId);
                    break;

                case "command.stop":
                    await HandleStopCommand(socket, chargerId, sessionId);
                    break;

                default:
                    Console.WriteLine($"Unknown routing key: {routingKey}");
                    break;
            }
        }


        private async Task HandleStartCommand(
            WebSocket socket,
            string chargerId,
            string sessionId,
            string? userId)
        {
            Console.WriteLine($"[START] Session={sessionId}");

            var state = ChargerStateStore.Get(chargerId);
            state.ActiveSessionId = sessionId;

            await SendOcppCommand(
                socket,
                "RequestStartTransaction",
                new
                {
                    sessionId,
                    userId
                });
        }


        private async Task HandleStopCommand(
            WebSocket socket,
            string chargerId,
            string sessionId)
        {
            Console.WriteLine($"[STOP] Session={sessionId}");

            var state = ChargerStateStore.Get(chargerId);
            state.ActiveSessionId = null;

            await SendOcppCommand(
                socket,
                "RequestStopTransaction",
                new
                {
                    sessionId
                });
        }

        private static async Task SendOcppCommand(
            WebSocket socket,
            string action,
            object payload)
        {
            var message = new object[]
            {
                2,
                Guid.NewGuid().ToString(),
                action,
                payload
            };

            var bytes = Encoding.UTF8.GetBytes(
                JsonSerializer.Serialize(message));

            await socket.SendAsync(
                bytes,
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }
    }
}
    


