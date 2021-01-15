using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Gss.Core.Interfaces;

namespace Gss.Core.Entities
{
  public class Microcontroller : IEntity
  {
    public Guid ID { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; }
    public string IPAddress { get; set; }
    public DateTime? LastResponseTime { get; set; }
    public bool Public { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string PasswordHash { get; set; }

    // ---- RELATIONSHIPS

    public virtual User Owner { get; set; }

    public virtual IList<MicrocontrollerSensor> MicrocontrollerSensors { get; set; }

    // ----- NOT IN DB

    public bool UserRequestedData { get; }

    public SensorData TryParseData(string data)
    {
      return null;
    }

    public void LeaveConversationRequest()
    {
    }
  }
}
