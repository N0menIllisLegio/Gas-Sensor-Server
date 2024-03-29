﻿using System;
using System.Threading.Tasks;
using Gss.Core.DTOs;
using Gss.Core.DTOs.SensorType;
using Gss.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Gss.Web.Controllers
{
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
    [SwaggerResponse(200, type: typeof(Response<PagedResultDto<SensorTypeDto>>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> GetAllSensorsTypes([FromBody] PagedInfoDto pagedRequest)
    {
      var pagedResult = await _sensorsTypesService.GetAllSensorsTypesAsync(pagedRequest);

      return Ok(new Response<PagedResultDto<SensorTypeDto>>(pagedResult));
    }

    [HttpGet("{id}")]
    [SwaggerOperation("Administrator Only", "Gets sensor's type by id.")]
    [SwaggerResponse(200, type: typeof(Response<SensorTypeDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> GetSensorType([FromRoute] Guid id)
    {
      var sensorTypeDto = await _sensorsTypesService.GetSensorTypeAsync(id);

      return Ok(new Response<SensorTypeDto>(sensorTypeDto));
    }

    [HttpPost]
    [SwaggerOperation("Administrator Only", "Creates sensor's type.")]
    [SwaggerResponse(200, type: typeof(Response<SensorTypeDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> Create([FromBody] CreateSensorTypeDto dto)
    {
      var sensorTypeDto = await _sensorsTypesService.CreateSensorTypeAsync(dto);

      return Ok(new Response<SensorTypeDto>(sensorTypeDto));
    }

    [HttpPut("{id}")]
    [SwaggerOperation("Administrator Only", "Updates sensor's type.")]
    [SwaggerResponse(200, type: typeof(Response<SensorTypeDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateSensorTypeDto dto)
    {
      var sensorTypeDto = await _sensorsTypesService.UpdateSensorTypeAsync(id, dto);

      return Ok(new Response<SensorTypeDto>(sensorTypeDto));
    }

    [HttpDelete("{id}")]
    [SwaggerOperation("Administrator Only", "Deletes sensor's type.")]
    [SwaggerResponse(200, type: typeof(Response<SensorTypeDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
      var sensorTypeDto = await _sensorsTypesService.DeleteSensorTypeAsync(id);

      return Ok(new Response<SensorTypeDto>(sensorTypeDto));
    }
  }
}
