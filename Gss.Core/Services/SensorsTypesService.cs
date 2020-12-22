using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Gss.Core.DTOs;
using Gss.Core.Entities;
using Gss.Core.Enums;
using Gss.Core.Interfaces;
using Gss.Core.Resources;

namespace Gss.Core.Services
{
  public class SensorsTypesService : ISensorsTypesService
  {
    private readonly ISensorsTypesRepository _sensorsTypesRepository;

    public SensorsTypesService(ISensorsTypesRepository sensorsTypesRepository)
    {
      _sensorsTypesRepository = sensorsTypesRepository;
    }

    public async Task<(ServiceResultDto<SensorType> result, int totalQueriedSensorsTypesCount)> GetAllSensorsTypes(
      int pageNumber, int pageSize,
      SortOrder sortOrder = SortOrder.None, string filterStr = "")
    {
      var (sensorsTypes, totalQueriedSensorsTypesCount) = await _sensorsTypesRepository.GetSensorsTypesAsync(
        pageNumber, pageSize, sortOrder, (type) => type.Name.Contains(filterStr), (type) => type.Name, true);

      return (new ServiceResultDto<SensorType>(sensorsTypes), totalQueriedSensorsTypesCount);
    }

    public async Task<ServiceResultDto<SensorType>> GetSensorType(string sensorTypeID)
    {
      var sensorTypeGuid = Guid.Parse(sensorTypeID);
      var sensorType = await _sensorsTypesRepository.GetSensorTypeAsync(sensorTypeGuid);

      if (sensorType is null)
      {
        return new ServiceResultDto<SensorType>()
          .AddError(Messages.NotFoundErrorString, "Sensor type");
      }

      return new ServiceResultDto<SensorType>(sensorType);
    }
  }
}
