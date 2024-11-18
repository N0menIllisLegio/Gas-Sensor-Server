using Gss.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gss.Infrastructure.TypeConfigurations;

internal sealed class SensorTypeConfiguration: IEntityTypeConfiguration<SensorType>
{
    public void Configure(EntityTypeBuilder<SensorType> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.Units).HasMaxLength(20);

        builder.HasMany<Sensor>()
            .WithOne(x => x.Type)
            .HasForeignKey(x => x.TypeId);
    }
}
