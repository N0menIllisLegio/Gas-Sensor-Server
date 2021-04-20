using Gss.Core.Interfaces;
using Gss.Core.Interfaces.Services;

namespace Gss.Core.Services
{
  public class SensorsDataService: ISensorsDataService
  {
    readonly IUnitOfWork _unitOfWork;

    public SensorsDataService(IUnitOfWork unitOfWork)
    {
      _unitOfWork = unitOfWork;
    }
  }
}
