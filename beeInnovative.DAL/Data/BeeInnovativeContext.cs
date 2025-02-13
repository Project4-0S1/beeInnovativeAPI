using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using beeInnovative.DAL.Models;

namespace beeInnovative.DAL.Data
{
    public class BeeInnovativeContext : DbContext
    {
        public BeeInnovativeContext() { }

        public BeeInnovativeContext(DbContextOptions<BeeInnovativeContext> options) : base(options)
        { }

        public DbSet<Beehive> Beehives { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Hornet> Hornets { get; set; }
        public DbSet<HornetDetection> HornetDetections { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<NestLocation> NestLocation { get; set; }
        public DbSet<EstimatedNestLocation> EstimatedNestLocation { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserBeehive> UserBeehives { get; set; }
        public DbSet<Calculation> Calculations { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Beehive>().ToTable("Beehive");
            modelBuilder.Entity<Color>().ToTable("Color");
            modelBuilder.Entity<Hornet>().ToTable("Hornet");
            modelBuilder.Entity<HornetDetection>().ToTable("HornetDetection");
            modelBuilder.Entity<Status>().ToTable("Status");
            modelBuilder.Entity<NestLocation>().ToTable("NestLocation");
            modelBuilder.Entity<EstimatedNestLocation>().ToTable("EstimatedNestLocation");
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<UserBeehive>().ToTable("UserBeehive");
            modelBuilder.Entity<Calculation>().ToTable("Calculation");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=beeInnovative;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            }
        }
    }
}
