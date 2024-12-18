using Gss.Core.DTOs.Sensor;
using Gss.Core.Entities;

namespace Gss.Core.Mappers;

public static class SensorMapperExtensions
{
    public static SensorDto MapToDto(this Sensor sensor)
    {
        return new SensorDto
        {
            Id = sensor.Id,
            Name = sensor.Name,
            Description = sensor.Description,
            Type = sensor.Type.MapToDto()
        };
    }
}