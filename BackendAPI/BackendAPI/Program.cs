using BackendApi.RabbitMq;
using BackendAPI.Data;
using BackendAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddControllers();
builder.Services.AddScoped<CommandService>();
builder.Services.AddScoped<ChargerService>();
builder.Services.AddScoped<SessionService>();
builder.Services.AddScoped<DriverService>();


builder.Services.AddSingleton<Publisher>();
builder.Services.AddHostedService<Consumer>();



builder.Services.AddOpenApi();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
