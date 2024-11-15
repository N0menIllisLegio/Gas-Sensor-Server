using FluentValidation;
using Gss.Core.DTOs.Authentication;

namespace Gss.Web.Controllers.Validators;

public sealed class ConfirmEmailDtoValidator: AbstractValidator<ConfirmEmailDto>
{
    public ConfirmEmailDtoValidator()
    {
        RuleFor(x => x.UserID).NotEmpty();
        RuleFor(x => x.Token).NotEmpty();
    }
}
