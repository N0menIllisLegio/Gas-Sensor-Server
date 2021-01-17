using System.Threading.Tasks;
using Gss.Core.DTOs;
using Gss.Core.DTOs.SensorType;
using Gss.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Gss.Web.Controllers
{
  //[Authorize], even make all gets anon
  [Route("api/[controller]/[action]")]
  [ApiController]
  public class SensorsTypesController : ControllerBase
  {
    private readonly ISensorsTypesService _sensorsTypesService;
    public SensorsTypesController(ISensorsTypesService sensorsTypesService)
    {
      _sensorsTypesService = sensorsTypesService;
    }

    [HttpPost]
    [SwaggerOperation(Description = "Gets all sensor's types.")]
    [SwaggerResponse(200, type: typeof(Response<PagedResultDto<SensorTypeDto>>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> GetAllSensorsTypes([FromBody] PagedInfoDto pagedRequest)
    {
      var pagedResult = await _sensorsTypesService.GetAllSensorsTypesAsync(pagedRequest);

      return Ok(new Response<PagedResultDto<SensorTypeDto>>(pagedResult));
    }

    [HttpGet]
    [SwaggerOperation(Description = "Gets sensor's type by id.")]
    [SwaggerResponse(200, type: typeof(Response<SensorTypeDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> GetSensorType([FromQuery] IdDto dto)
    {
      var sensorTypeDto = await _sensorsTypesService.GetSensorTypeAsync(dto.ID);

      return Ok(new Response<SensorTypeDto>(sensorTypeDto));
    }

    [HttpPost]
    [SwaggerOperation("Administrator Only", "Creates sensor's type.")]
    [SwaggerResponse(200, type: typeof(Response<SensorTypeDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> Create([FromBody] CreateSensorTypeDto dto)
    {
      var sensorTypeDto = await _sensorsTypesService.AddSensorTypeAsync(dto);

      return Ok(new Response<SensorTypeDto>(sensorTypeDto));
    }

    [HttpPut]
    [SwaggerOperation("Administrator Only", "Updates sensor's type.")]
    [SwaggerResponse(200, type: typeof(Response<SensorTypeDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> Update([FromBody] UpdateSensorTypeDto dto)
    {
      var sensorTypeDto = await _sensorsTypesService.UpdateSensorTypeAsync(dto);

      return Ok(new Response<SensorTypeDto>(sensorTypeDto));
    }

    [HttpDelete]
    [SwaggerOperation("Administrator Only", "Deletes sensor's type.")]
    [SwaggerResponse(200, type: typeof(Response<SensorTypeDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> Delete([FromQuery] IdDto dto)
    {
      var sensorTypeDto = await _sensorsTypesService.DeleteSensorTypeAsync(dto.ID);

      return Ok(new Response<SensorTypeDto>(sensorTypeDto));
    }
  }
}
