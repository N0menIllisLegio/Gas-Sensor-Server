namespace Gss.Core.DTOs.Microcontroller;

public class CreateMicrocontrollerDto
{
  public required string Name { get; set; }
  public required bool Public { get; set; }
  public double? Latitude { get; set; }
  public double? Longitude { get; set; }
  public required string Key { get; set; }
  public required List<Guid> SensorIDs { get; set; }
}