using FluentValidation;
using Gss.Core.DTOs.Authentication;

namespace Gss.Web.Controllers.Validators;

public sealed class ChangeEmailDtoValidator: AbstractValidator<ChangeEmailDto>
{
    public ChangeEmailDtoValidator()
    {
        RuleFor(x => x.Token).NotEmpty();
        RuleFor(x => x.NewEmail).NotEmpty();
        RuleFor(x => x.UserID).NotEmpty();
    }
}
