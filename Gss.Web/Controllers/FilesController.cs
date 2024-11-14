using Gss.Core.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Gss.Web.Controllers
{
  [Authorize]
  [Route("api/[controller]/[action]")]
  [ApiController]
  public class FilesController: ControllerBase
  {
    [HttpPost]
    [SwaggerOperation("Authorized", "Loads data from files in database.")]
    public IActionResult MicrocontrollerDataUpload([FromForm] object dto)
    {
      return Ok(new Response<object>());
    }
  }
}
