using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace AirQualityApp.Models
{
    public class AirQualityContext : DbContext
    {
        public AirQualityContext(DbContextOptions<AirQualityContext> options) : base(options)
        {
        }

        public DbSet<Country> Countries
        {
            get; set;
        }
        public DbSet<Coordinates> Coordinates { get; set; }
        public DbSet<Date> Dates { get; set; }
        public DbSet<Measurement> Measurements { get; set; }

    }
}