using System;
using System.Threading.Tasks;
using Gss.Core.DTOs;
using Gss.Core.DTOs.SensorType;

namespace Gss.Core.Interfaces
{
  public interface ISensorsTypesService
  {
    Task<PagedResultDto<SensorTypeDto>> GetAllSensorsTypes(PagedInfoDto pagedInfo);

    Task<SensorTypeDto> GetSensorType(Guid sensorTypeID);

    Task<SensorTypeDto> AddSensorType(CreateSensorTypeDto dto);

    Task<SensorTypeDto> UpdateSensorType(UpdateSensorTypeDto dto);

    Task<SensorTypeDto> DeleteSensorType(Guid sensorTypeID);
  }
}
