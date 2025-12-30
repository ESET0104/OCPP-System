using BackendAPI.RabbitMq.Contracts;
using BackendAPI.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace BackendApi.RabbitMq
{
    public class Consumer : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<Consumer> _logger;
        private readonly ConnectionFactory _factory;

        private IConnection _connection;
        private IChannel _channel;

        private const string ExchangeName = "charging_events_ex";
        private const string QueueName = "backend_events_q";

        public Consumer(
            IServiceScopeFactory scopeFactory,
            IConfiguration config,
            ILogger<Consumer> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;

            _factory = new ConnectionFactory
            {
                HostName = config["RabbitMq:HostName"],
                VirtualHost = config["RabbitMq:VirtualHost"],
                UserName = config["RabbitMq:UserName"],
                Password = config["RabbitMq:Password"]
            };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _connection = await _factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.ExchangeDeclareAsync(
                exchange: ExchangeName,
                type: ExchangeType.Direct,
                durable: true
            );

            await _channel.QueueDeclareAsync(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false
            );

            // Bind all relevant event types
            await _channel.QueueBindAsync(QueueName, ExchangeName, "event.session.started");
            await _channel.QueueBindAsync(QueueName, ExchangeName, "event.session.stopped");
            await _channel.QueueBindAsync(QueueName, ExchangeName, "event.meter.value");
            await _channel.QueueBindAsync(QueueName, ExchangeName, "event.charger.faulted");
            await _channel.QueueBindAsync(QueueName, ExchangeName, "event.charger.recovered");

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (sender, args) =>
            {
                var json = Encoding.UTF8.GetString(args.Body.ToArray());

                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var sessionService = scope.ServiceProvider.GetRequiredService<SessionService>();

                    switch (args.RoutingKey)
                    {
                        case "event.session.started":
                            var started = JsonSerializer.Deserialize<SessionStartEvent>(json);
                            await sessionService.HandleSessionStarted(started);
                            break;

                        case "event.session.stopped":
                            var stopped = JsonSerializer.Deserialize<SessionStopEvent>(json);
                            await sessionService.HandleSessionStopped(stopped);
                            break;

                        case "event.meter.value":
                            var meter = JsonSerializer.Deserialize<MeterValueEvent>(json);
                            await sessionService.HandleMeterValue(meter);
                            break;
                        case "event.charger.faulted":
                            var fault = JsonSerializer.Deserialize<ChargerFaultEvent>(json);
                            await sessionService.HandleChargerFault(fault);
                            break;
                        case "event.charger.recovered":
                            var result = JsonSerializer.Deserialize<ChargerRecoverEvent>(json);
                            await sessionService.HandleChargerRecovered(result);
                            break;
                    }

                    await _channel.BasicAckAsync(args.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing event {RoutingKey}", args.RoutingKey);
                    await _channel.BasicNackAsync(args.DeliveryTag, false, true);
                }
            };

            await _channel.BasicConsumeAsync(
                queue: QueueName,
                autoAck: false,
                consumer: consumer
            );

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        public override void Dispose()
        {
            _channel?.CloseAsync();
            _connection?.CloseAsync();
            base.Dispose();
        }
    }
}
