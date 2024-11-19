namespace Gss.Core.DTOs.SensorData;

public class SensorDataDto
{
  public DateTimeOffset WatchingDate { get; set; }
  public DateTime ReadTime { get; set; }
  public double AverageValue { get; set; }
}