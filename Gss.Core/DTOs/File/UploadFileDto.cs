using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Gss.Core.DTOs.File
{
  public class UploadFileDto
  {
    [Required]
    public IFormFile FileForm { get; set; }
  }
}
