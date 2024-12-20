using System.Reflection;
using Gss.Core.Entities;
using Gss.Infrastructure.TypeConfigurations;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Gss.Infrastructure;

public class AppDbContext : DbContext, IDataProtectionKeyContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Sensor> Sensors { get; private set; }
    public DbSet<SensorData> SensorsData { get; private set; }
    public DbSet<Microcontroller> Microcontrollers { get; private set; }
    public DbSet<SensorType> SensorsTypes { get; private set; }
    public DbSet<DataProtectionKey> DataProtectionKeys { get; private set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(
            Assembly.GetAssembly(typeof(MicrocontrollerConfiguration))!);
    }
}