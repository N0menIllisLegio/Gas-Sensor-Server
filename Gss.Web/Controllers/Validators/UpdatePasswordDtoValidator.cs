using FluentValidation;
using Gss.Core.DTOs.User;

namespace Gss.Web.Controllers.Validators;

public sealed class UpdatePasswordDtoValidator: AbstractValidator<UpdatePasswordDto>
{
    public UpdatePasswordDtoValidator()
    {
        RuleFor(x => x.UserID).NotEmpty();
        RuleFor(x => x.NewPassword).NotEmpty().Length(4, 20);
    }
}
