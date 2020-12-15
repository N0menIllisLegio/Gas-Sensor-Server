using System;
using System.Linq;
using System.Threading.Tasks;
using Gss.Core.DTOs;
using Gss.Core.Interfaces;
using Gss.Core.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gss.Web.Controllers
{
  [Authorize]
  [Route("api/[controller]/[action]")]
  [ApiController]
  public class MicrocontrollersController : ControllerBase
  {
    private readonly IMicrocontrollerService _microcontrollerService;

    public MicrocontrollersController(IMicrocontrollerService microcontrollerService)
    {
      _microcontrollerService = microcontrollerService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateMicrocontroller([FromBody] CreateMicrocontrollerDto dto)
    {
      var result = await _microcontrollerService.AddMicrocontroller(dto, User.Identity.Name);

      if (!result.Succeeded)
      {
        return BadRequest(result);
      }

      var microcontroller = result.Data.First();

      var microcontrollerInfoDto = new MicrocontrollerInfoDto(microcontroller);

      return Ok(new Response<MicrocontrollerInfoDto>(microcontrollerInfoDto));
    }

    [HttpPut]
    public async Task<IActionResult> UpdateMicrocontroller([FromBody] UpdateMicrocontrollerDto dto)
    {
      if (!ValidateGuidString(dto.ID))
      {
        return BadRequest(new Response<object>()
          .AddErrors(Messages.InvalidGuidErrorString));
      }

      var result = await _microcontrollerService.UpdateMicrocontroller(dto, User.Identity.Name);

      if (!result.Succeeded)
      {
        return BadRequest(result);
      }

      var microcontroller = result.Data.First();

      var microcontrollerInfoDto = new MicrocontrollerInfoDto(microcontroller);

      return Ok(new Response<MicrocontrollerInfoDto>(microcontrollerInfoDto));
    }

    [HttpDelete("{microcontrollerID}")]
    public async Task<IActionResult> DeleteMicrocontroller(string microcontrollerID)
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
        return BadRequest(result);
      }

      var microcontroller = result.Data.First();

      var microcontrollerInfoDto = new MicrocontrollerInfoDto(microcontroller);

      return Ok(new Response<MicrocontrollerInfoDto>(microcontrollerInfoDto));
    }

    // Change MC owner

    private bool ValidateGuidString(string guid)
    {
      return Guid.TryParse(guid, out _);
    }
  }
}
