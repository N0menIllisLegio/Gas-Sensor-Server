using Gss.Core.DTOs;
using Gss.Core.DTOs.Sensor;
using Gss.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Gss.Web.Controllers;

[Authorize]
[Route("api/[controller]/[action]")]
[ApiController]
public class SensorsController : ControllerBase
{
  private readonly ISensorsService _sensorsService;

  public SensorsController(ISensorsService sensorsService)
  {
    _sensorsService = sensorsService;
  }

  [HttpPost]
  [SwaggerOperation("Authorized", "Gets all sensors.")]
  [SwaggerResponse(200, type: typeof(PagedResultDto<SensorDto>))]
  [SwaggerResponse(400, type: typeof(ProblemDetails))]
  [SwaggerResponse(401, type: typeof(ProblemDetails))]
  [SwaggerResponse(422, type: typeof(ProblemDetails))]
  public async Task<IActionResult> GetAllSensors([FromBody] PagedInfoDto pagedRequest)
  {
    var pagedResultDto = await _sensorsService.GetAllSensorsAsync(pagedRequest);

    return Ok(pagedResultDto);
  }

  [Authorize(Roles = "Administrator")]
  [HttpGet("{id}")]
  [SwaggerOperation("Administrator Only", "Gets sensor by id.")]
  [SwaggerResponse(200, type: typeof(SensorDto))]
  [SwaggerResponse(401, type: typeof(ProblemDetails))]
  [SwaggerResponse(403, type: typeof(ProblemDetails))]
  [SwaggerResponse(404, type: typeof(ProblemDetails))]
  public async Task<IActionResult> GetSensor([FromRoute] Guid id)
  {
    var sensorDto = await _sensorsService.GetSensorAsync(id);

    return Ok(sensorDto);
  }

  [Authorize(Roles = "Administrator")]
  [HttpPost]
  [SwaggerOperation("Administrator Only", "Creates sensor.")]
  [SwaggerResponse(201, type: typeof(SensorDto))]
  [SwaggerResponse(400, type: typeof(ProblemDetails))]
  [SwaggerResponse(401, type: typeof(ProblemDetails))]
  [SwaggerResponse(403, type: typeof(ProblemDetails))]
  [SwaggerResponse(422, type: typeof(ProblemDetails))]
  public async Task<IActionResult> Create([FromBody] CreateSensorDto dto)
  {
    var sensorDto = await _sensorsService.CreateSensorAsync(dto);

    return CreatedAtAction(nameof(GetSensor), new { id = sensorDto.Id }, sensorDto);
  }

  [Authorize(Roles = "Administrator")]
  [HttpPut("{id}")]
  [SwaggerOperation("Administrator Only", "Updates sensor.")]
  [SwaggerResponse(200)]
  [SwaggerResponse(400, type: typeof(ProblemDetails))]
  [SwaggerResponse(401, type: typeof(ProblemDetails))]
  [SwaggerResponse(403, type: typeof(ProblemDetails))]
  [SwaggerResponse(404, type: typeof(ProblemDetails))]
  public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateSensorDto updateSensorDto)
  {
    await _sensorsService.UpdateSensorAsync(id, updateSensorDto);

    return Ok();
  }

  [Authorize(Roles = "Administrator")]
  [HttpDelete("{id}")]
  [SwaggerOperation("Administrator Only", "Deletes sensor.")]
  [SwaggerResponse(200)]
  public async Task<IActionResult> Delete([FromRoute] Guid id)
  {
    await _sensorsService.DeleteSensorAsync(id);

    return Ok();
  }
}