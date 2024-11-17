using Gss.Core.Interfaces;

namespace Gss.Web.CurrentUser;

internal sealed class CurrentUser : ICurrentUser, ICurrentUserDataSetter
{
    public bool IsAuthorized { get; set; }
    public Guid? Id { get; set; }
    public string? Email { get; set; }
    public string? GivenName { get; set; }
    public string? Surname { get; set; }
}