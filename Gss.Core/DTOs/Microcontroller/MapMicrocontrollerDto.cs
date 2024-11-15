using Gss.Core.DTOs.SensorType;

namespace Gss.Core.DTOs.Microcontroller
{
  public class MapMicrocontrollerDto
  {
    public Guid MicrocontrollerID { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public required List<SensorTypeDto> SensorTypes { get; set; }
  }
}
