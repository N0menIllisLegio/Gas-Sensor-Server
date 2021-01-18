﻿using System.Threading.Tasks;
using Gss.Core.DTOs;
using Gss.Core.DTOs.File;
using Gss.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Gss.Web.Controllers
{
  // TODO [Authorize]
  [Route("api/[controller]/[action]")]
  [ApiController]
  public class FilesController: ControllerBase
  {
    private readonly IFilesService _filesService;

    public FilesController(IFilesService filesService)
    {
      _filesService = filesService;
    }

    //[Authorize] Role = Administrator
    [HttpPost]
    [SwaggerOperation("Administrator Only", "Uploads image in cloud storage.")]
    [SwaggerResponse(200, type: typeof(Response<FileDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> AvatarUpload([FromForm] UploadFileDto dto)
    {
      var fileDto = await _filesService.UploadImages(dto);

      return Ok(new Response<FileDto>(fileDto));
    }

    //[Authorize] Role = Administrator
    [HttpDelete]
    [SwaggerOperation("Administrator Only", "Deletes image from cloud storage.")]
    [SwaggerResponse(200, type: typeof(Response<object>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> DeleteUserAvatar([FromBody] DeleteFileDto dto)
    {
      await _filesService.DeleteImage(dto);

      return Ok(new Response<object>());
    }

    [HttpPost]
    [SwaggerOperation("Authorized Only", "Loads data from files in database.")]
    public IActionResult MicrocontrollerDataUpload([FromForm] UploadFileDto dto)
    {
      return Ok(new Response<object>());
    }
  }
}
