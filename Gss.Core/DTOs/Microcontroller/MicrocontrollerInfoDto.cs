using System;
using Gss.Core.DTOs.User;

namespace Gss.Core.DTOs.Microcontroller
{
  public class MicrocontrollerInfoDto
  {
    public MicrocontrollerInfoDto()
    { }

    public MicrocontrollerInfoDto(Entities.Microcontroller microcontroller,
      bool displaySensitiveInfo = true)
    {
      ID = microcontroller.ID;
      Name = microcontroller.Name;
      LastResponseTime = microcontroller.LastResponseTime;
      Public = microcontroller.Public;
      UserInfo = microcontroller.Owner is not null
        ? new UserInfoDto(microcontroller.Owner)
        : null;

      if (displaySensitiveInfo)
      {
        IPAddress = microcontroller.IPAddress;
        Latitude = microcontroller.Latitude;
        Longitude = microcontroller.Longitude;
      }
    }

    public Guid ID { get; init; }
    public string Name { get; init; }
    public string IPAddress { get; init; }
    public DateTime? LastResponseTime { get; init; }
    public bool Public { get; init; }
    public double? Latitude { get; init; }
    public double? Longitude { get; init; }
    public UserInfoDto UserInfo { get; init; }
  }
}
