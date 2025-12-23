using OcppMicroservice.Messaging;
using OcppMicroservice.WebSockets;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseWebSockets();

app.UseMiddleware<WebSocketMiddleware>();


_ = RabbitMqConnection.Channel;

var commandConsumer = new RabbitMqConsumer(RabbitMqConnection.Channel);
commandConsumer.Start();

app.Run();
