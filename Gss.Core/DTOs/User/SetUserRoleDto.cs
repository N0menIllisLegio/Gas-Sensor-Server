namespace Gss.Core.DTOs.User
{
  public class SetUserRoleDto
  {
    public required Guid UserID { get; set; }
    public required string RoleName { get; set; }
  }
}
