namespace Gss.Core.DTOs.User
{
  public class UpdateUserDto : UpdateUserInfoDto
  {
    public required string Email { get; set; }
  }
}
