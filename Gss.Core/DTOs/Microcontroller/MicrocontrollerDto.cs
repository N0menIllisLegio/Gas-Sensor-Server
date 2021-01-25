using System;
using System.Collections.Generic;
using Gss.Core.DTOs.Sensor;
using Gss.Core.DTOs.User;

namespace Gss.Core.DTOs.Microcontroller
{
  public class MicrocontrollerDto
  {
    public Guid ID { get; set; }
    public string Name { get; set; }
    public DateTime? LastResponseTime { get; set; }
    public bool Public { get; set; }

    public string IPAddress { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    public UserDto UserInfo { get; set; }

    public IEnumerable<SensorDto> Sensors { get; set; }
  }
}
