using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Gss.Core.Entities;
using Gss.Core.Enums;

namespace Gss.Core.Interfaces
{
  public interface IMicrocontrollerRepository
  {
    Task<(List<Microcontroller> microcontrollers, int totalCount)> GetPublicMicrocontrollersAsync(int pageSize, int pageNumber,
      SortOrder sortOrder = SortOrder.None,
      Expression<Func<Microcontroller, bool>> filter = null,
      Expression<Func<Microcontroller, object>> sorter = null,
      bool notTracking = false);

    Task<(List<Microcontroller> microcontrollers, int totalCount)> GetMicrocontrollersAsync(int pageSize, int pageNumber,
      SortOrder sortOrder = SortOrder.None,
      Expression<Func<Microcontroller, bool>> filter = null,
      Expression<Func<Microcontroller, object>> sorter = null,
      bool notTracking = false);

    Task<Microcontroller> GetMicrocontrollerAsync(Guid microcontrollerID);

    Microcontroller AddMicrocontroller(Microcontroller microcontroller, bool generateID = true);

    Microcontroller UpdateMicrocontroller(Microcontroller microcontroller);

    Microcontroller DeleteMicrocontroller(Microcontroller microcontroller);

    Task<bool> SaveAsync();
  }
}
