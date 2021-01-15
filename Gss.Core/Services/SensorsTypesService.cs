using System;
using System.Threading.Tasks;
using Gss.Core.DTOs;
using Gss.Core.DTOs.SensorType;
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

    public async Task<ServiceResultDto<SensorType>> GetSensorType(Guid sensorTypeID)
    {
      var sensorType = await _sensorsTypesRepository.GetSensorTypeAsync(sensorTypeID);

      if (sensorType is null)
      {
        return new ServiceResultDto<SensorType>()
          .AddError(Messages.NotFoundErrorString, "Sensor type");
      }

      return new ServiceResultDto<SensorType>(sensorType);
    }

    public async Task<ServiceResultDto<SensorType>> AddSensorType(CreateSensorTypeDto dto)
    {
      var sensorType = new SensorType
      {
        Name = dto.Name,
        Units = dto.Units,
        Icon = dto.Icon
      };

      sensorType = _sensorsTypesRepository.AddSensorType(sensorType);

      if (await _sensorsTypesRepository.SaveAsync())
      {
        return new ServiceResultDto<SensorType>(sensorType);
      }

      return new ServiceResultDto<SensorType>()
        .AddError(Messages.CreationFailedErrorString, "Sensor type");
    }

    public async Task<ServiceResultDto<SensorType>> UpdateSensorType(UpdateSensorTypeDto dto)
    {
      var sensorType = await _sensorsTypesRepository.GetSensorTypeAsync(dto.ID);

      if (sensorType is null)
      {
        return new ServiceResultDto<SensorType>()
          .AddError(Messages.NotFoundErrorString, "Sensor type");
      }

      sensorType.Name = dto.Name;
      sensorType.Units = dto.Units;
      sensorType.Icon = dto.Icon;

      if (await _sensorsTypesRepository.SaveAsync())
      {
        return new ServiceResultDto<SensorType>(sensorType);
      }

      return new ServiceResultDto<SensorType>()
        .AddError(Messages.UpdateFailedErrorString, "Sensor type");
    }

    public async Task<ServiceResultDto<SensorType>> DeleteSensorType(Guid sensorTypeID)
    {
      var sensorType = await _sensorsTypesRepository.GetSensorTypeAsync(sensorTypeID);

      if (sensorType is null)
      {
        return new ServiceResultDto<SensorType>()
          .AddError(Messages.NotFoundErrorString, "Sensor type");
      }

      sensorType = _sensorsTypesRepository.DeleteSensorType(sensorType);

      if (await _sensorsTypesRepository.SaveAsync())
      {
        return new ServiceResultDto<SensorType>(sensorType);
      }

      return new ServiceResultDto<SensorType>()
        .AddError(Messages.DeletionFailedErrorString, "Sensor type");
    }
  }
}
