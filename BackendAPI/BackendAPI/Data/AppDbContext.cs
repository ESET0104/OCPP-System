using BackendAPI.Data.Entities;
using BackendAPI.Data.Entities.Users;
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

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<Supervisor> Supervisors { get; set; }
        public DbSet<SupportTicket> SupportTickets { get; set; }
        public DbSet<TicketScreenshot> TicketScreenshots { get; set; }
        public DbSet<Reservation> Reservations { get; set; }


        public DbSet<Driver> Drivers { get; set; } = default!;

        public DbSet<Vehicle> Vehicles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            modelBuilder.Entity<Admin>()
                .HasIndex(a => a.Email)
                .IsUnique();

            modelBuilder.Entity<Admin>()
                .HasIndex(a => a.Username)
                .IsUnique();

            
            modelBuilder.Entity<Manager>()
                .HasIndex(m => m.Email)
                .IsUnique();

            modelBuilder.Entity<Manager>()
                .HasIndex(m => m.Username)
                .IsUnique();

            modelBuilder.Entity<Supervisor>()
                .HasIndex(s => s.Email)
                .IsUnique();

            modelBuilder.Entity<Supervisor>()
                .HasIndex(s => s.Username)
                .IsUnique();

            modelBuilder.Entity<Driver>()
                .ToTable("Drivers")
                .HasIndex(d => d.Email)
                .IsUnique();

            modelBuilder.Entity<Driver>()
                .Property(d => d.Status)
                .HasDefaultValue("ACTIVE");
        }
    }
    
}
