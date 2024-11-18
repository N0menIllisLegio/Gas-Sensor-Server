namespace Gss.Web.CurrentUser;

internal interface ICurrentUserDataSetter
{
    bool IsAuthorized { set; }
    Guid? Id { set; }
    string? Email { set; }
    string? GivenName { set; }
    string? Surname { set; }
    bool IsAdministrator { set; }
}