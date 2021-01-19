using System.Threading.Tasks;
using Gss.Core.DTOs;
using Gss.Core.DTOs.Sensor;
using Gss.Core.Interfaces.Services;
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

    // anon
    [HttpPost]
    [SwaggerOperation("Administrator Only", "Gets all sensors.")]
    [SwaggerResponse(200, type: typeof(Response<PagedResultDto<SensorDto>>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> GetAllSensors([FromBody] PagedInfoDto pagedRequest)
    {
      var pagedResultDto = await _sensorsService.GetAllSensors(pagedRequest);
      return Ok(new Response<PagedResultDto<SensorDto>>(pagedResultDto));
    }

    // anon, access if public
    [HttpGet]
    [SwaggerOperation("Authorized Only", "Gets all sensors of microcontroller.")]
    [SwaggerResponse(200, type: typeof(Response<PagedResultDto<SensorDto>>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> GetMicrocontrollerSensors([FromQuery] IdDto dto)
    {
      return await Task.Run(Ok);
    }

    // anon, access if public
    [HttpGet]
    [SwaggerOperation("Authorized Only", "Gets sensor by id.")]
    [SwaggerResponse(200, type: typeof(Response<SensorDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> GetSensor([FromQuery] IdDto dto)
    {
      return await Task.Run(Ok);
    }

    [HttpPost]
    [SwaggerOperation("Administrator Only", "Creates sensor.")]
    [SwaggerResponse(200, type: typeof(Response<SensorDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> Create([FromBody] CreateSensorDto dto)
    {
      return await Task.Run(Ok);
    }

    [HttpPut]
    [SwaggerOperation("Administrator Only", "Updates sensor.")]
    [SwaggerResponse(200, type: typeof(Response<SensorDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> Update([FromBody] UpdateSensorDto dto)
    {
      return await Task.Run(Ok);
    }

    [HttpDelete]
    [SwaggerOperation("Administrator Only", "Deletes sensor.")]
    [SwaggerResponse(200, type: typeof(Response<SensorDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> Delete([FromQuery] IdDto dto)
    {
      return await Task.Run(Ok);
    }
  }
}
