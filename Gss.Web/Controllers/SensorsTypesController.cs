using Gss.Core.DTOs;
using Gss.Core.DTOs.SensorType;
using Gss.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Gss.Web.Controllers;

[Authorize(Roles = "Administrator")]
[Route("api/[controller]/[action]")]
[ApiController]
public class SensorsTypesController: ControllerBase
{
  private readonly ISensorsTypesService _sensorsTypesService;

  public SensorsTypesController(ISensorsTypesService sensorsTypesService)
  {
    _sensorsTypesService = sensorsTypesService;
  }

  [HttpPost]
  [SwaggerOperation("Administrator Only", "Gets all sensor's types.")]
  [SwaggerResponse(200, type: typeof(PagedResultDto<SensorTypeDto>))]
  [SwaggerResponse(400, type: typeof(ProblemDetails))]
  [SwaggerResponse(422, type: typeof(ProblemDetails))]
  public async Task<IActionResult> GetAllSensorsTypes([FromBody] PagedInfoDto pagedRequest)
  {
    var pagedResult = await _sensorsTypesService.GetAllSensorsTypesAsync(pagedRequest);

    return Ok(pagedResult);
  }

  [HttpGet("{id}")]
  [SwaggerOperation("Administrator Only", "Gets sensor's type by id.")]
  [SwaggerResponse(200, type: typeof(SensorTypeDto))]
  [SwaggerResponse(404, type: typeof(ProblemDetails))]
  public async Task<IActionResult> GetSensorType([FromRoute] Guid id)
  {
    var sensorType = await _sensorsTypesService.GetSensorTypeAsync(id);

    return Ok(sensorType);
  }

  [HttpPost]
  [SwaggerOperation("Administrator Only", "Creates sensor's type.")]
  [SwaggerResponse(201, type: typeof(SensorTypeDto))]
  [SwaggerResponse(400, type: typeof(ProblemDetails))]
  [SwaggerResponse(422, type: typeof(ProblemDetails))]
  public async Task<IActionResult> Create([FromBody] CreateSensorTypeDto dto)
  {
    var sensorType = await _sensorsTypesService.CreateSensorTypeAsync(dto);

    return CreatedAtAction(nameof(GetSensorType), new { id = sensorType.Id }, sensorType);
  }

  [HttpPut("{id}")]
  [SwaggerOperation("Administrator Only", "Updates sensor's type.")]
  [SwaggerResponse(200)]
  [SwaggerResponse(400, type: typeof(ProblemDetails))]
  [SwaggerResponse(422, type: typeof(ProblemDetails))]
  [SwaggerResponse(404, type: typeof(ProblemDetails))]
  public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateSensorTypeDto dto)
  {
    await _sensorsTypesService.UpdateSensorTypeAsync(id, dto);

    return Ok();
  }

  [HttpDelete("{id}")]
  [SwaggerOperation("Administrator Only", "Deletes sensor's type.")]
  [SwaggerResponse(200)]
  public async Task<IActionResult> Delete([FromRoute] Guid id)
  {
    await _sensorsTypesService.DeleteSensorTypeAsync(id);

    return Ok();
  }
}