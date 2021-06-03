using System;

namespace Gss.Core.DTOs.SensorType
{
  public class SensorTypeDto
  {
    public Guid ID { get; init; }
    public string Name { get; init; }
    public string Icon { get; init; }
    public string Units { get; init; }
  }
}
