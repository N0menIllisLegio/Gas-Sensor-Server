using System.Reflection;
using Gss.Core.Entities;
using Gss.Infrastructure.TypeConfigurations;
using Microsoft.EntityFrameworkCore;

namespace Gss.Infrastructure;

public class AppDbContext : DbContext
{
  public AppDbContext(DbContextOptions<AppDbContext> options)
    : base(options)
  { }

  public DbSet<Sensor> Sensors { get; set; }
  public DbSet<SensorData> SensorsData { get; set; }
  public DbSet<Microcontroller> Microcontrollers { get; set; }
  public DbSet<SensorType> SensorsTypes { get; set; }

  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);

    builder.ApplyConfigurationsFromAssembly(
      Assembly.GetAssembly(typeof(MicrocontrollerConfiguration))!);
  }
}