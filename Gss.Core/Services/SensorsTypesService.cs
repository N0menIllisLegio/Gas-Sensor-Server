using System;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Gss.Core.DTOs;
using Gss.Core.DTOs.SensorType;
using Gss.Core.Entities;
using Gss.Core.Exceptions;
using Gss.Core.Interfaces;
using Gss.Core.Resources;

namespace Gss.Core.Services
{
  public class SensorsTypesService : ISensorsTypesService
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SensorsTypesService(IUnitOfWork unitOfWork, IMapper mapper)
    {
      _unitOfWork = unitOfWork;
      _mapper = mapper;
    }

    public async Task<PagedResultDto<SensorTypeDto>> GetAllSensorsTypesAsync(PagedInfoDto pagedInfo)
    {
      var pagedResultDto = await _unitOfWork.SensorsTypes.GetPagedResultAsync(pagedInfo, sensorType => new { sensorType.Name, sensorType.Units });

      return pagedResultDto.Convert<SensorTypeDto>(_mapper);
    }

    public async Task<SensorTypeDto> GetSensorTypeAsync(Guid sensorTypeID)
    {
      var sensorType = await _unitOfWork.SensorsTypes.FindAsync(sensorTypeID);

      if (sensorType is null)
      {
        throw new AppException(String.Format(Messages.NotFoundErrorString, "Sensor type"),
          HttpStatusCode.NotFound);
      }

      return _mapper.Map<SensorTypeDto>(sensorType);
    }

    public async Task<SensorTypeDto> AddSensorTypeAsync(CreateSensorTypeDto createSensorTypeDto)
    {
      var sensorType = _mapper.Map<SensorType>(createSensorTypeDto);
      sensorType = _unitOfWork.SensorsTypes.Add(sensorType);

      bool success = await _unitOfWork.SaveAsync();

      if (!success)
      {
        throw new AppException(String.Format(Messages.CreationFailedErrorString, "Sensor type"),
          HttpStatusCode.BadRequest);
      }

      return _mapper.Map<SensorTypeDto>(sensorType);
    }

    public async Task<SensorTypeDto> UpdateSensorTypeAsync(UpdateSensorTypeDto updateSensorTypeDto)
    {
      var sensorType = await _unitOfWork.SensorsTypes.FindAsync(updateSensorTypeDto.ID);

      if (sensorType is null)
      {
        throw new AppException(String.Format(Messages.NotFoundErrorString, "Sensor type"),
          HttpStatusCode.NotFound);
      }

      sensorType.Name = updateSensorTypeDto.Name;
      sensorType.Units = updateSensorTypeDto.Units;
      sensorType.Icon = updateSensorTypeDto.Icon;

      bool success = await _unitOfWork.SaveAsync();

      if (!success)
      {
        throw new AppException(String.Format(Messages.UpdateFailedErrorString, "Sensor type"),
          HttpStatusCode.BadRequest);
      }

      return _mapper.Map<SensorTypeDto>(sensorType);
    }

    public async Task<SensorTypeDto> DeleteSensorTypeAsync(Guid sensorTypeID)
    {
      var sensorType = await _unitOfWork.SensorsTypes.FindAsync(sensorTypeID);

      if (sensorType is null)
      {
        throw new AppException(String.Format(Messages.NotFoundErrorString, "Sensor type"),
          HttpStatusCode.NotFound);
      }

      sensorType = _unitOfWork.SensorsTypes.Remove(sensorType);

      bool success = await _unitOfWork.SaveAsync();

      if (!success)
      {
        throw new AppException(String.Format(Messages.DeletionFailedErrorString, "Sensor type"),
          HttpStatusCode.BadRequest);
      }

      return _mapper.Map<SensorTypeDto>(sensorType);
    }
  }
}
