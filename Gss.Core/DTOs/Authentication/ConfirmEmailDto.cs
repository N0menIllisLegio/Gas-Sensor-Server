using System;
using System.ComponentModel.DataAnnotations;

namespace Gss.Core.DTOs.Authentication
{
  public class ConfirmEmailDto
  {
    [Required]
    public Guid UserID { get; set; }

    [Required]
    public string Token { get; set; }
  }
}
