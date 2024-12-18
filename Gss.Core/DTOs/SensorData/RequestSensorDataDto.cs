using Gss.Core.Utils;

namespace Gss.Core.DTOs.SensorData;

public class RequestSensorDataDto
{
    public Guid MicrocontrollerSensorId { get; set; }
    public SensorDataPeriod Period { get; set; }
    public required List<DateOnly> WatchingDates { get; set; }
}