using Gss.Core.Interfaces;

namespace Gss.Core.Services
{
  public class SensorsService : ISensorsService
  {
    private readonly ISensorsRepository _sensorsRepository;

    public SensorsService(ISensorsRepository sensorsRepository)
    {
      _sensorsRepository = sensorsRepository;
    }
  }
}
