using Microsoft.EntityFrameworkCore;
using SmartRide.Controllers;
using SmartRide.Models;

namespace SmartRide.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { } 
        public DbSet<PaymentMethods> PaymentMethods { get; set; }
        public DbSet<Passengers> Passengers { get; set; }
        public DbSet<Invoices> Invoices { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<RideRequest> RideRequests { get; set; }
        public DbSet<Rides> Rides { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PaymentMethods>()
                .Property(p => p.PaymentType)
                .HasConversion<string>();
        }
    }
}
