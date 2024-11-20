using System.Linq.Expressions;
using Gss.Core.DTOs;
using Gss.Core.DTOs.Microcontroller;
using Gss.Core.DTOs.Sensor;
using Gss.Core.Entities;
using Gss.Core.Exceptions;
using Gss.Core.Interfaces;
using Gss.Core.Interfaces.Services;
using Gss.Core.Mappers;
using Gss.Core.Resources;
using Microsoft.EntityFrameworkCore;

namespace Gss.Core.Services;

public class MicrocontrollersService : IMicrocontrollersService
{
  private const string Microcontroller = "Microcontroller";
  private const string Sensor = "Sensor";

  private readonly IUnitOfWork _unitOfWork;
  private readonly ICurrentUser _currentUser;

  public MicrocontrollersService(IUnitOfWork unitOfWork, ICurrentUser currentUser)
  {
    _unitOfWork = unitOfWork;
    _currentUser = currentUser;
  }

  public async Task<PagedResultDto<MicrocontrollerDto>> GetAllMicrocontrollersAsync(PagedInfoDto pagedInfo, CancellationToken cancellationToken = default)
  {
    var pagedResultDto = await _unitOfWork.Microcontrollers.GetPagedResultAsync(
      pagedInfo,
      search => search.Name.Contains(pagedInfo.SearchString),
      query => query
        .Include(mc => mc.MicrocontrollerSensors)
          .ThenInclude(ms => ms.Sensor)
            .ThenInclude(s => s.Type),
      cancellationToken);

    return pagedResultDto.Convert(x => x.MapToDto());
  }

  public async Task<PagedResultDto<MicrocontrollerDto>> GetPublicMicrocontrollersAsync(PagedInfoDto pagedInfo, CancellationToken cancellationToken = default)
  {
    var pagedResultDto = await _unitOfWork.Microcontrollers.GetPagedResultAsync(
      pagedInfo,
      search => search.Public && search.Name.Contains(pagedInfo.SearchString),
      query => query
        .Include(mc => mc.MicrocontrollerSensors)
          .ThenInclude(ms => ms.Sensor)
            .ThenInclude(s => s.Type),
      cancellationToken);

    return pagedResultDto.Convert(x => x.MapToDto());
  }

  public async Task<List<SensorDto>> GetMicrocontrollerSensorsAsync(Guid microcontrollerId, CancellationToken cancellationToken = default)
  {
    var result = await _unitOfWork.Microcontrollers.FirstOrDefaultAsync(
      x => x.Id == microcontrollerId,
      x => x.Include(e => e.Sensors).ThenInclude(e => e.Type),
      cancellationToken);

    if (result is null)
      throw new NotFoundException(string.Format(Messages.NotFoundErrorString, "Microcontroller"));

    return result.Sensors.Select(x => x.MapToDto()).ToList();
  }

  public async Task<MicrocontrollerDto> GetMicrocontrollerAsync(Guid microcontrollerId, CancellationToken cancellationToken = default)
  {
    var microcontroller = await TryGetMicrocontrollerAsync(microcontrollerId, cancellationToken);

    return microcontroller.MapToDto();
  }

  public async Task<List<MapMicrocontrollerDto>> GetPublicMicrocontrollersMapAsync(MapRequestDto mapRequestDto, CancellationToken cancellationToken = default)
  {
    var visibleMicrocontrollers = await _unitOfWork.Microcontrollers.GetVisibleMicrocontrollersAsync(
      mapRequestDto.SouthWestLatitude, mapRequestDto.SouthWestLongitude, mapRequestDto.NorthEastLatitude,
      mapRequestDto.NorthEastLongitude, cancellationToken);

    return visibleMicrocontrollers;
  }

  public async Task<PagedResultDto<MicrocontrollerDto>> GetUserMicrocontrollersAsync(Guid userId,
    PagedInfoDto pagedInfo, CancellationToken cancellationToken = default)
  {
    Expression<Func<Microcontroller, bool>> searchCriteria =
      userId == _currentUser.Id || _currentUser.IsAdministrator
        ? mc => mc.OwnerId == userId && mc.Name.Contains(pagedInfo.SearchString)
        : mc => mc.OwnerId == userId && mc.Name.Contains(pagedInfo.SearchString) && mc.Public;

    var pagedResultDto = await _unitOfWork.Microcontrollers.GetPagedResultAsync(
      pagedInfo,
      searchCriteria,
      query => query
        .Include(mc => mc.MicrocontrollerSensors)
          .ThenInclude(ms => ms.Sensor)
            .ThenInclude(s => s.Type), cancellationToken);

    return pagedResultDto.Convert(x => x.MapToDto());
  }

  public async Task<MicrocontrollerDto> AddMicrocontrollerAsync(CreateMicrocontrollerDto createMicrocontrollerDto, CancellationToken cancellationToken = default)
  {
    var microcontrollerId = Guid.NewGuid();

    _unitOfWork.Microcontrollers.Add(new Microcontroller
    {
      OwnerId = _currentUser.Id,
      Name = createMicrocontrollerDto.Name,
      Key = createMicrocontrollerDto.Key,
      Public = createMicrocontrollerDto.Public,
      Longitude = createMicrocontrollerDto.Longitude,
      Latitude = createMicrocontrollerDto.Latitude,
      MicrocontrollerSensors = createMicrocontrollerDto
        .SensorIDs
        .Select(sensor =>
          new MicrocontrollerSensors
          {
            MicrocontrollerId = microcontrollerId,
            SensorId = sensor
          })
        .ToList()
    });

    bool success = await _unitOfWork.SaveAsync(cancellationToken);

    if (!success)
      throw new AppException(string.Format(Messages.CreationFailedErrorString, Microcontroller));

    var microcontroller = await _unitOfWork.Microcontrollers.FirstOrDefaultAsync(
      x => x.Id == microcontrollerId,
      query => query
        .Include(mc => mc.MicrocontrollerSensors)
          .ThenInclude(ms => ms.Sensor)
            .ThenInclude(s => s.Type), cancellationToken);

    if (microcontroller is null)
      throw new AppException(string.Format(Messages.CreationFailedErrorString, Microcontroller));

    return microcontroller.MapToDto();
  }

