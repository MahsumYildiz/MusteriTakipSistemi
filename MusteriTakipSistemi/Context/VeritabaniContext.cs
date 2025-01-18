using Microsoft.EntityFrameworkCore;
using MusteriTakipSistemi.Models;
using MusteriTakipSistemi.Models.Configurations;

namespace MusteriTakipSistemi.Context
{
    public class VeritabaniContext : DbContext
    {
        public VeritabaniContext(DbContextOptions<VeritabaniContext> options) : base(options) { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Call> Calls { get; set; }
        public DbSet<Admin> Admins { get; set; }

        // OnModelCreating metodunu ekliyoruz
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // CustomerConfiguration sınıfını ekliyoruz
            modelBuilder.ApplyConfiguration(new CustomerConfiguration());
        }
    }
}
