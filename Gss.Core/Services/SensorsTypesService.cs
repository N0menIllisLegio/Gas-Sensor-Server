using Gss.Core.DTOs;
using Gss.Core.DTOs.SensorType;
using Gss.Core.Entities;
using Gss.Core.Exceptions;
using Gss.Core.Interfaces;
using Gss.Core.Interfaces.Services;
using Gss.Core.Mappers;
using Gss.Core.Resources;

namespace Gss.Core.Services;

public class SensorsTypesService : ISensorsTypesService
{
  private const string SensorType = "Sensor type";

  private readonly IUnitOfWork _unitOfWork;

  public SensorsTypesService(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<PagedResultDto<SensorTypeDto>> GetAllSensorsTypesAsync(PagedInfoDto pagedInfo, CancellationToken cancellationToken = default)
  {
    var pagedResultDto = await _unitOfWork.SensorsTypes.GetPagedResultAsync(pagedInfo, cancellationToken);

    return pagedResultDto.Convert(x => x.MapToDto());
  }

  public async Task<SensorTypeDto> GetSensorTypeAsync(Guid sensorTypeId, CancellationToken cancellationToken = default)
  {
    var sensorType = await _unitOfWork.SensorsTypes.FindAsync(sensorTypeId, cancellationToken);

    if (sensorType is null)
      throw new NotFoundException(string.Format(Messages.NotFoundErrorString, SensorType));

    return sensorType.MapToDto();
  }

  public async Task<SensorTypeDto> CreateSensorTypeAsync(CreateSensorTypeDto createSensorTypeDto, CancellationToken cancellationToken = default)
  {
    var sensorTypeId = _unitOfWork.SensorsTypes.Add(new SensorType
    {
      Name = createSensorTypeDto.Name,
      Units = createSensorTypeDto.Units,
      Icon = createSensorTypeDto.Icon
    });

    bool success = await _unitOfWork.SaveAsync(cancellationToken);

    if (!success)
      throw new AppException(string.Format(Messages.CreationFailedErrorString, SensorType));

    return await GetSensorTypeAsync(sensorTypeId, cancellationToken);
  }

  public async Task UpdateSensorTypeAsync(Guid sensorTypeId, UpdateSensorTypeDto updateSensorTypeDto, CancellationToken cancellationToken = default)
  {
    var sensorTypesUpdatedCount = await _unitOfWork.SensorsTypes
      .UpdateSensorTypeAsync(sensorTypeId, updateSensorTypeDto, cancellationToken);

    if (sensorTypesUpdatedCount == 0)
      throw new NotFoundException(string.Format(Messages.NotFoundErrorString, SensorType));
  }

  public async Task DeleteSensorTypeAsync(Guid sensorTypeId, CancellationToken cancellationToken = default)
  {
    await _unitOfWork.SensorsTypes.RemoveAsync(sensorTypeId, cancellationToken);
  }
}