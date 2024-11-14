using System;
using System.Threading.Tasks;
using Gss.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace Gss.Core.Interfaces
{
  public interface IUnitOfWork: IDisposable
  {
    IMicrocontrollersRepository Microcontrollers { get; }
    IRefreshTokensRepository RefreshTokens { get; }
    ISensorsRepository Sensors { get; }
    ISensorsTypesRepository SensorsTypes { get; }
    ISensorsDataRepository SensorsData { get; }

    Task<bool> SaveAsync();
    IDbContextTransaction BeginTransaction();
  }
}
