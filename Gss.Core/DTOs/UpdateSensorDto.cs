using System;

namespace Gss.Core.DTOs
{
  public class UpdateSensorDto
  {
    public string Name { get; set; }
    public string Description { get; set; }
    public Guid Type { get; set; }
  }
}
