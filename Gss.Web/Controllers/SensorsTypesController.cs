using System;
using System.Threading.Tasks;
using Gss.Core.DTOs;
using Gss.Core.Interfaces;
using Gss.Core.Resources;
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

    [HttpGet]
    [SwaggerOperation(Description = "Gets all sensor's types.")]
    [SwaggerResponse(200, type: typeof(Response<SensorTypeInfoDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> GetAllSensorsTypes([FromQuery] PagedRequest pagedRequest)
    {
      return await Task.Run(Ok);
    }

    [HttpGet("{sensorTypeID}")]
    [SwaggerOperation(Description = "Gets sensor's type by id.")]
    [SwaggerResponse(200, type: typeof(Response<SensorTypeInfoDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> GetSensorType(string sensorTypeID)
    {
      if (!ValidateGuidString(sensorTypeID))
      {
        return BadRequest(new Response<object>()
          .AddErrors(Messages.InvalidGuidErrorString));
      }

      return await Task.Run(Ok);
    }

    [HttpPost]
    [SwaggerOperation("Administrator Only", "Creates sensor's type.")]
    [SwaggerResponse(200, type: typeof(Response<SensorTypeInfoDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> Create([FromBody] CreateSensorTypeDto dto)
    {
      return await Task.Run(Ok);
    }

    [HttpPut]
    [SwaggerOperation("Administrator Only", "Updates sensor's type.")]
    [SwaggerResponse(200, type: typeof(Response<SensorTypeInfoDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> Update([FromBody] UpdateSensorTypeDto dto)
    {
      return await Task.Run(Ok);
    }

    [HttpDelete("{sensorTypeID}")]
    [SwaggerOperation("Administrator Only", "Deletes sensor's type.")]
    [SwaggerResponse(200, type: typeof(Response<SensorTypeInfoDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> Delete(string sensorTypeID)
    {
      if (!ValidateGuidString(sensorTypeID))
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
