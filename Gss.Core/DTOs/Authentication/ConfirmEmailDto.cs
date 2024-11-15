namespace Gss.Core.DTOs.Authentication
{
  public class ConfirmEmailDto
  {
    public Guid UserID { get; set; }
    public required string Token { get; set; }
  }
}
