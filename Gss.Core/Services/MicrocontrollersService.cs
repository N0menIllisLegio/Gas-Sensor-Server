using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Gss.Core.DTOs;
using Gss.Core.DTOs.Microcontroller;
using Gss.Core.DTOs.Sensor;
using Gss.Core.Entities;
using Gss.Core.Exceptions;
using Gss.Core.Helpers;
using Gss.Core.Interfaces;
using Gss.Core.Interfaces.Services;
using Gss.Core.Resources;
using Microsoft.EntityFrameworkCore;

namespace Gss.Core.Services
{
  public class MicrocontrollersService : IMicrocontrollersService
  {
    private const string _microcontroller = "Microcontroller";
    private const string _user = "User";
    private const string _sensor = "Sensor";

    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager _userManager;
    private readonly IMapper _mapper;

    public MicrocontrollersService(IUnitOfWork unitOfWork, UserManager userManager, IMapper mapper)
    {
      _unitOfWork = unitOfWork;
      _userManager = userManager;
      _mapper = mapper;
    }

    public async Task<PagedResultDto<MicrocontrollerDto>> GetPublicMicrocontrollersAsync(PagedInfoDto pagedInfo)
    {
      var pagedResultDto = await _unitOfWork.Microcontrollers.GetPagedResultAsync(pagedInfo,
        microcontroller => new { microcontroller.ID, microcontroller.Name, microcontroller.LastResponseTime }, mc => mc.Public);

      return pagedResultDto.Convert<MicrocontrollerDto>(_mapper);
    }

    public async Task<PagedResultDto<MicrocontrollerDto>> GetAllMicrocontrollersAsync(PagedInfoDto pagedInfo)
    {
      var pagedResultDto = await _unitOfWork.Microcontrollers.GetPagedResultAsync(pagedInfo,
        microcontroller => new { microcontroller.ID, microcontroller.Name, microcontroller.LastResponseTime },
        include: query => query.Include(mc => mc.MicrocontrollerSensors).ThenInclude(ms => ms.Sensor).ThenInclude(s => s.Type));

      return pagedResultDto.Convert<MicrocontrollerDto>(_mapper);
    }

    public async Task<PagedResultDto<MicrocontrollerDto>> GetUserMicrocontrollersAsync(string requestedByEmail,
      Guid userID, PagedInfoDto pagedInfo)
    {
      var user = await _userManager.FindByIdAsync(userID);

      if (user is null)
      {
        throw new AppException(String.Format(Messages.NotFoundErrorString, _user),
          HttpStatusCode.NotFound);
      }

      var requestedBy = await _userManager.FindByEmailAsync(requestedByEmail);
      bool administratorClaim = await _userManager.IsAdministrator(requestedByEmail);

      Expression<Func<Microcontroller, bool>> additionalCriteria = user == requestedBy || administratorClaim
        ? mc => mc.Owner.ID == user.ID
        : mc => mc.Owner.ID == user.ID && mc.Public;

      var pagedResultDto = await _unitOfWork.Microcontrollers.GetPagedResultAsync(pagedInfo,
          microcontroller => new { microcontroller.ID, microcontroller.Name, microcontroller.LastResponseTime },
          additionalCriteria,
          query => query.Include(mc => mc.MicrocontrollerSensors).ThenInclude(ms => ms.Sensor).ThenInclude(s => s.Type));

      return pagedResultDto.Convert<MicrocontrollerDto>(_mapper);
    }

    public async Task<MicrocontrollerDto> GetMicrocontrollerAsync(string requestedByEmail, Guid microcontrollerID)
    {
      var microcontroller = await _unitOfWork.Microcontrollers.FindAsync(microcontrollerID);

      if (microcontroller is null)
      {
        throw new AppException(String.Format(Messages.NotFoundErrorString, _microcontroller),
          HttpStatusCode.NotFound);
      }

      if (microcontroller.Owner is not null
        && microcontroller.Owner.Email.Equals(requestedByEmail))
      {
        return _mapper.Map<MicrocontrollerDto>(microcontroller);
      }

      bool administratorClaim = await _userManager.IsAdministrator(requestedByEmail);

      if (administratorClaim)
      {
        return _mapper.Map<MicrocontrollerDto>(microcontroller);
      }

      if (microcontroller.Public)
      {
        return _mapper.Map<MicrocontrollerDto>(microcontroller, options => options.AfterMap(
          (src, dst) =>
          {
            dst.Latitude = dst.Longitude = null;
            dst.IPAddress = null;
          }));
      }

      throw new AppException(Messages.AccessDeniedErrorString, HttpStatusCode.Unauthorized);
    }

    public async Task<MicrocontrollerDto> AddMicrocontrollerAsync(string requestedByEmail,
      CreateMicrocontrollerDto createMicrocontrollerDto)
    {
      var user = await _userManager.FindByEmailAsync(requestedByEmail);

      if (user is null)
      {
        throw new AppException(String.Format(Messages.NotFoundErrorString, _user),
          HttpStatusCode.NotFound);
      }

      var microcontroller = _mapper.Map<Microcontroller>(createMicrocontrollerDto);
      microcontroller.Owner = user;
      microcontroller = _unitOfWork.Microcontrollers.Add(microcontroller);

      bool success = await _unitOfWork.SaveAsync();

      if (!success)
      {
        throw new AppException(String.Format(Messages.CreationFailedErrorString, _microcontroller),
          HttpStatusCode.BadRequest);
      }

      return _mapper.Map<MicrocontrollerDto>(microcontroller);
    }

