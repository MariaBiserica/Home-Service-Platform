﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<Class>().Property(e => e.Name).HasMaxLength(10);
        }
        //dbSets
    }
}
