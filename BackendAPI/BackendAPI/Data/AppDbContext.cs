using BackendAPI.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Charger> Chargers { get; set; }
        public DbSet<ChargingSession> ChargingSessions { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Fault> Faults { get; set; }
        public DbSet<LogEntry> Logs { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<Supervisor> Supervisors { get; set; }
        public DbSet<SupportTicket> SupportTickets { get; set; }
        public DbSet<TicketScreenshot> TicketScreenshots { get; set; }
        public DbSet<Reservation> Reservations { get; set; }


    }

}
