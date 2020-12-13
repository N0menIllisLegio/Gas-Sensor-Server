using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Gss.Core.DTOs;
using Gss.Core.Entities;
using Gss.Core.Enums;
using Gss.Core.Interfaces;
using Gss.Core.Resources;

namespace Gss.Core.Services
{
  public class MicrocontrollerService: IMicrocontrollerService
  {
    private readonly IMicrocontrollerRepository _microcontrollerRepository;
    public MicrocontrollerService(IMicrocontrollerRepository microcontrollerRepository)
    {
      _microcontrollerRepository = microcontrollerRepository;
    }

    public Task<List<Microcontroller>> GetAllPublicControllers(int pageSize, int pageNumber,
      SortOrder sortOrder = SortOrder.None, Expression<Func<Microcontroller, object>> sortBy = null)
    {
      return _microcontrollerRepository.GetMicrocontrollersAsync(pageSize, pageNumber, (mc) => mc.Public, sortOrder, sortBy);
    }

    public Task<Microcontroller> GetMicrocontroller(Guid microcontrollerID)
    {
      return _microcontrollerRepository.GetMicrocontrollerAsync(microcontrollerID);
    }

    public async Task<Response<Microcontroller>> AddMicrocontroller(CreateMicrocontrollerDto dto, User user)
    {
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
      User user)
    {
      var microcontrollerID = Guid.Parse(dto.ID);
      var microcontroller = user.Microcontrollers
        .FirstOrDefault(mc => mc.ID == microcontrollerID);

      if (microcontroller is null)
      {
        return new Response<Microcontroller>()
          .AddErrors(String.Format(Messages.NotFoundErrorString, "Microcontoller"));
      }

      return await UpdateMicrocontroller(dto, microcontroller);
    }

    public async Task<Response<Microcontroller>> UpdateMicrocontroller(UpdateMicrocontrollerDto dto)
    {
      var microcontrollerID = Guid.Parse(dto.ID);
      var microcontroller = await _microcontrollerRepository.GetMicrocontrollerAsync(microcontrollerID);

      if (microcontroller is null)
      {
        return new Response<Microcontroller>()
          .AddErrors(String.Format(Messages.NotFoundErrorString, "Microcontoller"));
      }

      return await UpdateMicrocontroller(dto, microcontroller);
    }

    public async Task<Response<Microcontroller>> DeleteMicrocontroller(string microcontrollerID, User user)
    {
      var microcontrollerGuid = Guid.Parse(microcontrollerID);
      var microcontroller = user.Microcontrollers
        .FirstOrDefault(mc => mc.ID == microcontrollerGuid);

      if (microcontroller is null)
      {
        return new Response<Microcontroller>()
          .AddErrors(String.Format(Messages.NotFoundErrorString, "Microcontoller"));
      }

      return await DeleteMicrocontroller(microcontroller);
    }

    public async Task<Response<Microcontroller>> DeleteMicrocontroller(string microcontrollerID)
    {
      var microcontrollerGuid = Guid.Parse(microcontrollerID);
      var microcontroller = await _microcontrollerRepository.GetMicrocontrollerAsync(microcontrollerGuid);

      if (microcontroller is null)
      {
        return new Response<Microcontroller>()
          .AddErrors(String.Format(Messages.NotFoundErrorString, "Microcontoller"));
      }

      return await DeleteMicrocontroller(microcontroller);
    }

    private async Task<Response<Microcontroller>> DeleteMicrocontroller(Microcontroller microcontroller)
    {
      microcontroller = _microcontrollerRepository.DeleteMicrocontroller(microcontroller);

      if (await _microcontrollerRepository.SaveAsync())
      {
        return new Response<Microcontroller>(microcontroller);
      }

      return new Response<Microcontroller>()
        .AddErrors(String.Format(Messages.DeletionFailedErrorString, "Microcontroller"));
    }

    private async Task<Response<Microcontroller>> UpdateMicrocontroller(UpdateMicrocontrollerDto dto,
      Microcontroller microcontroller)
    {
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
  }
}
