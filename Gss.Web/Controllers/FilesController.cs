using System;
using System.Linq;
using System.Threading.Tasks;
using Gss.Core.DTOs;
using Gss.Core.Interfaces;
using Gss.Core.Resources;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Gss.Web.Controllers
{
  // TODO [Authorize] all controller?
  [Route("api/[controller]/[action]")]
  [ApiController]
  public class FilesController: ControllerBase
  {
    private readonly IAzureImagesRepository _azureImagesRepository;
    public FilesController(IAzureImagesRepository azureImagesRepository)
    {
      _azureImagesRepository = azureImagesRepository;
    }

    [HttpPost]
    [SwaggerOperation("Authorized Only", "Uploads user's avatar in cloud storage.")]
    [SwaggerResponse(200, type: typeof(Response<Uri>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> AvatarUpload([FromForm] FilesUploadDto dto)
    {
      var file = dto.FileForm.FirstOrDefault();
      string fileExtension = file.FileName.Split('.').Last();

      if (String.IsNullOrEmpty(fileExtension))
      {
        return BadRequest(new Response<object>()
          .AddErrors(Messages.MissingFileExtensionErrorString));
      }

      var response = await _azureImagesRepository.UploadImage(file.OpenReadStream(), fileExtension);

      return response.Succeeded
        ? Ok(response)
        : BadRequest(response);
    }

    [HttpPost]
    [SwaggerOperation("Authorized Only", "Loads data from files in database.")]
    public IActionResult MicrocontrollerDataUpload([FromForm] FilesUploadDto dto)
    {
      return Ok();
    }

    //[Authorize] Role = Administrator
    [HttpDelete("{userID}")]
    [SwaggerOperation("Administrator Only", "Gets all users existing in database. Paged.")]
    [SwaggerResponse(200, type: typeof(Response<object>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> DeleteUserAvatar(string userID)
    {
      if (!ValidateGuidString(userID))
      {
        return BadRequest(new Response<object>()
          .AddErrors(Messages.InvalidGuidErrorString));
      }

      await _azureImagesRepository.DeleteImage(userID);

      return Ok();
    }

    private bool ValidateGuidString(string guid)
    {
      return Guid.TryParse(guid, out _);
    }
  }
}
