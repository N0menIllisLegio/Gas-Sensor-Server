using Gss.Core.DTOs.SensorType;

namespace Gss.Core.DTOs.Sensor;

public class SensorDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public SensorTypeDto Type { get; set; } = null!;
}