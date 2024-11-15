namespace Gss.Core.DTOs.Authentication
{
  public class ChangePasswordDto
  {
    public Guid UserID { get; set; }
    public required string Token { get; set; }
    public required string Password { get; set; }
    public required string ConfirmPassword { get; set; }
  }
}
