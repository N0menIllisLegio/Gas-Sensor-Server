using System.Security.Claims;
using Gss.Core.Interfaces;

namespace Gss.Web.Middlewares;

internal interface ICurrentUserDataSetter
{
    bool IsAuthorized { set; }
    Guid? Id { set; }
    string? Email { set; }
    string? GivenName { set; }
    string? Surname { set; }
}

internal sealed class CurrentUser : ICurrentUser, ICurrentUserDataSetter
{
    public bool IsAuthorized { get; set; }
    public Guid? Id { get; set; }
    public string? Email { get; set; }
    public string? GivenName { get; set; }
    public string? Surname { get; set; }
}

internal sealed class CurrentUserDataSetterMiddleware
{
    private readonly RequestDelegate _next;

    public CurrentUserDataSetterMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ICurrentUserDataSetter currentUserDataSetter)
    {
        var authorizedUser = context.User.Identity;
        var claims = context.User.Claims.ToList();

        if (authorizedUser is not null && authorizedUser.IsAuthenticated)
        {
            currentUserDataSetter.IsAuthorized = true;
            currentUserDataSetter.Id = Guid.Parse(claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
            currentUserDataSetter.Email = claims.First(x => x.Type == ClaimTypes.Email).Value;
            currentUserDataSetter.GivenName = claims.First(x => x.Type == ClaimTypes.GivenName).Value;
            currentUserDataSetter.Surname = claims.First(x => x.Type == ClaimTypes.Surname).Value;
        }

        await _next(context);
    }
}
