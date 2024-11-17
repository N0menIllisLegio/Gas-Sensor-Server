namespace Gss.Core.Interfaces;

public interface ICurrentUser
{
    public bool IsAuthorized { get; }
    public Guid? Id { get; }
    public string? Email { get; }
    public string? GivenName { get; }
    public string? Surname { get; }
}
