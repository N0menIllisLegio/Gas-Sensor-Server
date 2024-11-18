using Gss.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gss.Infrastructure.TypeConfigurations;

internal sealed class SensorDataConfiguration: IEntityTypeConfiguration<SensorData>
{
    public void Configure(EntityTypeBuilder<SensorData> builder)
    {
        builder.Ignore(x => x.Id);

        builder.HasKey(e =>
            new { MicrocontrollerID = e.MicrocontrollerId, SensorID = e.SensorId, e.ValueReadTime });
    }
}
