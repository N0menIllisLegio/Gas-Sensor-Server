using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Gss.Core.Entities;
using Gss.Core.Enums;
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
      Expression<Func<Microcontroller, bool>> filter = null,
      SortOrder sortOrder = SortOrder.None, Expression<Func<Microcontroller, object>> orderBy = null,
      bool notTracking = false)
    {
      var query = _appDbContext.Microcontrollers.Where(filter ?? ((_) => true));

      if (sortOrder == SortOrder.Ascendind && orderBy is not null)
      {
        query = query.OrderBy(orderBy);
      }
      else if (sortOrder == SortOrder.Descending && orderBy is not null)
      {
        query = query.OrderByDescending(orderBy);
      }

      if (notTracking)
      {
        query = query.AsNoTracking();
      }

      return await query.Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();
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
