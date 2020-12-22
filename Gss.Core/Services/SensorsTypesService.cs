using Gss.Core.Interfaces;

namespace Gss.Core.Services
{
  public class SensorsTypesService : ISensorsTypesService
  {
    private readonly ISensrosTypesRepository _sensrosTypesRepository;
    public SensorsTypesService(ISensrosTypesRepository sensrosTypesRepository)
    {
      _sensrosTypesRepository = sensrosTypesRepository;
    }
  }
}
