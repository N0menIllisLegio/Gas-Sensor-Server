using System;
using System.Collections.Generic;
using Gss.Core.Entities;

namespace Gss.Core.Models
{
  public class MapMicrocontrollerModel
  {
    public Guid MicrocontrollerID { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public List<SensorType> SensorTypes { get; set; }
  }
}
