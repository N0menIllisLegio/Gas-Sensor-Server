using System.ComponentModel.DataAnnotations;

namespace Gss.Core.DTOs.Authentication
{
  public class RequestTokenRefreshDto
  {
    [Required]
    public string AccessToken { get; set; }

    [Required]
    public string RefreshToken { get; set; }
  }
}
