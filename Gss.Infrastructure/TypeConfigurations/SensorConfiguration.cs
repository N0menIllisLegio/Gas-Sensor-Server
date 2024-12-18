using Gss.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gss.Infrastructure.TypeConfigurations;

internal sealed class SensorConfiguration : IEntityTypeConfiguration<Sensor>
{
    public void Configure(EntityTypeBuilder<Sensor> builder)
    {
        builder.Property(x => x.Description).HasMaxLength(2000);
        builder.Property(x => x.Name).HasMaxLength(200);
    }
}