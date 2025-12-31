using BackendAPI.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Charger> Chargers { get; set; }
        public DbSet<ChargingSession> ChargingSessions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Fault> Faults { get; set; }
        public DbSet<LogEntry> Logs { get; set; }

        public DbSet<Driver> Drivers => Set<Driver>();
        public DbSet<Vehicle> Vehicles => Set<Vehicle>();


     




    }

}
