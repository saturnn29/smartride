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
    }
}
