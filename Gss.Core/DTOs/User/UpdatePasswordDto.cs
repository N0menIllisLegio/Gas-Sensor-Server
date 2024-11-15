namespace Gss.Core.DTOs.User
{
  public class UpdatePasswordDto
  {
    public Guid UserID { get; set; }
    public required string NewPassword { get; set; }
  }
}
