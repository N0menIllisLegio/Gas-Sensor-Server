using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gss.Core.DTOs;
using Gss.Core.DTOs.Microcontroller;
using Gss.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Gss.Web.Controllers
{
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
    [SwaggerResponse(200, type: typeof(Response<PagedResultDto<MicrocontrollerDto>>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> GetAllMicrocontrollers([FromBody] PagedInfoDto dto)
    {
      var pagedResultDto = await _microcontrollerService.GetAllMicrocontrollersAsync(dto);

      return Ok(new Response<PagedResultDto<MicrocontrollerDto>>(pagedResultDto));
    }

    [HttpPost]
    [SwaggerOperation(Description = "Gets all public microcontrollers.")]
    [SwaggerResponse(200, type: typeof(Response<PagedResultDto<MicrocontrollerDto>>))]
    public async Task<IActionResult> GetPublicMicrocontrollers([FromBody] PagedInfoDto dto)
    {
      var pagedResultDto = await _microcontrollerService.GetPublicMicrocontrollersAsync(dto);

      return Ok(new Response<PagedResultDto<MicrocontrollerDto>>(pagedResultDto));
    }

    [HttpPost]
    [SwaggerOperation(Description = "Gets all public microcontrollers. For map.")]
    [SwaggerResponse(200, type: typeof(Response<List<MapMicrocontrollerDto>>))]
    public async Task<IActionResult> GetPublicMicrocontrollersMap([FromBody] MapRequestDto dto)
    {
      var mapResponse = await _microcontrollerService.GetPublicMicrocontrollersMapAsync(dto);

      return Ok(new Response<List<MapMicrocontrollerDto>>(mapResponse));
    }

    [HttpPost("{userID}")]
    [SwaggerOperation(description: "Gets all microcontrollers that belongs to user.")]
    [SwaggerResponse(200, type: typeof(Response<PagedResultDto<MicrocontrollerDto>>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> GetUserMicrocontrollers([FromRoute] Guid userID, [FromBody] PagedInfoDto pagedInfoDto)
    {
      var pagedResultDto = await _microcontrollerService.GetUserMicrocontrollersAsync(User.Identity.Name, userID, pagedInfoDto);

      return Ok(new Response<PagedResultDto<MicrocontrollerDto>>(pagedResultDto));
    }

    [HttpGet("{id}")]
    [SwaggerOperation(description: "Gets microcontroller by id.")]
    [SwaggerResponse(200, type: typeof(Response<MicrocontrollerDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> GetMicrocontroller([FromRoute] Guid id)
    {
      var microcontrollerDto = await _microcontrollerService.GetMicrocontrollerAsync(User.Identity.Name, id);

      return Ok(new Response<MicrocontrollerDto>(microcontrollerDto));
    }

    [Authorize]
    [HttpPost]
    [SwaggerOperation("Authorized", "Creates microcontroller.")]
    [SwaggerResponse(200, type: typeof(Response<MicrocontrollerDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> Create([FromBody] CreateMicrocontrollerDto dto)
    {
      var microcontrollerDto = await _microcontrollerService.AddMicrocontrollerAsync(User.Identity.Name, dto);

      return Ok(new Response<MicrocontrollerDto>(microcontrollerDto));
    }

    [Authorize]
    [HttpPut("{microcontrollerID}")]
    [SwaggerOperation("Authorized", "Updates microcontroller.")]
    [SwaggerResponse(200, type: typeof(Response<MicrocontrollerDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> Update([FromRoute] Guid microcontrollerID, [FromBody] UpdateMicrocontrollerDto dto)
    {
      var microcontrollerDto = await _microcontrollerService.UpdateMicrocontrollerAsync(User.Identity.Name, microcontrollerID, dto);

      return Ok(new Response<MicrocontrollerDto>(microcontrollerDto));
    }

    [Authorize]
    [HttpDelete("{id}")]
    [SwaggerOperation("Authorized", "Deletes microcontroller.")]
    [SwaggerResponse(200, type: typeof(Response<MicrocontrollerDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
      var microcontrollerDto = await _microcontrollerService
        .DeleteMicrocontrollerAsync(User.Identity.Name, id);

      return Ok(new Response<MicrocontrollerDto>(microcontrollerDto));
    }

    [Authorize(Roles = "Administrator")]
    [HttpPatch]
    [SwaggerOperation("Administrator Only", "Changes microcontroller owner.")]
    [SwaggerResponse(200, type: typeof(Response<MicrocontrollerDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> ChangeOwner([FromQuery] ChangeMicrocontrollerOwnerDto dto)
    {
      var microcontrollerDto = await _microcontrollerService
        .ChangeMicrocontrollerOwnerAsync(dto.MicrocontrollerID, dto.UserID);

      return Ok(new Response<MicrocontrollerDto>(microcontrollerDto));
    }

    [Authorize(Roles = "Administrator")]
    [HttpPatch]
    [SwaggerOperation("Administrator Only", "Adds sensor to microcontroller.")]
    [SwaggerResponse(200, type: typeof(Response<MicrocontrollerDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> AddSensor([FromBody] AddSensorDto dto)
    {
      var microcontrollerDto = await _microcontrollerService.AddSensorAsync(User.Identity.Name, dto);

      return Ok(new Response<MicrocontrollerDto>(microcontrollerDto));
    }

    [Authorize(Roles = "Administrator")]
    [HttpPatch]
    [SwaggerOperation("Administrator Only", "Removes sensor from microcontroller.")]
    [SwaggerResponse(200, type: typeof(Response<MicrocontrollerDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> RemoveSensor([FromBody] RemoveSensorDto dto)
    {
      var microcontrollerDto = await _microcontrollerService.RemoveSensorAsync(User.Identity.Name, dto);

      return Ok(new Response<MicrocontrollerDto>(microcontrollerDto));
    }

    [Authorize]
    [HttpPatch]
    [SwaggerOperation("Authorized", "Requests sensor's value from microcontroller.")]
    [SwaggerResponse(200, type: typeof(Response<RequestSensorValueResponseDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> RequestSensorValue([FromBody] RequestSensorValueDto requestSensorValueDto)
    {
      var response = await _microcontrollerService.RequestSensorValue(
        User.Identity.Name, requestSensorValueDto.MicrocontrollerID, requestSensorValueDto.SensorID);

      return Ok(new Response<RequestSensorValueResponseDto>(response));
    }
  }
}
