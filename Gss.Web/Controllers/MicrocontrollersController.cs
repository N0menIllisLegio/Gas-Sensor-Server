using System.Threading.Tasks;
using Gss.Core.DTOs;
using Gss.Core.DTOs.Microcontroller;
using Gss.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Gss.Web.Controllers
{
  //[Authorize]
  [Route("api/[controller]/[action]")]
  [ApiController]
  public class MicrocontrollersController : ControllerBase
  {
    private readonly IMicrocontrollersService _microcontrollerService;

    public MicrocontrollersController(IMicrocontrollersService microcontrollerService)
    {
      _microcontrollerService = microcontrollerService;
    }

    // Admin only
    [HttpPost]
    [SwaggerOperation("Administrator Only", "Gets all microcontrollers. Paged.")]
    [SwaggerResponse(200, type: typeof(Response<PagedResultDto<MicrocontrollerDto>>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> GetAllMicrocontrollers([FromBody] PagedInfoDto dto)
    {
      var pagedResultDto = await _microcontrollerService.GetAllMicrocontrollersAsync(dto);

      return Ok(new Response<PagedResultDto<MicrocontrollerDto>>(pagedResultDto));
    }

    [AllowAnonymous]
    [HttpPost]
    [SwaggerOperation(Description = "Gets all public microcontrollers.")]
    [SwaggerResponse(200, type: typeof(Response<PagedResultDto<MicrocontrollerDto>>))]
    public async Task<IActionResult> GetPublicMicrocontrollers([FromBody] PagedInfoDto dto)
    {
      var pagedResultDto = await _microcontrollerService.GetPublicMicrocontrollersAsync(dto);

      return Ok(new Response<PagedResultDto<MicrocontrollerDto>>(pagedResultDto));
    }

    [HttpPost]
    [SwaggerOperation("Authorized Only", "Gets all microcontrollers that belongs to user.")]
    [SwaggerResponse(200, type: typeof(Response<PagedResultDto<MicrocontrollerDto>>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> GetUserMicrocontrollers([FromQuery] IdDto dto, [FromBody] PagedInfoDto pagedInfoDto)
    {
      var pagedResultDto = await _microcontrollerService.GetUserMicrocontrollersAsync(dto.ID, User.Identity.Name, pagedInfoDto);

      return Ok(new Response<PagedResultDto<MicrocontrollerDto>>(pagedResultDto));
    }

    [HttpGet]
    [SwaggerOperation("Authorized Only", "Gets microcontroller by id.")]
    [SwaggerResponse(200, type: typeof(Response<MicrocontrollerDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> GetMicrocontroller([FromQuery] IdDto dto)
    {
      var microcontrollerDto = await _microcontrollerService.GetMicrocontrollerAsync(dto.ID, User.Identity.Name);

      return Ok(new Response<MicrocontrollerDto>(microcontrollerDto));
    }

    [HttpPost]
    [SwaggerOperation("Authorized Only", "Creates microcontroller.")]
    [SwaggerResponse(200, type: typeof(Response<MicrocontrollerDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> Create([FromBody] CreateMicrocontrollerDto dto)
    {
      var microcontrollerDto = await _microcontrollerService.AddMicrocontrollerAsync(dto, User.Identity.Name);

      return Ok(new Response<MicrocontrollerDto>(microcontrollerDto));
    }

    [HttpPut]
    [SwaggerOperation("Authorized Only", "Updates microcontroller.")]
    [SwaggerResponse(200, type: typeof(Response<MicrocontrollerDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> Update([FromBody] UpdateMicrocontrollerDto dto)
    {
      var microcontrollerDto = await _microcontrollerService.UpdateMicrocontrollerAsync(dto, User.Identity.Name);

      return Ok(new Response<MicrocontrollerDto>(microcontrollerDto));
    }

    [HttpDelete]
    [SwaggerOperation("Authorized Only", "Deletes microcontroller.")]
    [SwaggerResponse(200, type: typeof(Response<MicrocontrollerDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> Delete([FromQuery] IdDto dto)
    {
      var microcontrollerDto = await _microcontrollerService
        .DeleteMicrocontrollerAsync(dto.ID, User.Identity.Name);

      return Ok(new Response<MicrocontrollerDto>(microcontrollerDto));
    }

    // Admin only
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
  }
}
