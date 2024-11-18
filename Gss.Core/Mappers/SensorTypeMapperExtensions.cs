using Gss.Core.DTOs.SensorType;
using Gss.Core.Entities;

namespace Gss.Core.Mappers;

internal static class SensorTypeMapperExtensions
{
    public static SensorTypeDto MapToDto(this SensorType sensorType)
    {
        return new SensorTypeDto
        {
            Id = sensorType.Id,
            Name = sensorType.Name,
            Units = sensorType.Units,
            Icon = sensorType.Icon,
        };
    }
}
