using Gss.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gss.Infrastructure.TypeConfigurations;

internal sealed class SensorDataConfiguration : IEntityTypeConfiguration<SensorData>
{
    public void Configure(EntityTypeBuilder<SensorData> builder)
    {
        builder.HasKey(e => new { e.MicrocontrollerSensorId, ValueReadTime = e.ReadTime });
        builder.HasOne<MicrocontrollerSensors>()
            .WithMany()
            .HasForeignKey(x => x.MicrocontrollerSensorId);
    }
}