using System;
using System.Threading.Tasks;
using Gss.Core.DTOs;
using Gss.Core.Interfaces;
using Gss.Core.Resources;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Gss.Web.Controllers
{
  //[Authorize] Role = Administrator
  [Route("api/[controller]/[action]")]
  [ApiController]
  public class SensorsController : ControllerBase
  {
    private readonly ISensorsService _sensorsService;

    public SensorsController(ISensorsService sensorsService)
    {
      _sensorsService = sensorsService;
    }

    [HttpGet]
    [SwaggerOperation("Administrator Only", "Gets all sensors.")]
    [SwaggerResponse(200, type: typeof(Response<SensorInfoDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> GetAllSensors([FromQuery] PagedRequest pagedRequest)
    {
      return await Task.Run(Ok);
    }

    // Just Authorized
    [HttpGet("{microcontrollerID}")]
    [SwaggerOperation("Authorized Only", "Gets all sensors of microcontroller.")]
    [SwaggerResponse(200, type: typeof(Response<SensorInfoDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> GetMicrocontrollerSensors(string microcontrollerID)
    {
      if (!ValidateGuidString(microcontrollerID))
      {
        return BadRequest(new Response<object>()
          .AddErrors(Messages.InvalidGuidErrorString));
      }

      return await Task.Run(Ok);
    }

    [HttpPost]
    [SwaggerOperation("Administrator Only", "Creates sensor.")]
    [SwaggerResponse(200, type: typeof(Response<SensorInfoDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> Create([FromBody] CreateSensorDto dto)
    {
      return await Task.Run(Ok);
    }

    [HttpPut]
    [SwaggerOperation("Administrator Only", "Updates sensor.")]
    [SwaggerResponse(200, type: typeof(Response<SensorInfoDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> Update([FromBody] UpdateSensorDto dto)
    {
      return await Task.Run(Ok);
    }

    [HttpDelete("{sensorID}")]
    [SwaggerOperation("Administrator Only", "Deletes sensor.")]
    [SwaggerResponse(200, type: typeof(Response<SensorInfoDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> Delete(string sensorID)
    {
      if (!ValidateGuidString(sensorID))
      {
        return BadRequest(new Response<object>()
          .AddErrors(Messages.InvalidGuidErrorString));
      }

      return await Task.Run(Ok);
    }

    private bool ValidateGuidString(string guid)
    {
      return Guid.TryParse(guid, out _);
    }
  }
}
