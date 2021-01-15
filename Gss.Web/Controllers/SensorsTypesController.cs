using System;
using System.Linq;
using System.Threading.Tasks;
using Gss.Core.DTOs;
using Gss.Core.DTOs.SensorType;
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
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> GetAllSensorsTypes([FromQuery] PagedRequest pagedRequest)
    {
      var (sensorsTypes, sensorsTypesCount) = await _sensorsTypesService.GetAllSensorsTypes(pagedRequest.PageNumber,
        pagedRequest.PageSize, pagedRequest.SortOrder, pagedRequest.Filter);

      var microcontrollersInfos = sensorsTypes.Data.Select(type => new SensorTypeInfoDto(type)).ToList();
      var response = new PagedResponse<SensorTypeInfoDto>(microcontrollersInfos, pagedRequest.PageNumber, pagedRequest.PageSize)
      {
        TotalRecords = sensorsTypesCount,
        OrderedBy = pagedRequest.SortBy,
        SortOrder = pagedRequest.SortOrder,
        Filter = pagedRequest.Filter,
        FilteredBy = pagedRequest.FilterBy
      };

      return Ok(response);
    }

    [HttpGet("{sensorTypeID}")]
    [SwaggerOperation(Description = "Gets sensor's type by id.")]
    [SwaggerResponse(200, type: typeof(Response<SensorTypeInfoDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> GetSensorType(string sensorTypeID)
    {
      if (!Guid.TryParse(sensorTypeID, out var id))
      {
        return BadRequest(new Response<object>()
          .AddErrors(Messages.InvalidGuidErrorString));
      }

      var result = await _sensorsTypesService.GetSensorType(id);

      if (!result.Succeeded)
      {
        return BadRequest(new Response<object>(result));
      }

      var sensorType = result.Data.First();
      var sensorTypeInfoDto = new SensorTypeInfoDto(sensorType);

      return Ok(new Response<SensorTypeInfoDto>(sensorTypeInfoDto));
    }

    [HttpPost]
    [SwaggerOperation("Administrator Only", "Creates sensor's type.")]
    [SwaggerResponse(200, type: typeof(Response<SensorTypeInfoDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> Create([FromBody] CreateSensorTypeDto dto)
    {
      var result = await _sensorsTypesService.AddSensorType(dto);

      if (!result.Succeeded)
      {
        return BadRequest(new Response<object>(result));
      }

      var sensorType = result.Data.First();
      var sensorTypeInfoDto = new SensorTypeInfoDto(sensorType);

      return Ok(new Response<SensorTypeInfoDto>(sensorTypeInfoDto));
    }

    [HttpPut]
    [SwaggerOperation("Administrator Only", "Updates sensor's type.")]
    [SwaggerResponse(200, type: typeof(Response<SensorTypeInfoDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> Update([FromBody] UpdateSensorTypeDto dto)
    {
      var result = await _sensorsTypesService.UpdateSensorType(dto);

      if (!result.Succeeded)
      {
        return BadRequest(new Response<object>(result));
      }

      var sensorType = result.Data.First();
      var sensorTypeInfoDto = new SensorTypeInfoDto(sensorType);

      return Ok(new Response<SensorTypeInfoDto>(sensorTypeInfoDto));
    }

    [HttpDelete("{sensorTypeID}")]
    [SwaggerOperation("Administrator Only", "Deletes sensor's type.")]
    [SwaggerResponse(200, type: typeof(Response<SensorTypeInfoDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> Delete(string sensorTypeID)
    {
      if (!Guid.TryParse(sensorTypeID, out var id))
      {
        return BadRequest(new Response<object>()
          .AddErrors(Messages.InvalidGuidErrorString));
      }

      var result = await _sensorsTypesService.DeleteSensorType(id);

      if (!result.Succeeded)
      {
        return BadRequest(new Response<object>(result));
      }

      var sensorType = result.Data.First();
      var sensorTypeInfoDto = new SensorTypeInfoDto(sensorType);

      return Ok(new Response<SensorTypeInfoDto>(sensorTypeInfoDto));
    }
  }
}
