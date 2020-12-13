using System;
using System.Linq;
using System.Threading.Tasks;
using Gss.Core.DTOs;
using Gss.Core.Entities;
using Gss.Core.Helpers;
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
    private readonly UserManager _userManager;

    public MicrocontrollersController(IMicrocontrollerService microcontrollerService, UserManager userManager)
    {
      _microcontrollerService = microcontrollerService;
      _userManager = userManager;
    }

    //[HttpGet]
    //public async Task<IActionResult> GetAllUserMicrocontrollers()
    //{
    //  // var response = await _microcontrollerRepository.GetMicrocontrollersAsync(4, 1, (m) => m.Public && m.Name != null, Core.Enums.SortOrder.Ascendind, (m) => m.Name);
    //  return Ok();
    //}

    [HttpPost]
    public async Task<IActionResult> CreateMicrocontroller([FromBody] CreateMicrocontrollerDto dto)
    {
      var user = await _userManager.FindByEmailAsync(User.Identity.Name);

      if (user is null)
      {
        return BadRequest(new Response<object>()
          .AddErrors(String.Format(Messages.NotFoundErrorString, "User")));
      }

      var result = await _microcontrollerService.AddMicrocontroller(dto, user);

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

      Response<Microcontroller> result;

      if (await _userManager.IsAdministrator(User.Identity.Name))
      {
        result = await _microcontrollerService.UpdateMicrocontroller(dto);
      }
      else
      {
        var user = await _userManager.FindByEmailAsync(User.Identity.Name);

        if (user is null)
        {
          return BadRequest(new Response<object>()
            .AddErrors(String.Format(Messages.NotFoundErrorString, "User")));
        }

        result = await _microcontrollerService.UpdateMicrocontroller(dto, user);
      }

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

      Response<Microcontroller> result;

      if (await _userManager.IsAdministrator(User.Identity.Name))
      {
        result = await _microcontrollerService.DeleteMicrocontroller(microcontrollerID);
      }
      else
      {
        var user = await _userManager.FindByEmailAsync(User.Identity.Name);

        if (user is null)
        {
          return BadRequest(new Response<object>()
            .AddErrors(String.Format(Messages.NotFoundErrorString, "User")));
        }

        result = await _microcontrollerService.DeleteMicrocontroller(microcontrollerID, user);
      }

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
