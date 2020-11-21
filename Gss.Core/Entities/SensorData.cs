﻿using System;

namespace Gss.Core.Entities
{
  public class SensorData
  {
    public Guid MicrocontrollerID { get; set; }
    public Microcontroller Microcontroller { get; set; }

    public Guid SensorID { get; set; }
    public Sensor Sensor { get; set; }

    public DateTime ValueReadTime { get; set; }

    public int SensorValue { get; set; }
    public DateTime ValueReceivedTime { get; set; }
  }
}
