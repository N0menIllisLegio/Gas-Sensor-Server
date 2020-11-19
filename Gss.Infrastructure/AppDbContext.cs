using System;
using Gss.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Gss.Infrastructure
{
  public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid, IdentityUserClaim<Guid>,
      IdentityUserRole<Guid>, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
  {
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    { }

    public DbSet<Sensor> Sensors { get; set; }
    public DbSet<SensorData> SensorsData { get; set; }
    public DbSet<Microcontroller> Microcontrollers { get; set; }
  }
}
