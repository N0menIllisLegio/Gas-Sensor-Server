using FluentValidation;
using Gss.Core.DTOs.User;

namespace Gss.Web.Controllers.Validators;

public sealed class UpdateUserDtoValidator: AbstractValidator<UpdateUserDto>
{
    public UpdateUserDtoValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}
