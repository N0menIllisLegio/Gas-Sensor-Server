using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Gss.Core.DTOs
{
  public class FilesUploadDto
  {
    [Required]
    public IFormFileCollection FileForm { get; set; }
  }
}
