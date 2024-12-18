namespace Gss.Core.DTOs.Sensor;

public class CreateSensorDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required Guid TypeId { get; set; }
}