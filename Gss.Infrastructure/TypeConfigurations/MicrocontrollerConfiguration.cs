using Gss.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gss.Infrastructure.TypeConfigurations;

internal sealed class MicrocontrollerConfiguration: IEntityTypeConfiguration<Microcontroller>
{
    public void Configure(EntityTypeBuilder<Microcontroller> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.HasIndex(x => x.OwnerId);

        builder
            .HasMany(x => x.Sensors)
            .WithMany(x => x.Microcontrollers)
            .UsingEntity<MicrocontrollerSensors>(
                l => l.HasOne(x => x.Sensor).WithMany(x => x.SensorMicrocontrollers),
                r => r.HasOne(x => x.Microcontroller).WithMany(x => x.MicrocontrollerSensors));
    }
}
