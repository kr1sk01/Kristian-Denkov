using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace TermisImporter.Data
{
    public class ServiceDbContext : DbContext
    {
        public ServiceDbContext(string connectionString) : base(connectionString)
        {
        }

        public DbSet<HourlyForecast> HourlyForecasts { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<HourlyForecast>().HasKey(x => new { x.Month, x.Day, x.Hour });
        }
    }
}
