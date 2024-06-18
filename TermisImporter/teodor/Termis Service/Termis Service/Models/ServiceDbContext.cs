using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Termis_Service.Models
{
    public class ServiceDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            ServiceSettings serviceSettings = (ServiceSettings)ConfigurationManager.GetSection("serviceSettings");
            string databaseName = serviceSettings.DatabaseName;

            optionsBuilder.UseSqlServer($"Server=(localdb)\\MSSQLLocalDB;Integrated Security=True;Database={databaseName}");
        }

        public DbSet<Master> Masters { get; set; }
        public DbSet<Detail> Details { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Detail>()
                .Property(x => x.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Master>()
                .Property(x => x.Id)
                .ValueGeneratedOnAdd();
        }
    }
}
