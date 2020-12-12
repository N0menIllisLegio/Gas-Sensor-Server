using Gss.Core.DTOs;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Gss.Web.Controllers
{
  // TODO [Authorize] all controller?
  [Route("api/[controller]/[action]")]
  [ApiController]
  public class FilesController : ControllerBase
  {
    [HttpPost]
    [SwaggerOperation("Authorized Only", "Uploads user's avatar in cloud storage.")]
    public IActionResult AvatarUpload([FromForm] FilesUploadDto dto)
    {
      return Ok();
    }

    [HttpPost]
    [SwaggerOperation("Authorized Only", "Loads data from files in database.")]
    public IActionResult MicrocontrollerDataUpload([FromForm] FilesUploadDto dto)
    {
      return Ok();
    }
  }
}
