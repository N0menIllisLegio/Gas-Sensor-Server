using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Gss.Core.DTOs;
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
    private readonly IMicrocontrollersRepository _microcontrollerRepository;
    private readonly UserManager _userManager;
    public MicrocontrollersService(IMicrocontrollersRepository microcontrollerRepository, UserManager userManager)
    {
      _microcontrollerRepository = microcontrollerRepository;
      _userManager = userManager;
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

      microcontroller = _microcontrollerRepository.AddMicrocontroller(microcontroller);

      if (await _microcontrollerRepository.SaveAsync())
      {
        return new ServiceResultDto<Microcontroller>(microcontroller);
      }

      return new ServiceResultDto<Microcontroller>()
        .AddError(Messages.CreationFailedErrorString, "Microcontroller");
    }

    public async Task<ServiceResultDto<Microcontroller>> UpdateMicrocontroller(UpdateMicrocontrollerDto dto,
      string ownerEmail)
    {
      var microcontrollerID = Guid.Parse(dto.ID);
      Microcontroller microcontroller;

      if (await _userManager.IsAdministrator(ownerEmail))
      {
        microcontroller = await _microcontrollerRepository
          .GetMicrocontrollerAsync(microcontrollerID);
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

      microcontroller.Name = dto.Name;
      microcontroller.Latitude = dto.Latitude;
      microcontroller.Longitude = dto.Longitude;
      microcontroller.Public = dto.Public;

      if (await _microcontrollerRepository.SaveAsync())
      {
        return new ServiceResultDto<Microcontroller>(microcontroller);
      }

      return new ServiceResultDto<Microcontroller>()
        .AddError(Messages.UpdateFailedErrorString, "Microcontroller");
    }

    public async Task<ServiceResultDto<Microcontroller>> ChangeMicrocontrollerOwner(string microcontrollerID,
      string newOwnerID)
    {
      var microcontrollerGuid = Guid.Parse(microcontrollerID);
      var microcontroller = await _microcontrollerRepository.GetMicrocontrollerAsync(microcontrollerGuid);

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

      if (await _microcontrollerRepository.SaveAsync())
      {
        return new ServiceResultDto<Microcontroller>(microcontroller);
      }

      return new ServiceResultDto<Microcontroller>()
        .AddError(Messages.UpdateFailedErrorString, "Microcontroller");
    }

    public async Task<ServiceResultDto<Microcontroller>> DeleteMicrocontroller(string microcontrollerID, string ownerEmail)
    {
      var microcontrollerGuid = Guid.Parse(microcontrollerID);
      Microcontroller microcontroller;

      if (await _userManager.IsAdministrator(ownerEmail))
      {
        microcontroller = await _microcontrollerRepository.GetMicrocontrollerAsync(microcontrollerGuid);
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
          .FirstOrDefault(mc => mc.ID == microcontrollerGuid);
      }

      if (microcontroller is null)
      {
        return new ServiceResultDto<Microcontroller>()
          .AddError(Messages.NotFoundErrorString, "Microcontoller");
      }

      microcontroller = _microcontrollerRepository.DeleteMicrocontroller(microcontroller);

      if (await _microcontrollerRepository.SaveAsync())
      {
        return new ServiceResultDto<Microcontroller>(microcontroller);
      }

      return new ServiceResultDto<Microcontroller>()
        .AddError(Messages.DeletionFailedErrorString, "Microcontroller");
    }

    public async Task<(ServiceResultDto<Microcontroller> result, bool displaySensitiveInfo)> GetMicrocontroller(string microcontrollerID,
      string requestedByEmail)
    {
      var microcontrollerGuid = Guid.Parse(microcontrollerID);
      var microcontroller = await _microcontrollerRepository.GetMicrocontrollerAsync(microcontrollerGuid);

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

    public async Task<(ServiceResultDto<Microcontroller> result, int microcontrollersCount, bool displaySensitiveInfo)> GetUserMicrocontrollers(string userID, string requestedByEmail,
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

      var (microcontrollers, totalQueriedMicrocontrollersCount) = await _microcontrollerRepository
        .GetMicrocontrollersAsync(pageSize, pageNumber, sortOrder, filter, sorter, true);

      return (new ServiceResultDto<Microcontroller>(microcontrollers), totalQueriedMicrocontrollersCount);
    }

    public async Task<(ServiceResultDto<Microcontroller> result, int totalQueriedMicrocontrollersCount)> GetPublicMicrocontrollers(
      int pageNumber, int pageSize,
      SortOrder sortOrder = SortOrder.None, string sortBy = "",
      string filterBy = null, string filterStr = "")
    {
      var sorter = GetOrderer(sortBy);
      var filter = GetFilter(filterBy, filterStr);

      var (microcontrollers, totalQueriedMicrocontrollersCount) = await _microcontrollerRepository
        .GetPublicMicrocontrollersAsync(pageSize, pageNumber, sortOrder, filter, sorter, true);

      return (new ServiceResultDto<Microcontroller>(microcontrollers), totalQueriedMicrocontrollersCount);
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
