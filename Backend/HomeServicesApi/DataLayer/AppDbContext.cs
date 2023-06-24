using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace DataLayer
{
    public class AppDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer("Data Source=.;Initial Catalog=HomeServices_db;Integrated Security=True;TrustServerCertificate=True")
                .LogTo(Console.WriteLine);
        }

      
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>().Navigation(c => c.User).AutoInclude();
            modelBuilder.Entity<Customer>().Navigation(c => c.Address).AutoInclude();

            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique(true);
            base.OnModelCreating(modelBuilder);
        }
        
        public DbSet<Admin> Admins { get; set; }
        public DbSet<ServiceType> ServiceTypes { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Provider> Providers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public  DbSet<Customer> Customers { get; set; }


    }
}
