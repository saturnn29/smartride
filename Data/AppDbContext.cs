using Microsoft.EntityFrameworkCore;
using SmartRide.Models;

namespace SmartRide.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { } 
        public DbSet<PaymentMethods> PaymentMethods { get; set; }
        public DbSet<UserRegistration> Users { get; set; }
        public DbSet<DriverRegistration> Drivers { get; set; }
    }
}
