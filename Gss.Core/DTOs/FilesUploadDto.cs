using Microsoft.AspNetCore.Http;

namespace Gss.Core.DTOs
{
  public class FilesUploadDto
  {
    public IFormFileCollection FileForm { get; set; }
  }
}
