using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Gss.Core.Entities;
using Gss.Core.Enums;
using Gss.Core.Helpers;
using Gss.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gss.Infrastructure.Repositories
{
  public class MicrocontrollerRepository: IMicrocontrollerRepository
  {
    private readonly AppDbContext _appDbContext;

    public MicrocontrollerRepository(AppDbContext appDbContext)
    {
      _appDbContext = appDbContext;
    }

    public async Task<List<Microcontroller>> GetMicrocontrollersAsync(int pageSize, int pageNumber,
      SortOrder sortOrder = SortOrder.None,
      Expression<Func<Microcontroller, bool>> filter = null,
      Expression<Func<Microcontroller, object>> sorter = null,
      bool notTracking = false)
    {
      var query = _appDbContext.Microcontrollers
        .AsQueryable().GetPage(pageNumber, pageSize, sortOrder, sorter, filter);

      if (notTracking)
      {
        query = query.AsNoTracking();
      }

      return await query.ToListAsync();
    }

    public async Task<Microcontroller> GetMicrocontrollerAsync(Guid microcontrollerID)
    {
      return await _appDbContext.Microcontrollers.FindAsync(microcontrollerID);
    }

    public Microcontroller AddMicrocontroller(Microcontroller microcontroller, bool generateID = true)
    {
      if (generateID)
      {
        microcontroller.ID = Guid.NewGuid();
      }

      return _appDbContext.Microcontrollers.Add(microcontroller).Entity;
    }

    public Microcontroller UpdateMicrocontroller(Microcontroller microcontroller)
    {
      return _appDbContext.Microcontrollers.Update(microcontroller).Entity;
    }

    public Microcontroller DeleteMicrocontroller(Microcontroller microcontroller)
    {
      return _appDbContext.Microcontrollers.Remove(microcontroller).Entity;
    }

    public async Task<bool> SaveAsync()
    {
      try
      {
        await _appDbContext.SaveChangesAsync();

        return true;
      }
      catch
      {
        return false;
      }
    }
  }
}
