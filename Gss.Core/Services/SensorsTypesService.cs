using Gss.Core.Interfaces;

namespace Gss.Core.Services
{
  public class SensorsTypesService : ISensorsTypesService
  {
    private readonly ISensorsTypesRepository _sensrosTypesRepository;
    public SensorsTypesService(ISensorsTypesRepository sensrosTypesRepository)
    {
      _sensrosTypesRepository = sensrosTypesRepository;
    }
  }
}
