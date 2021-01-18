using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace Gss.Core.Interfaces
{
  public interface IUnitOfWork : IDisposable
  {
    IAzureFilesRepository AzureFiles { get; }
    IMicrocontrollersRepository Microcontrollers { get; }
    IRefreshTokensRepository RefreshTokens { get; }
    ISensorsRepository Sensors { get; }
    ISensorsTypesRepository SensorsTypes { get; }

    Task<bool> SaveAsync();
    IDbContextTransaction BeginTransaction();
  }
}
