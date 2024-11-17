using Gss.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gss.Infrastructure.TypeConfigurations;

internal sealed class SensorDataConfiguration: IEntityTypeConfiguration<SensorData>
{
    public void Configure(EntityTypeBuilder<SensorData> builder)
    {
        builder.HasKey(e =>
            new { e.MicrocontrollerID, e.SensorID, e.ValueReadTime });
    }
}
