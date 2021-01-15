﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Gss.Core.DTOs;
using Gss.Core.DTOs.Microcontroller;
using Gss.Core.Entities;
using Gss.Core.Enums;
using Gss.Core.Helpers;
using Gss.Core.Interfaces;
using Gss.Core.Resources;
using Microsoft.EntityFrameworkCore;

namespace Gss.Core.Services
{
  public class MicrocontrollersService : IMicrocontrollersService
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager _userManager;
    public MicrocontrollersService(IUnitOfWork unitOfWork, UserManager userManager)
    {
      _unitOfWork = unitOfWork;
      _userManager = userManager;
    }

    public async Task<(ServiceResultDto<Microcontroller> result, int microcontrollersCount, bool displaySensitiveInfo)> GetUserMicrocontrollers(
      string userID, string requestedByEmail,
      int pageNumber, int pageSize,
      SortOrder sortOrder = SortOrder.None, string sortBy = "",
      string filterBy = null, string filterStr = "")
    {
      var requestedBy = await _userManager.FindByEmailAsync(requestedByEmail);
      var user = await _userManager.FindByIdAsync(userID);

      if (user is null)
      {
        return (new ServiceResultDto<Microcontroller>()
          .AddError(Messages.NotFoundErrorString, "User"), 0, false);
      }

      var sorter = GetOrderer(sortBy);
      var filter = GetFilter(filterBy, filterStr);
      var microcontrollersQuery = user.Microcontrollers.AsQueryable();

      bool administratorClaim = await _userManager.IsAdministrator(requestedByEmail);

      var (pagedMicrocontrollersQuery, totalMicrocontrollersQuery) = user == requestedBy || administratorClaim
        ? microcontrollersQuery
          .GetPage(pageNumber, pageSize, sortOrder, sorter, filter)
        : microcontrollersQuery
          .Where(mc => mc.Public)
          .GetPage(pageNumber, pageSize, sortOrder, sorter, filter);

      var microcontrollers = await pagedMicrocontrollersQuery.ToListAsync();
      int totalQueriedMicrocontrollersCount = await totalMicrocontrollersQuery.CountAsync();

      return (new ServiceResultDto<Microcontroller>(microcontrollers), totalQueriedMicrocontrollersCount, user == requestedBy || administratorClaim);
    }

    public async Task<(ServiceResultDto<Microcontroller> result, int totalQueriedMicrocontrollersCount)> GetAllMicrocontrollers(
      int pageNumber, int pageSize,
      SortOrder sortOrder = SortOrder.None, string sortBy = "",
      string filterBy = null, string filterStr = "")
    {
      var sorter = GetOrderer(sortBy);
      var filter = GetFilter(filterBy, filterStr);

      var (microcontrollers, totalQueriedMicrocontrollersCount) = await _unitOfWork.Microcontrollers
        .GetMicrocontrollersAsync(pageNumber, pageSize, sortOrder, filter, sorter, true);

      return (new ServiceResultDto<Microcontroller>(microcontrollers), totalQueriedMicrocontrollersCount);
    }

    public async Task<(ServiceResultDto<Microcontroller> result, int totalQueriedMicrocontrollersCount)> GetPublicMicrocontrollers(
      int pageNumber, int pageSize,
      SortOrder sortOrder = SortOrder.None, string sortBy = "",
      string filterBy = null, string filterStr = "")
    {
      var sorter = GetOrderer(sortBy);
      var filter = GetFilter(filterBy, filterStr);

      var (microcontrollers, totalQueriedMicrocontrollersCount) = await _unitOfWork.Microcontrollers
        .GetPublicMicrocontrollersAsync(pageNumber, pageSize, sortOrder, filter, sorter, true);

      return (new ServiceResultDto<Microcontroller>(microcontrollers), totalQueriedMicrocontrollersCount);
    }

    public async Task<(ServiceResultDto<Microcontroller> result, bool displaySensitiveInfo)> GetMicrocontroller(Guid microcontrollerID,
      string requestedByEmail)
    {
      var microcontroller = await _unitOfWork.Microcontrollers.GetMicrocontrollerAsync(microcontrollerID);

      if (microcontroller is null)
      {
        return (new ServiceResultDto<Microcontroller>()
          .AddError(Messages.NotFoundErrorString, "Microcontroller"), false);
      }

      if (microcontroller.Owner.Email.Equals(requestedByEmail))
      {
        return (new ServiceResultDto<Microcontroller>(microcontroller), true);
      }

      if (microcontroller.Public)
      {
        return (new ServiceResultDto<Microcontroller>(microcontroller), false);
      }

      bool administratorClaim = await _userManager.IsAdministrator(requestedByEmail);

      if (administratorClaim)
      {
        return (new ServiceResultDto<Microcontroller>(microcontroller), true);
      }

      return (new ServiceResultDto<Microcontroller>()
        .AddError(Messages.AccessDeniedErrorString), false);
    }