  public async Task UpdateMicrocontrollerAsync(
    Guid microcontrollerId, UpdateMicrocontrollerDto updateMicrocontrollerDto, CancellationToken cancellationToken = default)
  {
    var microcontroller = await TryGetMicrocontrollerAsync(microcontrollerId, cancellationToken);

    if (microcontroller.MicrocontrollerSensors.Count -
        updateMicrocontrollerDto.RemoveMicrocontrollerSensorIds.Count +
        updateMicrocontrollerDto.AddSensorIds.Count > 5)
      throw new UserInputException("Sensors can't be more than 5 per microcontroller");

    microcontroller.Name = updateMicrocontrollerDto.Name;
    microcontroller.Key = updateMicrocontrollerDto.Key;
    microcontroller.Public = updateMicrocontrollerDto.Public;
    microcontroller.Longitude = updateMicrocontrollerDto.Longitude;
    microcontroller.Latitude = updateMicrocontrollerDto.Latitude;
    microcontroller.RequestedMicrocontrollerSensorId = null;

    foreach (var currentSensor in microcontroller.MicrocontrollerSensors.ToArray())
    {
      var sensorToRemove = updateMicrocontrollerDto.RemoveMicrocontrollerSensorIds
        .FirstOrDefault(x => x == currentSensor.Id);

      if (sensorToRemove != default)
        microcontroller.MicrocontrollerSensors.Remove(currentSensor);
    }

    foreach (var newSensorId in updateMicrocontrollerDto.AddSensorIds)
    {
      microcontroller.MicrocontrollerSensors.Add(new MicrocontrollerSensors
      {
        MicrocontrollerId = microcontrollerId,
        SensorId = newSensorId
      });
    }

    if (microcontroller.MicrocontrollerSensors.Count > 5)
      throw new UserInputException("Sensors can't be more than 5 per microcontroller");

    bool success = await _unitOfWork.SaveAsync(cancellationToken);

    if (!success)
      throw new AppException(string.Format(Messages.UpdateFailedErrorString, Microcontroller));
  }

  public async Task DeleteMicrocontrollerAsync(Guid microcontrollerId, CancellationToken cancellationToken = default)
  {
    await _unitOfWork.Microcontrollers.RemoveAsync(microcontrollerId, cancellationToken);
  }

  public async Task<RequestSensorValueResponseDto> RequestSensorValueAsync(
    Guid microcontrollerId, Guid microcontrollerSensorId, CancellationToken cancellationToken = default)
  {
    var microcontroller = await TryGetMicrocontrollerAsync(microcontrollerId, cancellationToken);

    if (microcontroller.RequestedMicrocontrollerSensorId == microcontrollerSensorId)
    {
      return new RequestSensorValueResponseDto
      {
        PreviousRequestedSensorId = microcontrollerSensorId,
        CurrentRequestedSensorId = microcontrollerSensorId
      };
    }

    _ = microcontroller.MicrocontrollerSensors.FirstOrDefault(ms => ms.Id == microcontrollerSensorId)
      ?? throw new NotFoundException(string.Format(Messages.NotFoundErrorString, Sensor));

    var result = new RequestSensorValueResponseDto
    {
      PreviousRequestedSensorId = microcontroller.RequestedMicrocontrollerSensorId,
      CurrentRequestedSensorId = microcontrollerSensorId
    };

    microcontroller.RequestedMicrocontrollerSensorId = microcontrollerSensorId;

    bool success = await _unitOfWork.SaveAsync(cancellationToken);

    if (!success)
      throw new AppException(string.Format(Messages.ChangeReuqestedSensorIDFailedErrorString));

    return result;
  }

  public async Task SetSensorValueThresholdAsync(Guid microcontrollerSensorId, int? criticalValue, CancellationToken cancellationToken = default)
  {
    var updatedEntries = await _unitOfWork.Microcontrollers.SetSensorValueThresholdAsync(
      _currentUser, microcontrollerSensorId, criticalValue, cancellationToken);

    if (updatedEntries == 0)
      throw new NotFoundException(string.Format(Messages.NotFoundErrorString, Microcontroller));
  }

  private async Task<Microcontroller> TryGetMicrocontrollerAsync(Guid microcontrollerId, CancellationToken cancellationToken)
  {
    var microcontroller = await _unitOfWork.Microcontrollers.FirstOrDefaultAsync(mc => mc.Id == microcontrollerId,
      x => x.Include(p => p.MicrocontrollerSensors).ThenInclude(p => p.Sensor).ThenInclude(p => p.Type),
      cancellationToken);

    if (microcontroller is not null)
    {
      if (microcontroller.Public || _currentUser.IsAdministrator || microcontroller.OwnerId == _currentUser.Id)
        return microcontroller;
    }

    throw new NotFoundException(string.Format(Messages.NotFoundErrorString, Microcontroller));
  }
}
