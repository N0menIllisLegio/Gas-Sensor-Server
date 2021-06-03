using System;
using System.Threading.Tasks;
using Gss.Core.DTOs;
using Gss.Core.DTOs.Sensor;
using Gss.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Gss.Web.Controllers
{
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
    [SwaggerResponse(200, type: typeof(Response<PagedResultDto<SensorDto>>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> GetAllSensors([FromBody] PagedInfoDto pagedRequest)
    {
      var pagedResultDto = await _sensorsService.GetAllSensors(pagedRequest);

      return Ok(new Response<PagedResultDto<SensorDto>>(pagedResultDto));
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost("{microcontrollerID}")]
    [SwaggerOperation("Administrator Only", "Gets all sensors of microcontroller.")]
    [SwaggerResponse(200, type: typeof(Response<PagedResultDto<SensorDto>>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> GetMicrocontrollerSensors([FromRoute] Guid microcontrollerID, [FromBody] PagedInfoDto pagedRequest)
    {
      var pagedResult = await _sensorsService.GetMicrocontrollerSensors(microcontrollerID, pagedRequest);

      return Ok(new Response<PagedResultDto<SensorDto>>(pagedResult));
    }

    [Authorize(Roles = "Administrator")]
    [HttpGet("{id}")]
    [SwaggerOperation("Administrator Only", "Gets sensor by id.")]
    [SwaggerResponse(200, type: typeof(Response<SensorDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> GetSensor([FromRoute] Guid id)
    {
      var sensorDto = await _sensorsService.GetSensorAsync(id);

      return Ok(new Response<SensorDto>(sensorDto));
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost]
    [SwaggerOperation("Administrator Only", "Creates sensor.")]
    [SwaggerResponse(200, type: typeof(Response<SensorDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> Create([FromBody] CreateSensorDto dto)
    {
      var sensorDto = await _sensorsService.CreateSensorAsync(dto);

      return Ok(new Response<SensorDto>(sensorDto));
    }

    [Authorize(Roles = "Administrator")]
    [HttpPut("{id}")]
    [SwaggerOperation("Administrator Only", "Updates sensor.")]
    [SwaggerResponse(200, type: typeof(Response<SensorDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateSensorDto updateSensorDto)
    {
      var sensorDto = await _sensorsService.UpdateSensorAsync(id, updateSensorDto);

      return Ok(new Response<SensorDto>(sensorDto));
    }

    [Authorize(Roles = "Administrator")]
    [HttpDelete("{id}")]
    [SwaggerOperation("Administrator Only", "Deletes sensor.")]
    [SwaggerResponse(200, type: typeof(Response<SensorDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
      var sensorDto = await _sensorsService.DeleteSensorAsync(id);

      return Ok(new Response<SensorDto>(sensorDto));
    }

    [Authorize(Roles = "Administrator")]
    [HttpPatch]
    [SwaggerOperation("Administrator Only", "Changes sensor's type.")]
    [SwaggerResponse(200, type: typeof(Response<SensorDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> SetSensorType([FromBody] SetSensorTypeDto dto)
    {
      var sensorDto = await _sensorsService.SetSensorTypeAsync(dto);

      return Ok(new Response<SensorDto>(sensorDto));
    }
  }
}
