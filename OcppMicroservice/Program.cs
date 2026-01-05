////using OcppMicroservice.Messaging;
////using OcppMicroservice.Watchdog;
////using OcppMicroservice.WebSockets;

////var builder = WebApplication.CreateBuilder(args);

////builder.Services.AddHostedService<ChargerWatchdog>();

////var app = builder.Build();

////app.UseWebSockets();

////app.UseMiddleware<WebSocketMiddleware>();



////_ = RabbitMqConnection.Channel;

////var commandConsumer = new RabbitMqConsumer(RabbitMqConnection.Channel);
////commandConsumer.Start();

////app.Run();


//using OcppMicroservice.Messaging;
//using OcppMicroservice.Watchdog;
//using OcppMicroservice.WebSockets;

//var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddHostedService<ChargerWatchdog>();


//RabbitMqConnection.Init(builder.Configuration);

//var app = builder.Build();

//app.UseWebSockets();

//// Expose /ws endpoint
//app.Map("/ws", wsApp =>
//{
//    wsApp.UseMiddleware<WebSocketMiddleware>();
//});

//var commandConsumer = new RabbitMqConsumer(RabbitMqConnection.Channel);
//commandConsumer.Start();

//app.Run();



using OcppMicroservice.Messaging;
using OcppMicroservice.Watchdog;
using OcppMicroservice.WebSockets;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<ChargerWatchdog>();

var app = builder.Build();

app.UseWebSockets();

app.Map("/ws", wsApp =>
{
    wsApp.UseMiddleware<WebSocketMiddleware>();
});

_ = RabbitMqConnection.Channel;

var commandConsumer = new RabbitMqConsumer(RabbitMqConnection.Channel);
commandConsumer.Start();

app.Run();
