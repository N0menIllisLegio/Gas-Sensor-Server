using System;
using System.Threading.Tasks;
using Gss.Core.DTOs;
using Gss.Core.DTOs.SensorType;

namespace Gss.Core.Interfaces.Services
{
  public interface ISensorsTypesService
  {
    Task<PagedResultDto<SensorTypeDto>> GetAllSensorsTypesAsync(PagedInfoDto pagedInfo);
    Task<SensorTypeDto> GetSensorTypeAsync(Guid sensorTypeID);
    Task<SensorTypeDto> CreateSensorTypeAsync(CreateSensorTypeDto dto);
    Task<SensorTypeDto> UpdateSensorTypeAsync(UpdateSensorTypeDto dto);
    Task<SensorTypeDto> DeleteSensorTypeAsync(Guid sensorTypeID);
  }
}
