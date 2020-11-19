using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gss.Core.Entities
{
  public class SensorData
  {
    [Key]
    [Column(Order = 1)]
    public Microcontroller Microcontroller { get; set; }

    [Key]
    [Column(Order = 2)]
    public DateTime ValueReadTime { get; set; }

    public int SensorValue { get; set; }
    public DateTime ValueReceivedTime { get; set; }
  }
}
