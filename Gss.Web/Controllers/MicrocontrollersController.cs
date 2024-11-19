using Gss.Core.DTOs;
using Gss.Core.DTOs.Microcontroller;
using Gss.Core.DTOs.Sensor;
using Gss.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Gss.Web.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class MicrocontrollersController : ControllerBase
{
  private readonly IMicrocontrollersService _microcontrollerService;

  public MicrocontrollersController(IMicrocontrollersService microcontrollerService)
  {
      _microcontrollerService = microcontrollerService;
    }

  [Authorize(Roles = "Administrator")]
  [HttpPost]
  [SwaggerOperation("Administrator Only", "Gets all microcontrollers. Paged.")]
  [SwaggerResponse(200, type: typeof(PagedResultDto<MicrocontrollerDto>))]
  [SwaggerResponse(400, type: typeof(ProblemDetails))]
  [SwaggerResponse(401, type: typeof(ProblemDetails))]
  [SwaggerResponse(403, type: typeof(ProblemDetails))]
  [SwaggerResponse(422, type: typeof(ProblemDetails))]
  public async Task<IActionResult> GetAllMicrocontrollers([FromBody] PagedInfoDto dto)
  {
    var pagedResultDto = await _microcontrollerService.GetAllMicrocontrollersAsync(dto);

    return Ok(pagedResultDto);
  }

  [Authorize(Roles = "Administrator")]
  [HttpGet("{microcontrollerId}")]
  [SwaggerOperation("Administrator Only", "Gets all sensors of microcontroller.")]
  [SwaggerResponse(200, type: typeof(List<SensorDto>))]
  public async Task<IActionResult> GetMicrocontrollerSensors([FromRoute] Guid microcontrollerId)
  {
    var result = await _microcontrollerService.GetMicrocontrollerSensorsAsync(microcontrollerId);

    return Ok(result);
  }

  [HttpPost]
  [SwaggerOperation(Description = "Gets all public microcontrollers.")]
  [SwaggerResponse(200, type: typeof(PagedResultDto<MicrocontrollerDto>))]
  [SwaggerResponse(400, type: typeof(ProblemDetails))]
  [SwaggerResponse(422, type: typeof(ProblemDetails))]
  public async Task<IActionResult> GetPublicMicrocontrollers([FromBody] PagedInfoDto dto)
  {
    var pagedResultDto = await _microcontrollerService.GetPublicMicrocontrollersAsync(dto);

    return Ok(pagedResultDto);
  }

  [HttpPost]
  [SwaggerOperation(Description = "Gets all public microcontrollers. For map.")]
  [SwaggerResponse(200, type: typeof(List<MapMicrocontrollerDto>))]
  [SwaggerResponse(400, type: typeof(ProblemDetails))]
  [SwaggerResponse(422, type: typeof(ProblemDetails))]
  public async Task<IActionResult> GetPublicMicrocontrollersMap([FromBody] MapRequestDto dto)
  {
    var mapResponse = await _microcontrollerService.GetPublicMicrocontrollersMapAsync(dto);

    return Ok(mapResponse);
  }

  [HttpPost("{userId}")]
  [SwaggerOperation(description: "Gets all microcontrollers that belongs to user.")]
  [SwaggerResponse(200, type: typeof(MicrocontrollerDto))]
  [SwaggerResponse(400, type: typeof(ProblemDetails))]
  [SwaggerResponse(422, type: typeof(ProblemDetails))]
  public async Task<IActionResult> GetUserMicrocontrollers([FromRoute] Guid userId, [FromBody] PagedInfoDto pagedInfoDto)
  {
    var pagedResultDto = await _microcontrollerService.GetUserMicrocontrollersAsync(userId, pagedInfoDto);

    return Ok(pagedResultDto);
  }

  [HttpGet("{id}")]
  [SwaggerOperation(description: "Gets microcontroller by id.")]
  [SwaggerResponse(200, type: typeof(MicrocontrollerDto))]
  [SwaggerResponse(404, type: typeof(ProblemDetails))]
  public async Task<IActionResult> GetMicrocontroller([FromRoute] Guid id)
  {
    var microcontrollerDto = await _microcontrollerService.GetMicrocontrollerAsync(id);

    return Ok(microcontrollerDto);
  }

  [Authorize]
  [HttpPost]
  [SwaggerOperation("Authorized", "Creates microcontroller.")]
  [SwaggerResponse(201, type: typeof(MicrocontrollerDto))]
  [SwaggerResponse(400, type: typeof(ProblemDetails))]
  [SwaggerResponse(401, type: typeof(ProblemDetails))]
  [SwaggerResponse(404, type: typeof(ProblemDetails))]
  [SwaggerResponse(422, type: typeof(ProblemDetails))]
  public async Task<IActionResult> Create([FromBody] CreateMicrocontrollerDto dto)
  {
    var microcontrollerDto = await _microcontrollerService.AddMicrocontrollerAsync(dto);

    return CreatedAtAction(nameof(GetMicrocontroller), new { id = microcontrollerDto.Id }, microcontrollerDto);
  }

  [Authorize]
  [HttpPut("{microcontrollerId}")]
  [SwaggerOperation("Authorized", "Updates microcontroller.")]
  [SwaggerResponse(200)]
  [SwaggerResponse(400, type: typeof(ProblemDetails))]
  [SwaggerResponse(401, type: typeof(ProblemDetails))]
  [SwaggerResponse(422, type: typeof(ProblemDetails))]
  public async Task<IActionResult> Update([FromRoute] Guid microcontrollerId, [FromBody] UpdateMicrocontrollerDto dto)
  {
    await _microcontrollerService.UpdateMicrocontrollerAsync(microcontrollerId, dto);

    return Ok();
  }

  [Authorize]
  [HttpDelete("{id}")]
  [SwaggerOperation("Authorized", "Deletes microcontroller.")]
  [SwaggerResponse(200)]
  [SwaggerResponse(401, type: typeof(ProblemDetails))]
  public async Task<IActionResult> Delete([FromRoute] Guid id)
  {
    await _microcontrollerService.DeleteMicrocontrollerAsync(id);

    return Ok();
  }

  [Authorize]
  [HttpPatch]
  [SwaggerOperation("Authorized", "Requests sensor's value from microcontroller.")]
  [SwaggerResponse(200, type: typeof(RequestSensorValueResponseDto))]
  [SwaggerResponse(400, type: typeof(ProblemDetails))]
  [SwaggerResponse(401, type: typeof(ProblemDetails))]
  [SwaggerResponse(404, type: typeof(ProblemDetails))]
  [SwaggerResponse(422, type: typeof(ProblemDetails))]
  public async Task<IActionResult> RequestSensorValue([FromBody] RequestSensorValueDto requestSensorValueDto)
  {
    var response = await _microcontrollerService.RequestSensorValueAsync(
      requestSensorValueDto.MicrocontrollerId, requestSensorValueDto.SensorId);

    return Ok(response);
  }

  [Authorize]
  [HttpPatch]
  [SwaggerOperation("Authorized", "Sets sensor's critical value threshold, after reaching it email will be send.")]
  [SwaggerResponse(200)]
  [SwaggerResponse(400, type: typeof(ProblemDetails))]
  [SwaggerResponse(401, type: typeof(ProblemDetails))]
  [SwaggerResponse(404, type: typeof(ProblemDetails))]
  [SwaggerResponse(422, type: typeof(ProblemDetails))]
  public async Task<IActionResult> SetSensorsCriticalValue([FromBody] SetSensorsCriticalValueDto dto)
  {
    await _microcontrollerService.SetSensorValueThresholdAsync(dto.MicrocontrollerSensorId, dto.CriticalValue);

    return Ok();
  }
}