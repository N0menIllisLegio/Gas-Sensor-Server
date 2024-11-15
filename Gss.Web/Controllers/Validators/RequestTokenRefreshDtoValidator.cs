using FluentValidation;
using Gss.Core.DTOs.Authentication;

namespace Gss.Web.Controllers.Validators;

public sealed class RequestTokenRefreshDtoValidator: AbstractValidator<RequestTokenRefreshDto>
{
    public RequestTokenRefreshDtoValidator()
    {
        RuleFor(x => x.AccessToken).NotEmpty();
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}
