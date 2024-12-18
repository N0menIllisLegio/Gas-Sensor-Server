namespace Gss.Core.DTOs.Microcontroller;

public class UpdateMicrocontrollerDto
{
    public required string Name { get; set; }
    public bool Public { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? Key { get; set; }

    public required List<Guid> AddSensorIds { get; set; }
    public required List<Guid> RemoveMicrocontrollerSensorIds { get; set; }
}