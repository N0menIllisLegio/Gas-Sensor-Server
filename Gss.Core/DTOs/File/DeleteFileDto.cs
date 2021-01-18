using System;
using System.ComponentModel.DataAnnotations;

namespace Gss.Core.DTOs.File
{
  public class DeleteFileDto
  {
    [Required]
    public Uri FileUrl { get; set; }
  }
}
