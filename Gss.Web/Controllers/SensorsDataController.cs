using Gss.Core.DTOs.SensorData;
using Gss.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Gss.Web.Controllers;

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
  [SwaggerResponse(200, type: typeof(List<SensorDataDto>))]
  [SwaggerResponse(400, type: typeof(ProblemDetails))]
  [SwaggerResponse(404, type: typeof(ProblemDetails))]
  public async Task<IActionResult> GetSensorData([FromBody] RequestSensorDataDto requestSensorDataDto, CancellationToken cancellationToken)
  {
    var sensorData = await _sensorsDataService.GetSensorDataAsync(requestSensorDataDto, cancellationToken);

    return Ok(sensorData);
  }
}