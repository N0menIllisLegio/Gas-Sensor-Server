using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Gss.Core.Helpers;
using Gss.Core.Interfaces;

namespace Gss.Core.Entities
{
  public class Microcontroller : IEntity
  {
    [ExpressionsBuilder]
    public Guid ID { get; set; }

    [ExpressionsBuilder]
    [Required]
    [MaxLength(200)]
    public string Name { get; set; }
    public string IPAddress { get; set; }

    [ExpressionsBuilder]
    public DateTime? LastResponseTime { get; set; }

    [ExpressionsBuilder]
    public bool Public { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string PasswordHash { get; set; }

    // ID of microcontroller's sensor
    // from which data will be requested
    public Guid? RequestedSensorID { get; set; }

    // ---- RELATIONSHIPS

    [ExpressionsBuilder]
    public virtual User Owner { get; set; }

    public virtual IList<MicrocontrollerSensors> MicrocontrollerSensors { get; set; }

    // ----- NOT IN DB

    public SensorData TryParseData(string data)
    {
      return null;
    }

    public void LeaveConversationRequest()
    {
    }
  }
}
