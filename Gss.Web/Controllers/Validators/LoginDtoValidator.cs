using FluentValidation;
using Gss.Core.DTOs.Authentication;

namespace Gss.Web.Controllers.Validators;

public sealed class LoginDtoValidator: AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Login).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}
