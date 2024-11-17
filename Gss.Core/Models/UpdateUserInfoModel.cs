namespace Gss.Core.Models;

public class UpdateUserInfoModel
{
  public string? AvatarPath { get; set; }
  public required string Email { get; set; }
  public string? PhoneNumber { get; set; }
  public required string FirstName { get; set; }
  public string? LastName { get; set; }
  public string? Gender { get; set; }
  public DateTimeOffset? Birthday { get; set; }
}