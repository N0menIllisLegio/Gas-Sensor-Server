using Gss.Core.Interfaces.Repositories;

namespace Gss.Core.Interfaces;

public interface IUnitOfWork: IDisposable
{
  IMicrocontrollersRepository Microcontrollers { get; }
  ISensorsRepository Sensors { get; }
  ISensorsTypesRepository SensorsTypes { get; }
  ISensorsDataRepository SensorsData { get; }

  Task<bool> SaveAsync();
}