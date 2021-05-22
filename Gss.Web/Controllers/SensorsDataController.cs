using System.Collections.Generic;
using System.Threading.Tasks;
using Gss.Core.DTOs;
using Gss.Core.DTOs.SensorData;
using Gss.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

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

    [HttpPost]
    [SwaggerOperation(Description = "Gets sensor's data.")]
    [SwaggerResponse(200, type: typeof(Response<List<SensorDataDto>>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> GetSensorData([FromBody] RequestSensorDataDto requestSensorDataDto)
    {
      var sensorData = await _sensorsDataService.GetSensorData(User.Identity.Name, requestSensorDataDto);

      return Ok(new Response<List<SensorDataDto>>(sensorData));
    }
  }
}