    public async Task<ServiceResultDto<Microcontroller>> AddMicrocontroller(CreateMicrocontrollerDto dto, string ownerEmail)
    {
      var user = await _userManager.FindByEmailAsync(ownerEmail);

      if (user is null)
      {
        return new ServiceResultDto<Microcontroller>()
          .AddError(Messages.NotFoundErrorString, "User");
      }

      var microcontroller = new Microcontroller
      {
        Name = dto.Name,
        Latitude = dto.Latitude,
        Longitude = dto.Longitude,
        Public = dto.Public,
        Owner = user
      };

      microcontroller = _unitOfWork.Microcontrollers.AddMicrocontroller(microcontroller);

      if (await _unitOfWork.SaveAsync())
      {
        return new ServiceResultDto<Microcontroller>(microcontroller);
      }

      return new ServiceResultDto<Microcontroller>()
        .AddError(Messages.CreationFailedErrorString, "Microcontroller");
    }

    public async Task<ServiceResultDto<Microcontroller>> UpdateMicrocontroller(UpdateMicrocontrollerDto dto,
      string ownerEmail)
    {
      Microcontroller microcontroller;

      if (await _userManager.IsAdministrator(ownerEmail))
      {
        microcontroller = await _unitOfWork.Microcontrollers
          .GetMicrocontrollerAsync(dto.ID);
      }
      else
      {
        var user = await _userManager.FindByEmailAsync(ownerEmail);

        if (user is null)
        {
          return new ServiceResultDto<Microcontroller>()
            .AddError(Messages.NotFoundErrorString, "User");
        }

        microcontroller = user.Microcontrollers
          .FirstOrDefault(mc => mc.ID == dto.ID);
      }

      if (microcontroller is null)
      {
        return new ServiceResultDto<Microcontroller>()
          .AddError(Messages.NotFoundErrorString, "Microcontoller");
      }

      microcontroller.Name = dto.Name;
      microcontroller.Latitude = dto.Latitude;
      microcontroller.Longitude = dto.Longitude;
      microcontroller.Public = dto.Public;

      if (await _unitOfWork.SaveAsync())
      {
        return new ServiceResultDto<Microcontroller>(microcontroller);
      }

      return new ServiceResultDto<Microcontroller>()
        .AddError(Messages.UpdateFailedErrorString, "Microcontroller");
    }

    public async Task<ServiceResultDto<Microcontroller>> ChangeMicrocontrollerOwner(Guid microcontrollerID,
      string newOwnerID)
    {
      var microcontroller = await _unitOfWork.Microcontrollers.GetMicrocontrollerAsync(microcontrollerID);

      if (microcontroller is null)
      {
        return new ServiceResultDto<Microcontroller>()
          .AddError(Messages.NotFoundErrorString, "Microcontroller");
      }

      var user = await _userManager.FindByIdAsync(newOwnerID);

      if (user is null)
      {
        return new ServiceResultDto<Microcontroller>()
          .AddError(Messages.NotFoundErrorString, "User");
      }

      microcontroller.Owner = user;

      if (await _unitOfWork.SaveAsync())
      {
        return new ServiceResultDto<Microcontroller>(microcontroller);
      }

      return new ServiceResultDto<Microcontroller>()
        .AddError(Messages.UpdateFailedErrorString, "Microcontroller");
    }

    public async Task<ServiceResultDto<Microcontroller>> DeleteMicrocontroller(Guid microcontrollerID, string ownerEmail)
    {
      Microcontroller microcontroller;

      if (await _userManager.IsAdministrator(ownerEmail))
      {
        microcontroller = await _unitOfWork.Microcontrollers.GetMicrocontrollerAsync(microcontrollerID);
      }
      else
      {
        var user = await _userManager.FindByEmailAsync(ownerEmail);

        if (user is null)
        {
          return new ServiceResultDto<Microcontroller>()
            .AddError(Messages.NotFoundErrorString, "User");
        }

        microcontroller = user.Microcontrollers
          .FirstOrDefault(mc => mc.ID == microcontrollerID);
      }

      if (microcontroller is null)
      {
        return new ServiceResultDto<Microcontroller>()
          .AddError(Messages.NotFoundErrorString, "Microcontoller");
      }

      microcontroller = _unitOfWork.Microcontrollers.DeleteMicrocontroller(microcontroller);

      if (await _unitOfWork.SaveAsync())
      {
        return new ServiceResultDto<Microcontroller>(microcontroller);
      }

      return new ServiceResultDto<Microcontroller>()
        .AddError(Messages.DeletionFailedErrorString, "Microcontroller");
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
