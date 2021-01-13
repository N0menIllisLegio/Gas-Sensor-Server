using System;
using System.Linq;
using System.Threading.Tasks;
using Gss.Core.DTOs;
using Gss.Core.Interfaces;
using Gss.Core.Resources;
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
    [HttpGet]
    [SwaggerOperation("Administrator Only", "Gets all microcontrollers. Paged.")]
    [SwaggerResponse(200, type: typeof(PagedResponse<MicrocontrollerInfoDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> GetAllMicrocontrollers([FromQuery] PagedRequest pagedRequest)
    {
      var (microcontrollers, microcontrollersCount) = await _microcontrollerService.GetAllMicrocontrollers(pagedRequest.PageNumber,
        pagedRequest.PageSize, pagedRequest.SortOrder, pagedRequest.SortBy, pagedRequest.FilterBy, pagedRequest.Filter);

      var microcontrollersInfos = microcontrollers.Data.Select(microcontroller => new MicrocontrollerInfoDto(microcontroller)).ToList();
      var response = new PagedResponse<MicrocontrollerInfoDto>(microcontrollersInfos, pagedRequest.PageNumber, pagedRequest.PageSize)
      {
        TotalRecords = microcontrollersCount,
        OrderedBy = pagedRequest.SortBy,
        SortOrder = pagedRequest.SortOrder,
        Filter = pagedRequest.Filter,
        FilteredBy = pagedRequest.FilterBy
      };

      return Ok(response);
    }

    [AllowAnonymous]
    [HttpGet]
    [SwaggerOperation(Description = "Gets all public microcontrollers.")]
    [SwaggerResponse(200, type: typeof(PagedResponse<MicrocontrollerInfoDto>))]
    public async Task<IActionResult> GetPublicMicrocontrollers([FromQuery] PagedRequest pagedRequest)
    {
      var (microcontrollers, microcontrollersCount) = await _microcontrollerService.GetPublicMicrocontrollers(pagedRequest.PageNumber,
        pagedRequest.PageSize, pagedRequest.SortOrder, pagedRequest.SortBy, pagedRequest.FilterBy, pagedRequest.Filter);

      var microcontrollersInfos = microcontrollers.Data.Select(microcontroller => new MicrocontrollerInfoDto(microcontroller, false));
      var response = new PagedResponse<MicrocontrollerInfoDto>(microcontrollersInfos, pagedRequest.PageNumber, pagedRequest.PageSize)
      {
        TotalRecords = microcontrollersCount,
        OrderedBy = pagedRequest.SortBy,
        SortOrder = pagedRequest.SortOrder,
        Filter = pagedRequest.Filter,
        FilteredBy = pagedRequest.FilterBy
      };

      return Ok(response);
    }

    [HttpGet("{userID}")]
    [SwaggerOperation("Authorized Only", "Gets all microcontrollers that belongs to user.")]
    [SwaggerResponse(200, type: typeof(PagedResponse<MicrocontrollerInfoDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> GetUserMicrocontrollers(string userID, [FromQuery] PagedRequest pagedRequest)
    {
      if (!ValidateGuidString(userID))
      {
        return BadRequest(new Response<object>()
          .AddErrors(Messages.InvalidGuidErrorString));
      }

      var (result, microcontrollersCount, displaySensitiveInfo) = await _microcontrollerService.GetUserMicrocontrollers(userID, User.Identity.Name,
        pagedRequest.PageNumber, pagedRequest.PageSize, pagedRequest.SortOrder, pagedRequest.SortBy, pagedRequest.FilterBy, pagedRequest.Filter);

      if (!result.Succeeded)
      {
        return BadRequest(new Response<object>(result));
      }

      var microcontrollersInfos = result.Data.Select(microcontroller => new MicrocontrollerInfoDto(microcontroller, displaySensitiveInfo));
      var response = new PagedResponse<MicrocontrollerInfoDto>(microcontrollersInfos, pagedRequest.PageNumber, pagedRequest.PageSize)
      {
        TotalRecords = microcontrollersCount,
        OrderedBy = pagedRequest.SortBy,
        SortOrder = pagedRequest.SortOrder,
        Filter = pagedRequest.Filter,
        FilteredBy = pagedRequest.FilterBy
      };

      return Ok(response);
    }

    [HttpGet("{microcontrollerID}")]
    [SwaggerOperation("Authorized Only", "Gets microcontroller by id.")]
    [SwaggerResponse(200, type: typeof(Response<MicrocontrollerInfoDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> GetMicrocontroller(string microcontrollerID)
    {
      if (!ValidateGuidString(microcontrollerID))
      {
        return BadRequest(new Response<object>()
          .AddErrors(Messages.InvalidGuidErrorString));
      }

      var (result, displaySensitiveInfo)
        = await _microcontrollerService.GetMicrocontroller(microcontrollerID, User.Identity.Name);

      if (!result.Succeeded)
      {
        return BadRequest(new Response<object>(result));
      }

      var microcontroller = result.Data.First();
      var microcontrollerInfoDto = new MicrocontrollerInfoDto(microcontroller, displaySensitiveInfo);

      return Ok(new Response<MicrocontrollerInfoDto>(microcontrollerInfoDto));
    }

    [HttpPost]
    [SwaggerOperation("Authorized Only", "Creates microcontroller.")]
    [SwaggerResponse(200, type: typeof(Response<MicrocontrollerInfoDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> Create([FromBody] CreateMicrocontrollerDto dto)
    {
      var result = await _microcontrollerService.AddMicrocontroller(dto, User.Identity.Name);

      if (!result.Succeeded)
      {
        return BadRequest(new Response<object>(result));
      }

      var microcontroller = result.Data.First();
      var microcontrollerInfoDto = new MicrocontrollerInfoDto(microcontroller);

      return Ok(new Response<MicrocontrollerInfoDto>(microcontrollerInfoDto));
    }

    [HttpPut]
    [SwaggerOperation("Authorized Only", "Updates microcontroller.")]
    [SwaggerResponse(200, type: typeof(Response<MicrocontrollerInfoDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> Update([FromBody] UpdateMicrocontrollerDto dto)
    {
      if (!ValidateGuidString(dto.ID))
      {
        return BadRequest(new Response<object>()
          .AddErrors(Messages.InvalidGuidErrorString));
      }

      var result = await _microcontrollerService.UpdateMicrocontroller(dto, User.Identity.Name);

      if (!result.Succeeded)
      {
        return BadRequest(new Response<object>(result));
      }

      var microcontroller = result.Data.First();
      var microcontrollerInfoDto = new MicrocontrollerInfoDto(microcontroller);

      return Ok(new Response<MicrocontrollerInfoDto>(microcontrollerInfoDto));
    }

    [HttpDelete("{microcontrollerID}")]
    [SwaggerOperation("Authorized Only", "Deletes microcontroller.")]
    [SwaggerResponse(200, type: typeof(Response<MicrocontrollerInfoDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> Delete(string microcontrollerID)
    {
      if (!ValidateGuidString(microcontrollerID))
      {
        return BadRequest(new Response<object>()
          .AddErrors(Messages.InvalidGuidErrorString));
      }

      var result = await _microcontrollerService
        .DeleteMicrocontroller(microcontrollerID, User.Identity.Name);

      if (!result.Succeeded)
      {
        return BadRequest(new Response<object>(result));
      }

      var microcontroller = result.Data.First();
      var microcontrollerInfoDto = new MicrocontrollerInfoDto(microcontroller);

      return Ok(new Response<MicrocontrollerInfoDto>(microcontrollerInfoDto));
    }

    // Admin only
    [HttpPatch("{microcontrollerID}/{userID}")]
    [SwaggerOperation("Administrator Only", "Changes microcontroller owner.")]
    [SwaggerResponse(200, type: typeof(Response<MicrocontrollerInfoDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> ChangeOwner(string microcontrollerID, string userID)
    {
      if (!ValidateGuidString(microcontrollerID) || !ValidateGuidString(userID))
      {
        return BadRequest(new Response<object>()
          .AddErrors(Messages.InvalidGuidErrorString));
      }

      var result = await _microcontrollerService
        .ChangeMicrocontrollerOwner(microcontrollerID, userID);

      if (!result.Succeeded)
      {
        return BadRequest(new Response<object>(result));
      }

      var microcontroller = result.Data.First();
      var microcontrollerInfoDto = new MicrocontrollerInfoDto(microcontroller);

      return Ok(new Response<MicrocontrollerInfoDto>(microcontrollerInfoDto));
    }

    private bool ValidateGuidString(string guid)
    {
      return Guid.TryParse(guid, out _);
    }
  }
}
