using Gss.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gss.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }

        public DbSet<Sensor> Sensors { get; set; }
        public DbSet<SensorData> SensorsData { get; set; }
        public DbSet<Microcontroller> Microcontrollers { get; set; }
    }
}
