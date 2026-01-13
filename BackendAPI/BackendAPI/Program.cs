using BackendApi.RabbitMq;
using BackendAPI.Data;
using BackendAPI.Notifications;
using BackendAPI.Repositories;
using BackendAPI.Services;
using BackendAPI.Services.UserServices;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;


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
//builder.Services.AddScoped<INotificationSender, AzureNotificationSender>();
builder.Services.AddScoped<INotificationSender, DummyNotificationSender>();

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



builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("AuthPolicy", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});


builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddScoped<CommandService>();
builder.Services.AddScoped<ChargerService>();
builder.Services.AddScoped<SessionService>();
builder.Services.AddScoped<DriverService>();
builder.Services.AddScoped<VehicleService>();

builder.Services.AddSingleton<PasswordHasher>();
builder.Services.AddScoped<DashboardService>();

builder.Services.AddScoped<LogsService>();


builder.Services.AddScoped<TicketService>();
builder.Services.AddScoped<ReservationService>();
builder.Services.AddScoped<ReportsService>();
builder.Services.AddSingleton<Publisher>();
builder.Services.AddHostedService<Consumer>();

builder.Services.AddScoped<NotificationService>();
builder.Services.AddHttpContextAccessor();





builder.Services.AddControllers();
builder.Services.AddOpenApi();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
