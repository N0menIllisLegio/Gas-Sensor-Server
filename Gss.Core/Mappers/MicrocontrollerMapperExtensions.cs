using Gss.Core.DTOs.Microcontroller;
using Gss.Core.Entities;

namespace Gss.Core.Mappers;

public static class MicrocontrollerMapperExtensions
{
    public static MicrocontrollerDto MapToDto(this Microcontroller microcontroller)
    {
        return new MicrocontrollerDto
        {
            Id = microcontroller.Id,
            Name = microcontroller.Name,
            Latitude = microcontroller.Latitude,
            Longitude = microcontroller.Longitude,
            Public = microcontroller.Public,
            LastResponseTime = microcontroller.LastResponseTime,
            RequestedSensorId = microcontroller.RequestedMicrocontrollerSensorId,
            OwnerId = microcontroller.OwnerId,
            Sensors = microcontroller.MicrocontrollerSensors.Select(x => x.MapToDto())
        };
    }

    public static MicrocontrollerSensorDto MapToDto(this MicrocontrollerSensors microcontroller)
    {
        return new MicrocontrollerSensorDto()
        {
            Id = microcontroller.SensorId,
            MicrocontrollerSensorId = microcontroller.Id,
            CriticalValue = microcontroller.CriticalValue,
            Name = microcontroller.Sensor.Name,
            Description = microcontroller.Sensor.Description,
            Type = microcontroller.Sensor.Type.MapToDto(),
        };
    }
}
