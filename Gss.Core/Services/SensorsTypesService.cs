using AutoMapper;
using Gss.Core.DTOs;
using Gss.Core.DTOs.SensorType;
using Gss.Core.Entities;
using Gss.Core.Exceptions;
using Gss.Core.Interfaces;
using Gss.Core.Interfaces.Services;
using Gss.Core.Resources;

namespace Gss.Core.Services;

public class SensorsTypesService : ISensorsTypesService
{
  private readonly IUnitOfWork _unitOfWork;

  public SensorsTypesService(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<PagedResultDto<SensorType>> GetAllSensorsTypesAsync(PagedInfoDto pagedInfo)
  {
    var pagedResultDto = await _unitOfWork.SensorsTypes.GetPagedResultAsync(
      pagedInfo.PageNumber, pagedInfo.PageSize,
      type => type.Name.Contains(pagedInfo.SearchString) ||
              type.Units != null && type.Units.Contains(pagedInfo.SearchString),
      pagedInfo.SortOptions);

    return pagedResultDto;
  }

  public async Task<SensorType> GetSensorTypeAsync(Guid sensorTypeId)
  {
    var sensorType = await _unitOfWork.SensorsTypes.FindAsync(sensorTypeId);

    if (sensorType is null)
      throw new NotFoundException(string.Format(Messages.NotFoundErrorString, "Sensor type"));

    return sensorType;
  }

  public async Task<SensorType> CreateSensorTypeAsync(CreateSensorTypeDto createSensorTypeDto)
  {
    var sensorType = _unitOfWork.SensorsTypes.Add(new SensorType
    {
      Name = createSensorTypeDto.Name,
      Units = createSensorTypeDto.Units,
      Icon = createSensorTypeDto.Icon
    });

    bool success = await _unitOfWork.SaveAsync();

    if (!success)
      throw new AppException(string.Format(Messages.CreationFailedErrorString, "Sensor type"));

    return sensorType;
  }

  public async Task UpdateSensorTypeAsync(Guid sensorTypeId, UpdateSensorTypeDto updateSensorTypeDto)
  {
    var sensorTypesUpdatedCount = await _unitOfWork.SensorsTypes
      .UpdateSensorTypeAsync(sensorTypeId, updateSensorTypeDto);

    if (sensorTypesUpdatedCount == 0)
      throw new NotFoundException(string.Format(Messages.NotFoundErrorString, "Sensor type"));
  }

  public async Task DeleteSensorTypeAsync(Guid sensorTypeId)
  {
    await _unitOfWork.SensorsTypes.RemoveAsync(sensorTypeId);
  }
}