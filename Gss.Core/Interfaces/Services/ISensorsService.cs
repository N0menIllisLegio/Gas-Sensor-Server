using System;
using System.Threading.Tasks;
using Gss.Core.DTOs;
using Gss.Core.DTOs.Sensor;

namespace Gss.Core.Interfaces.Services
{
  public interface ISensorsService
  {
    Task<SensorDto> CreateSensorAsync(CreateSensorDto createSensorDto);
    Task<SensorDto> DeleteSensorAsync(Guid sensorID);
    Task<PagedResultDto<SensorDto>> GetAllSensors(PagedInfoDto pagedRequest);
    Task<PagedResultDto<SensorDto>> GetMicrocontrollerSensors(Guid microcontrollerID);
    Task<SensorDto> GetSensor(Guid sensorID);
    Task<SensorDto> UpdateSensorAsync(UpdateSensorDto updateSensorDto);
  }
}
