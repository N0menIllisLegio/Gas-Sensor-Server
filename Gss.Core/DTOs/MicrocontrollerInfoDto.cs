using System;
using Gss.Core.Entities;

namespace Gss.Core.DTOs
{
  public class MicrocontrollerInfoDto
  {
    public MicrocontrollerInfoDto()
    { }

    public MicrocontrollerInfoDto(Microcontroller microcontroller)
    {
      ID = microcontroller.ID;
      Name = microcontroller.Name;
      IPAddress = microcontroller.IPAddress;
      LastResponseTime = microcontroller.LastResponseTime;
      Latitude = microcontroller.Latitude;
      Longitude = microcontroller.Longitude;
      Public = microcontroller.Public;
    }

    public Guid ID { get; set; }
    public string Name { get; set; }
    public string IPAddress { get; set; }
    public DateTime? LastResponseTime { get; set; }
    public bool Public { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
  }
}
