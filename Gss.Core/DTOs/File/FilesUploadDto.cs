using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Gss.Core.DTOs.File
{
  public class FilesUploadDto
  {
    [Required]
    public IFormFileCollection FileForm { get; set; }
  }
}
