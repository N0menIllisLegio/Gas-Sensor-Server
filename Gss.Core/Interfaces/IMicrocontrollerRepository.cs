﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Gss.Core.Entities;
using Gss.Core.Enums;

namespace Gss.Core.Interfaces
{
  public interface IMicrocontrollerRepository
  {
    Task<List<Microcontroller>> GetMicrocontrollersAsync(int pageSize, int pageNumber,
      Expression<Func<Microcontroller, bool>> filter = null,
      SortOrder sortOrder = SortOrder.None, Expression<Func<Microcontroller, object>> orderBy = null,
      bool notTracking = false);

    Task<Microcontroller> GetMicrocontrollerAsync(Guid microcontrollerID);

    Microcontroller AddMicrocontroller(Microcontroller microcontroller, bool generateID = true);

    Microcontroller UpdateMicrocontroller(Microcontroller microcontroller);

    Microcontroller DeleteMicrocontroller(Microcontroller microcontroller);

    Task<bool> SaveAsync();
  }
}
