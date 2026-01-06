using BackendAPI.Data;
using BackendAPI.Repositories;
using BackendAPI.Services.UserServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DotNetEnv;
using BackendAPI.Services;
using BackendApi.RabbitMq;


var builder = WebApplication.CreateBuilder(args);

Env.Load();
builder.Configuration
    .AddEnvironmentVariables();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration["DB_CONNECTION"]
    )
);

builder.Services.AddScoped(typeof(IUserRepository<>), typeof(UserRepository<>));
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<ManagerService>();
builder.Services.AddScoped<SupervisorService>();

// JWT Auth
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization();
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






var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
