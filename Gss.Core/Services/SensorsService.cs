using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Gss.Core.DTOs;
using Gss.Core.DTOs.Sensor;
using Gss.Core.Entities;
using Gss.Core.Exceptions;
using Gss.Core.Interfaces;
using Gss.Core.Interfaces.Services;
using Gss.Core.Resources;
using Microsoft.EntityFrameworkCore;

namespace Gss.Core.Services
{
  public class SensorsService : ISensorsService
  {
    private const string _sensor = "Sensor";

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SensorsService(IUnitOfWork unitOfWork, IMapper mapper)
    {
      _unitOfWork = unitOfWork;
      _mapper = mapper;
    }

    public async Task<PagedResultDto<SensorDto>> GetAllSensors(PagedInfoDto pagedInfoDto)
    {
      var pagedResult = await _unitOfWork.Sensors.GetPagedResultAsync(pagedInfoDto,
                sensor => new { sensor.Id, sensor.Name, sensor.Description },
                include: query => query.Include((sensor) => sensor.Type));

      return pagedResult.Convert<SensorDto>(_mapper);
    }

    public async Task<PagedResultDto<SensorDto>> GetMicrocontrollerSensors(Guid microcontrollerID, PagedInfoDto pagedInfoDto)
    {
      var pagedResult = await _unitOfWork.Sensors.GetPagedResultAsync(pagedInfoDto,
        sensor => new { sensor.Id, sensor.Name, sensor.Description },
        sensor => sensor.SensorMicrocontrollers.Any(microcontollerSensor => microcontollerSensor.MicrocontrollerID == microcontrollerID),
        query => query.Include(sensor => sensor.Type));

      return pagedResult.Convert<SensorDto>(_mapper);
    }

    public async Task<SensorDto> GetSensorAsync(Guid sensorID)
    {
      var sensor = await _unitOfWork.Sensors.FindAsync(sensorID);

      if (sensor is null)
      {
        throw new AppException(String.Format(Messages.NotFoundErrorString, _sensor),
          HttpStatusCode.NotFound);
      }

      return _mapper.Map<SensorDto>(sensor);
    }

    public async Task<SensorDto> CreateSensorAsync(CreateSensorDto createSensorDto)
    {
      var sensor = _mapper.Map<Sensor>(createSensorDto);
      sensor = _unitOfWork.Sensors.Add(sensor);

      bool success = await _unitOfWork.SaveAsync();

      if (!success)
      {
        throw new AppException(String.Format(Messages.CreationFailedErrorString, _sensor),
          HttpStatusCode.BadRequest);
      }

      return _mapper.Map<SensorDto>(sensor);
    }

    public async Task<SensorDto> UpdateSensorAsync(Guid sensorID, UpdateSensorDto updateSensorDto)
    {
      var sensor = await _unitOfWork.Sensors.FindAsync(sensorID);

      if (sensor is null)
      {
        throw new AppException(String.Format(Messages.NotFoundErrorString, _sensor),
          HttpStatusCode.NotFound);
      }

      sensor = _mapper.Map(updateSensorDto, sensor);

      bool success = await _unitOfWork.SaveAsync();

      if (!success)
      {
        throw new AppException(String.Format(Messages.UpdateFailedErrorString, _sensor),
          HttpStatusCode.BadRequest);
      }

      return _mapper.Map<SensorDto>(sensor);
    }

    public async Task<SensorDto> DeleteSensorAsync(Guid sensorID)
    {
      var sensor = await _unitOfWork.Sensors.FindAsync(sensorID);

      if (sensor is null)
      {
        throw new AppException(String.Format(Messages.NotFoundErrorString, _sensor),
          HttpStatusCode.NotFound);
      }

      sensor = _unitOfWork.Sensors.Remove(sensor);

      bool success = await _unitOfWork.SaveAsync();

      if (!success)
      {
        throw new AppException(String.Format(Messages.DeletionFailedErrorString, _sensor),
          HttpStatusCode.BadRequest);
      }

      return _mapper.Map<SensorDto>(sensor);
    }

    public async Task<SensorDto> SetSensorTypeAsync(SetSensorTypeDto dto)
    {
      var sensor = await _unitOfWork.Sensors.FindAsync(dto.SensorID);

      if (sensor is null)
      {
        throw new AppException(String.Format(Messages.NotFoundErrorString, _sensor),
          HttpStatusCode.NotFound);
      }

      var sensorType = await _unitOfWork.SensorsTypes.FindAsync(dto.SensorTypeID);

      if (sensorType is null)
      {
        throw new AppException(String.Format(Messages.NotFoundErrorString, "Sensor's type"),
          HttpStatusCode.NotFound);
      }

      sensor.Type = sensorType;

      bool success = await _unitOfWork.SaveAsync();

      if (!success)
      {
        throw new AppException(String.Format(Messages.DeletionFailedErrorString, _sensor),
          HttpStatusCode.BadRequest);
      }

      return _mapper.Map<SensorDto>(sensor);
    }
  }
}
