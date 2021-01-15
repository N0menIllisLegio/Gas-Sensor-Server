using System;
using System.Threading.Tasks;
using Gss.Core.DTOs;
using Gss.Core.DTOs.SensorType;
using Gss.Core.Entities;
using Gss.Core.Enums;

namespace Gss.Core.Interfaces
{
  public interface ISensorsTypesService
  {
    Task<(ServiceResultDto<SensorType> result, int totalQueriedSensorsTypesCount)> GetAllSensorsTypes(
      int pageNumber, int pageSize, SortOrder sortOrder = SortOrder.None, string filterStr = "");

    Task<ServiceResultDto<SensorType>> GetSensorType(Guid sensorTypeID);

    Task<ServiceResultDto<SensorType>> AddSensorType(CreateSensorTypeDto dto);

    Task<ServiceResultDto<SensorType>> UpdateSensorType(UpdateSensorTypeDto dto);

    Task<ServiceResultDto<SensorType>> DeleteSensorType(Guid sensorTypeID);
  }
}
