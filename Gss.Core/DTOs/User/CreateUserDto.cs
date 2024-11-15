namespace Gss.Core.DTOs.User
{
  public class CreateUserDto
  {
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string ConfirmPassword { get; set; }
    public required string PhoneNumber { get; set; }
    public required string FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Gender { get; set; }
    public DateTimeOffset? Birthday { get; set; }
  }
}
