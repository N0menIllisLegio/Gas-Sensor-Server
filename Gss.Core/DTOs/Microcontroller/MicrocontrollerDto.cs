using Gss.Core.DTOs.User;

namespace Gss.Core.DTOs.Microcontroller
{
  public class MicrocontrollerDto
  {
    public Guid ID { get; set; }
    public required string Name { get; set; }
    public DateTimeOffset? LastResponseTime { get; set; }
    public bool Public { get; set; }

    public required string IPAddress { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public Guid? RequestedSensorID { get; set; }

    public required UserDto UserInfo { get; set; }

    public required IEnumerable<MicrocontrollerSensorDto> Sensors { get; set; }
  }
}
