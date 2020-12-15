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
  public class MicrocontrollerService: IMicrocontrollerService
  {
    private readonly IMicrocontrollerRepository _microcontrollerRepository;
    private readonly UserManager _userManager;
    public MicrocontrollerService(IMicrocontrollerRepository microcontrollerRepository, UserManager userManager)
    {
      _microcontrollerRepository = microcontrollerRepository;
      _userManager = userManager;
    }

    public async Task<Response<Microcontroller>> AddMicrocontroller(CreateMicrocontrollerDto dto, string ownerEmail)
    {
      var user = await _userManager.FindByEmailAsync(ownerEmail);

      if (user is null)
      {
        return new Response<Microcontroller>()
          .AddErrors(String.Format(Messages.NotFoundErrorString, "User"));
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
        return new Response<Microcontroller>(microcontroller);
      }

      return new Response<Microcontroller>()
        .AddErrors(String.Format(Messages.CreationFailedErrorString, "Microcontroller"));
    }

    public async Task<Response<Microcontroller>> UpdateMicrocontroller(UpdateMicrocontrollerDto dto,
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
          return new Response<Microcontroller>()
            .AddErrors(String.Format(Messages.NotFoundErrorString, "User"));
        }

        microcontroller = user.Microcontrollers
          .FirstOrDefault(mc => mc.ID == microcontrollerID);
      }

      if (microcontroller is null)
      {
        return new Response<Microcontroller>()
          .AddErrors(String.Format(Messages.NotFoundErrorString, "Microcontoller"));
      }

      microcontroller.Name = dto.Name;
      microcontroller.Latitude = dto.Latitude;
      microcontroller.Longitude = dto.Longitude;
      microcontroller.Public = dto.Public;

      if (await _microcontrollerRepository.SaveAsync())
      {
        return new Response<Microcontroller>(microcontroller);
      }

      return new Response<Microcontroller>()
        .AddErrors(String.Format(Messages.UpdateFailedErrorString, "Microcontroller"));
    }

    public Task<Response<Microcontroller>> ChangeMicrocontrollerOwner(string microcontrollerID,
      string newOwnerID)
    {
      throw new NotImplementedException();
    }

    public async Task<Response<Microcontroller>> DeleteMicrocontroller(string microcontrollerID, string ownerEmail)
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
          return new Response<Microcontroller>()
            .AddErrors(String.Format(Messages.NotFoundErrorString, "User"));
        }

        microcontroller = user.Microcontrollers
          .FirstOrDefault(mc => mc.ID == microcontrollerGuid);
      }

      if (microcontroller is null)
      {
        return new Response<Microcontroller>()
          .AddErrors(String.Format(Messages.NotFoundErrorString, "Microcontoller"));
      }

      microcontroller = _microcontrollerRepository.DeleteMicrocontroller(microcontroller);

      if (await _microcontrollerRepository.SaveAsync())
      {
        return new Response<Microcontroller>(microcontroller);
      }

      return new Response<Microcontroller>()
        .AddErrors(String.Format(Messages.DeletionFailedErrorString, "Microcontroller"));
    }

    public async Task<Response<Microcontroller>> GetMicrocontroller(string microcontrollerID)
    {
      var microcontrollerGuid = Guid.Parse(microcontrollerID);
      var microcontroller = await _microcontrollerRepository.GetMicrocontrollerAsync(microcontrollerGuid);

      if (microcontroller is null)
      {
        return new Response<Microcontroller>()
          .AddErrors(String.Format(Messages.NotFoundErrorString, "Microcontroller"));
      }

      return new Response<Microcontroller>(microcontroller);
    }

    public async Task<PagedResponse<Microcontroller>> GetUserMicrocontrollers(string userID,
      int pageNumber, int pageSize,
      SortOrder sortOrder = SortOrder.None, string sortBy = "",
      string filterBy = null, string filter = "")
    {
      var user = await _userManager.FindByIdAsync(userID);

      if (user is null)
      {
        return new PagedResponse<Microcontroller>()
          .AddErrors(String.Format(Messages.NotFoundErrorString, "User"));
      }

      var filterExpression = GetFilter(filterBy, filter);
      var orderExpression = GetOrderer(sortBy);

      var userMicrocontrollers = await user.Microcontrollers.AsQueryable()
        .GetPage(pageNumber, pageSize, sortOrder, orderExpression, filterExpression)
        .AsNoTracking().ToListAsync();

      return new PagedResponse<Microcontroller>(userMicrocontrollers, pageNumber, pageSize)
      {
        TotalRecords = userMicrocontrollers.Count,
        OrderedBy = sortBy,
        SortOrder = sortOrder,
        Filter = filter,
        FilteredBy = filterBy
      };
    }

    public Task<PagedResponse<Microcontroller>> GetAllMicrocontrollers(int pageNumber, int pageSize,
      SortOrder sortOrder = SortOrder.None, string sortBy = "",
      string filterBy = null, string filter = "")
    {
      throw new NotImplementedException();
    }

    public Task<PagedResponse<Microcontroller>> GetPublicMicrocontrollers(int pageNumber, int pageSize,
      SortOrder sortOrder = SortOrder.None, string sortBy = "",
      string filterBy = null, string filter = "")
    {
      throw new NotImplementedException();
    }

    private Expression<Func<Microcontroller, bool>> GetFilter(string filterBy, string filter)
    {
      return filterBy switch
      {
        "PUBLIC" => (microcontroller) => microcontroller.Public.ToString().ToUpper() == filter.ToUpper(),
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
