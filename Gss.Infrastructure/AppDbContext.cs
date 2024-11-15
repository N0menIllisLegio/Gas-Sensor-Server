using Gss.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Gss.Infrastructure
{
  public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid, IdentityUserClaim<Guid>,
      IdentityUserRole<Guid>, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
  {
    private readonly Guid _administratorRoleID = Guid.Parse("2672396d-e4e2-4f8e-880f-cca9a7a260d6");
    private readonly Guid _userRoleID = Guid.Parse("04bf1ca1-0600-4c4b-86c5-2f16998b02d8");
    private readonly Guid _administratorID = Guid.Parse("a21afc0b-1135-4b23-a672-758e1f788bc8");

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    { }

    public DbSet<Sensor> Sensors { get; set; }
    public DbSet<SensorData> SensorsData { get; set; }
    public DbSet<Microcontroller> Microcontrollers { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<SensorType> SensorsTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);
      builder.Entity<SensorData>().HasKey(e =>
        new { e.MicrocontrollerID, e.SensorID, e.ValueReadTime });

      builder.Entity<RefreshToken>()
        .HasOne(p => p.User)
        .WithMany(p => p.RefreshTokens)
        .OnDelete(DeleteBehavior.Cascade);

      builder.Entity<IdentityRole<Guid>>()
        .HasData(new IdentityRole<Guid>
          {
          Id = _administratorRoleID,
          Name = "Administrator",
          NormalizedName = "ADMINISTRATOR",
          ConcurrencyStamp = Guid.NewGuid().ToString()
        },
        new IdentityRole<Guid>
        {
          Id = _userRoleID,
          Name = "User",
          NormalizedName = "USER",
          ConcurrencyStamp = Guid.NewGuid().ToString()
        });

      builder.Entity<User>()
        .HasData(new User
        {
          Id = _administratorID,
          Email = "example@example.com",
          NormalizedEmail = "EXAMPLE@EXAMPLE.COM",
          UserName = "example@example.com",
          NormalizedUserName = "EXAMPLE@EXAMPLE.COM",
          TwoFactorEnabled = false,
          PhoneNumberConfirmed = false,
          LockoutEnabled = false,
          ConcurrencyStamp = Guid.NewGuid().ToString(),
          SecurityStamp = Guid.NewGuid().ToString(),
          CreationDate = DateTimeOffset.UtcNow,
          FirstName = "Admin",
          PasswordHash = "AQAAAAIAAYagAAAAEIztH6wjdB+L4cs5Dj7hQbtinLbe1++8zmazhsCk5Q1gDoNs25exsRuXMo2q+i9iHg==",
          LastName = string.Empty,
          AvatarPath = string.Empty,
          Gender = string.Empty,
        });

      builder.Entity<IdentityUserRole<Guid>>()
        .HasData(new IdentityUserRole<Guid>
          {
            UserId = _administratorID,
            RoleId = _administratorRoleID
          });
    }
  }
}
