using System;
using System.ComponentModel.DataAnnotations;

namespace Gss.Core.DTOs
{
  public class IdDto
  {
    [Required]
    public Guid ID { get; set; }
  }
}
