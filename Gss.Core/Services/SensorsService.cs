using AutoMapper;
using Gss.Core.DTOs;
using Gss.Core.DTOs.Sensor;
using Gss.Core.Entities;
using Gss.Core.Exceptions;
using Gss.Core.Interfaces;
using Gss.Core.Interfaces.Services;
using Gss.Core.Mappers;
using Gss.Core.Resources;
using Microsoft.EntityFrameworkCore;

namespace Gss.Core.Services;

public class SensorsService : ISensorsService
{
  private const string Sensor = "Sensor";

  private readonly IUnitOfWork _unitOfWork;

  public SensorsService(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<PagedResultDto<SensorDto>> GetAllSensorsAsync(PagedInfoDto pagedInfoDto)
  {
    var pagedResult = await _unitOfWork.Sensors.GetPagedResultAsync(
        pagedInfoDto,
        search => search.Name.Contains(pagedInfoDto.SearchString) ||
                  search.Description != null && search.Description.Contains(pagedInfoDto.SearchString),
        include => include.Include(sensor => sensor.Type));

    return pagedResult.Convert(x => x.MapToDto());
  }

  public async Task<SensorDto> GetSensorAsync(Guid sensorId)
  {
    var sensor = await _unitOfWork.Sensors.FindAsync(sensorId);

    if (sensor is null)
      throw new NotFoundException(string.Format(Messages.NotFoundErrorString, Sensor));

    return sensor.MapToDto();
  }

  public async Task<SensorDto> CreateSensorAsync(CreateSensorDto createSensorDto)
  {
    var sensor = _unitOfWork.Sensors.Add(new Sensor
    {
      Name = createSensorDto.Name,
      Description = createSensorDto.Description,
      TypeId = createSensorDto.TypeId
    });

    bool success = await _unitOfWork.SaveAsync();

    if (!success)
      throw new AppException(string.Format(Messages.CreationFailedErrorString, Sensor));

    return sensor.MapToDto();
  }

  public async Task UpdateSensorAsync(Guid sensorId, UpdateSensorDto updateSensorDto)
  {
    var updatedCount = await _unitOfWork.Sensors.UpdateSensorAsync(sensorId, updateSensorDto);

    if (updatedCount == 0)
      throw new NotFoundException(string.Format(Messages.NotFoundErrorString, Sensor));
  }

  public async Task DeleteSensorAsync(Guid sensorId)
  {
    await _unitOfWork.Sensors.RemoveAsync(sensorId);
  }
}