using Gss.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gss.Web.Controllers
{
  [Route("api/[controller]/[action]")]
  [ApiController]
  public class SensorsDataController: ControllerBase
  {
    private readonly ISensorsDataService _sensorsDataService;
    public SensorsDataController(ISensorsDataService sensorsDataService)
    {
      _sensorsDataService = sensorsDataService;
    }
  }
}
