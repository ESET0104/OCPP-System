using BackendApi.RabbitMq;
using BackendAPI.Data;
using BackendAPI.Services;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

Env.Load();
builder.Configuration
    .AddEnvironmentVariables();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration["DB_CONNECTION"]
    )
);

builder.Services.AddControllers();
builder.Services.AddScoped<CommandService>();
builder.Services.AddScoped<ChargerService>();
builder.Services.AddScoped<SessionService>();
builder.Services.AddScoped<DriverService>();
builder.Services.AddScoped<VehicleService>();

builder.Services.AddScoped<TicketService>();
builder.Services.AddSingleton<Publisher>();
builder.Services.AddHostedService<Consumer>();
builder.Services.AddScoped<ReservationService>();




builder.Services.AddOpenApi();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