    public async Task<MicrocontrollerDto> UpdateMicrocontrollerAsync(string requestedByEmail, Guid microcontrollerID,
      UpdateMicrocontrollerDto updateMicrocontrollerDto)
    {
      var microcontroller = await TryGetMicrocontroller(microcontrollerID, requestedByEmail);

      if (microcontroller is null)
      {
        throw new AppException(String.Format(Messages.NotFoundErrorString, _microcontroller),
          HttpStatusCode.NotFound);
      }

      _mapper.Map(updateMicrocontrollerDto, microcontroller);

      bool success = await _unitOfWork.SaveAsync();

      if (!success)
      {
        throw new AppException(String.Format(Messages.UpdateFailedErrorString, _microcontroller),
          HttpStatusCode.BadRequest);
      }

      return _mapper.Map<MicrocontrollerDto>(microcontroller);
    }

    public async Task<MicrocontrollerDto> ChangeMicrocontrollerOwnerAsync(Guid microcontrollerID, Guid newOwnerID)
    {
      var microcontroller = await _unitOfWork.Microcontrollers.FindAsync(microcontrollerID);

      if (microcontroller is null)
      {
        throw new AppException(String.Format(Messages.NotFoundErrorString, _microcontroller),
          HttpStatusCode.NotFound);
      }

      var user = await _userManager.FindByIdAsync(newOwnerID);

      if (user is null)
      {
        throw new AppException(String.Format(Messages.NotFoundErrorString, _user),
          HttpStatusCode.NotFound);
      }

      microcontroller.Owner = user;

      bool success = await _unitOfWork.SaveAsync();

      if (!success)
      {
        throw new AppException(String.Format(Messages.UpdateFailedErrorString, _microcontroller),
          HttpStatusCode.BadRequest);
      }

      return _mapper.Map<MicrocontrollerDto>(microcontroller);
    }

    public async Task<MicrocontrollerDto> DeleteMicrocontrollerAsync(string requestedByEmail, Guid microcontrollerID)
    {
      var microcontroller = await TryGetMicrocontroller(microcontrollerID, requestedByEmail);

      if (microcontroller is null)
      {
        throw new AppException(String.Format(Messages.NotFoundErrorString, _microcontroller),
          HttpStatusCode.NotFound);
      }

      microcontroller = _unitOfWork.Microcontrollers.Remove(microcontroller);

      bool success = await _unitOfWork.SaveAsync();

      if (!success)
      {
        throw new AppException(String.Format(Messages.DeletionFailedErrorString, _microcontroller),
          HttpStatusCode.BadRequest);
      }

      return _mapper.Map<MicrocontrollerDto>(microcontroller);
    }

    public async Task<MicrocontrollerDto> AddSensorAsync(string requestedByEmail, AddSensorDto dto)
    {
      var microcontroller = await TryGetMicrocontroller(dto.MicrocontollerID, requestedByEmail);

      if (microcontroller is null)
      {
        throw new AppException(String.Format(Messages.NotFoundErrorString, _microcontroller),
          HttpStatusCode.NotFound);
      }

      var sensor = await _unitOfWork.Sensors.FindAsync(dto.SensorID);

      if (sensor is null)
      {
        throw new AppException(String.Format(Messages.NotFoundErrorString, _sensor),
          HttpStatusCode.NotFound);
      }

      var microcontrollersSensor = new MicrocontrollerSensors
      {
        Microcontroller = microcontroller,
        Sensor = sensor
      };

      microcontroller.MicrocontrollerSensors.Add(microcontrollersSensor);

      bool success = await _unitOfWork.SaveAsync();

      if (!success)
      {
        throw new AppException(Messages.FailedAddingSensorToMicrocontrollerErrorString,
          HttpStatusCode.BadRequest);
      }

      return _mapper.Map<MicrocontrollerDto>(microcontroller);
    }

    public async Task<MicrocontrollerDto> RemoveSensorAsync(string requestedByEmail, RemoveSensorDto dto)
    {
      var microcontroller = await TryGetMicrocontroller(dto.MicrocontollerID, requestedByEmail);

      if (microcontroller is null)
      {
        throw new AppException(String.Format(Messages.NotFoundErrorString, _microcontroller),
          HttpStatusCode.NotFound);
      }

      var microcontrollerSensor = microcontroller.MicrocontrollerSensors
        .FirstOrDefault(microcontrollerSensors => microcontrollerSensors.SensorID == dto.SensorID);

      if (microcontrollerSensor is null)
      {
        throw new AppException(String.Format(Messages.NotFoundErrorString, _sensor),
          HttpStatusCode.NotFound);
      }

      microcontroller.MicrocontrollerSensors.Remove(microcontrollerSensor);

      bool success = await _unitOfWork.SaveAsync();

      if (!success)
      {
        throw new AppException(Messages.FailedRemovingSensorFromMicrocontrollerErrorString,
          HttpStatusCode.BadRequest);
      }

      var result = _mapper.Map<MicrocontrollerDto>(microcontroller);
      result.Sensors = microcontroller.MicrocontrollerSensors
        .Select(microcontrollerSensor => _mapper.Map<SensorDto>(microcontrollerSensor));

      return result;
    }

    private async Task<Microcontroller> TryGetMicrocontroller(Guid microcontrollerID, string requestedByEmail)
    {
      Microcontroller microcontroller;

      bool administratorClaim = await _userManager.IsAdministrator(requestedByEmail);

      if (administratorClaim)
      {
        microcontroller = await _unitOfWork.Microcontrollers.FindAsync(microcontrollerID);
      }
      else
      {
        var user = await _userManager.FindByEmailAsync(requestedByEmail);

        if (user is null)
        {
          throw new AppException(String.Format(Messages.NotFoundErrorString, _user),
            HttpStatusCode.NotFound);
        }

        microcontroller = user.Microcontrollers.FirstOrDefault(mc => mc.ID == microcontrollerID);
      }

      return microcontroller;
    }
  }
}
