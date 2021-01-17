using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Gss.Core.DTOs;
using Gss.Core.DTOs.Microcontroller;
using Gss.Core.Entities;
using Gss.Core.Exceptions;
using Gss.Core.Helpers;
using Gss.Core.Interfaces;
using Gss.Core.Resources;

namespace Gss.Core.Services
{
  public class MicrocontrollersService : IMicrocontrollersService
  {
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
        microcontroller => new { microcontroller.Name, microcontroller.LastResponseTime }, mc => mc.Public);

      return pagedResultDto.Convert<MicrocontrollerDto>(_mapper);
    }

    public async Task<PagedResultDto<MicrocontrollerDto>> GetAllMicrocontrollersAsync(PagedInfoDto pagedInfo)
    {
      var pagedResultDto = await _unitOfWork.Microcontrollers.GetPagedResultAsync(pagedInfo,
        microcontroller => new { microcontroller.Name, microcontroller.LastResponseTime });

      return pagedResultDto.Convert<MicrocontrollerDto>(_mapper);
    }

    public async Task<PagedResultDto<MicrocontrollerDto>> GetUserMicrocontrollersAsync(Guid userID, string requestedByEmail, PagedInfoDto pagedInfo)
    {
      var user = await _userManager.FindByIdAsync(userID);

      if (user is null)
      {
        throw new AppException(String.Format(Messages.NotFoundErrorString, "User"),
          HttpStatusCode.NotFound);
      }

      var requestedBy = await _userManager.FindByEmailAsync(requestedByEmail);
      bool administratorClaim = await _userManager.IsAdministrator(requestedByEmail);
      var pagedResultDto = user == requestedBy || administratorClaim
        ? await _unitOfWork.Microcontrollers.GetPagedResultAsync(pagedInfo,
          microcontroller => new { microcontroller.Name, microcontroller.LastResponseTime },
          mc => mc.Owner.ID == user.ID)
        : await _unitOfWork.Microcontrollers.GetPagedResultAsync(pagedInfo,
          microcontroller => new { microcontroller.Name, microcontroller.LastResponseTime },
          mc => mc.Owner.ID == user.ID && mc.Public);

      return pagedResultDto.Convert<MicrocontrollerDto>(_mapper);
    }

    public async Task<MicrocontrollerDto> GetMicrocontrollerAsync(Guid microcontrollerID,
      string requestedByEmail)
    {
      var microcontroller = await _unitOfWork.Microcontrollers.FindAsync(microcontrollerID);

      if (microcontroller is null)
      {
        throw new AppException(String.Format(Messages.NotFoundErrorString, "Microcontroller"),
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

    public async Task<MicrocontrollerDto> AddMicrocontrollerAsync(CreateMicrocontrollerDto createMicrocontrollerDto,
      string ownerEmail)
    {
      var user = await _userManager.FindByEmailAsync(ownerEmail);

      if (user is null)
      {
        throw new AppException(String.Format(Messages.NotFoundErrorString, "User"),
          HttpStatusCode.NotFound);
      }

      var microcontroller = _mapper.Map<Microcontroller>(createMicrocontrollerDto);
      microcontroller.Owner = user;
      microcontroller = _unitOfWork.Microcontrollers.Add(microcontroller);

      bool success = await _unitOfWork.SaveAsync();

      if (!success)
      {
        throw new AppException(String.Format(Messages.CreationFailedErrorString, "Microcontroller"),
          HttpStatusCode.BadRequest);
      }

      return _mapper.Map<MicrocontrollerDto>(microcontroller);
    }

    public async Task<MicrocontrollerDto> UpdateMicrocontrollerAsync(UpdateMicrocontrollerDto updateMicrocontrollerDto,
      string ownerEmail)
    {
      Microcontroller microcontroller;

      bool administratorClaim = await _userManager.IsAdministrator(ownerEmail);

      if (await _userManager.IsAdministrator(ownerEmail))
      {
        microcontroller = await _unitOfWork.Microcontrollers.FindAsync(updateMicrocontrollerDto.ID);
      }
      else
      {
        var user = await _userManager.FindByEmailAsync(ownerEmail);

        if (user is null)
        {
          throw new AppException(String.Format(Messages.NotFoundErrorString, "User"),
            HttpStatusCode.NotFound);
        }

        microcontroller = user.Microcontrollers.FirstOrDefault(mc => mc.ID == updateMicrocontrollerDto.ID);
      }

      if (microcontroller is null)
      {
        throw new AppException(String.Format(Messages.NotFoundErrorString, "Microcontoller"),
          HttpStatusCode.NotFound);
      }

      microcontroller.Name = updateMicrocontrollerDto.Name;
      microcontroller.Latitude = updateMicrocontrollerDto.Latitude;
      microcontroller.Longitude = updateMicrocontrollerDto.Longitude;
      microcontroller.Public = updateMicrocontrollerDto.Public;

      bool success = await _unitOfWork.SaveAsync();

      if (!success)
      {
        throw new AppException(String.Format(Messages.UpdateFailedErrorString, "Microcontoller"),
          HttpStatusCode.BadRequest);
      }

      return _mapper.Map<MicrocontrollerDto>(microcontroller);
    }

    public async Task<MicrocontrollerDto> ChangeMicrocontrollerOwnerAsync(Guid microcontrollerID,
      Guid newOwnerID)
    {
      var microcontroller = await _unitOfWork.Microcontrollers.FindAsync(microcontrollerID);

      if (microcontroller is null)
      {
        throw new AppException(String.Format(Messages.NotFoundErrorString, "Microcontoller"),
          HttpStatusCode.NotFound);
      }

      var user = await _userManager.FindByIdAsync(newOwnerID);

      if (user is null)
      {
        throw new AppException(String.Format(Messages.NotFoundErrorString, "User"),
          HttpStatusCode.NotFound);
      }

      microcontroller.Owner = user;

      bool success = await _unitOfWork.SaveAsync();

      if (!success)
      {
        throw new AppException(String.Format(Messages.UpdateFailedErrorString, "Microcontoller"),
          HttpStatusCode.BadRequest);
      }

      return _mapper.Map<MicrocontrollerDto>(microcontroller);
    }

    public async Task<MicrocontrollerDto> DeleteMicrocontrollerAsync(Guid microcontrollerID, string ownerEmail)
    {
      Microcontroller microcontroller;

      bool administratorClaim = await _userManager.IsAdministrator(ownerEmail);

      if (administratorClaim)
      {
        microcontroller = await _unitOfWork.Microcontrollers.FindAsync(microcontrollerID);
      }
      else
      {
        var user = await _userManager.FindByEmailAsync(ownerEmail);

        if (user is null)
        {
          throw new AppException(String.Format(Messages.NotFoundErrorString, "User"),
            HttpStatusCode.NotFound);
        }

        microcontroller = user.Microcontrollers.FirstOrDefault(mc => mc.ID == microcontrollerID);
      }

      if (microcontroller is null)
      {
        throw new AppException(String.Format(Messages.NotFoundErrorString, "Microcontoller"),
          HttpStatusCode.NotFound);
      }

      microcontroller = _unitOfWork.Microcontrollers.Remove(microcontroller);

      bool success = await _unitOfWork.SaveAsync();

      if (!success)
      {
        throw new AppException(String.Format(Messages.DeletionFailedErrorString, "Microcontoller"),
          HttpStatusCode.BadRequest);
      }

      return _mapper.Map<MicrocontrollerDto>(microcontroller);
    }

    private Expression<Func<Microcontroller, bool>> GetFilter(string filterBy, string filter)
    {
      return filterBy switch
      {
        "PUBLIC" => filter.ToUpper() switch
          {
            "TRUE" => (microcontroller) => microcontroller.Public,
            _ => (microcontroller) => !microcontroller.Public
          },
        "IPADDRESS" => (microcontroller) => microcontroller.IPAddress.Contains(filter),
        _ => (microcontroller) => microcontroller.Name.Contains(filter)
      };
    }

    private Expression<Func<Microcontroller, object>> GetOrderer(string sortBy)
    {
      return sortBy switch
      {
        "PUBLIC" => (microcontroller) => microcontroller.Public,
        "IPADDRESS" => (microcontroller) => microcontroller.IPAddress,
        "LASTRESPONSETIME" => (microcontroller) => microcontroller.LastResponseTime,
        "LATITUDE" => (microcontroller) => microcontroller.Latitude,
        "LONGITUDE" => (microcontroller) => microcontroller.Longitude,
        _ => (microcontroller) => microcontroller.Name
      };
    }
  }
}
