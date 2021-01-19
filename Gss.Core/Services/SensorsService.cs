using Gss.Core.Interfaces;
using Gss.Core.Interfaces.Services;

namespace Gss.Core.Services
{
  public class SensorsService : ISensorsService
  {
    private readonly IUnitOfWork _unitOfWork;

    public SensorsService(IUnitOfWork unitOfWork)
    {
      _unitOfWork = unitOfWork;
    }
  }
}
