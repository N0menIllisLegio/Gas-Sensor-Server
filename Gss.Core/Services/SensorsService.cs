using System;
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

    public async Task<PagedResultDto<SensorDto>> GetAllSensors(PagedInfoDto pagedRequest)
    {
      var sensors = await _unitOfWork.Sensors.GetAllAsync();

      return await Task.FromResult<PagedResultDto<SensorDto>>(null);
    }

    public async Task<PagedResultDto<SensorDto>> GetMicrocontrollerSensors(Guid microcontrollerID)
    {
      return await Task.FromResult<PagedResultDto<SensorDto>>(null);
    }

    public async Task<SensorDto> GetSensor(Guid sensorID)
    {
      return await Task.FromResult<SensorDto>(null);
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

    public async Task<SensorDto> UpdateSensorAsync(UpdateSensorDto dto)
    {
      return await Task.FromResult<SensorDto>(null);
    }

    public async Task<SensorDto> DeleteSensorAsync(Guid sensorID)
    {
      var sensor = await _unitOfWork.Sensors.FindAsync(sensorID);

      if (sensor is null)
      {
        throw new AppException(String.Format(Messages.NotFoundErrorString, _sensor), HttpStatusCode.NotFound);
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
  }
}
