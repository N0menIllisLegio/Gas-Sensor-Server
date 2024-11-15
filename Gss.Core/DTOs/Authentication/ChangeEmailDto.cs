namespace Gss.Core.DTOs.Authentication
{
  public class ChangeEmailDto
  {
    public Guid UserID { get; set; }
    public required string NewEmail { get; set; }
    public required string Token { get; set; }
  }
}
