namespace Gss.Core.DTOs.SensorType;

public sealed class SensorTypeDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Icon { get; set; }
    public string? Units { get; set; }
}
