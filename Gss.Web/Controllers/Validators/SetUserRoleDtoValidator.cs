using FluentValidation;
using Gss.Core.DTOs.User;

namespace Gss.Web.Controllers.Validators;

public sealed class SetUserRoleDtoValidator: AbstractValidator<SetUserRoleDto>
{
    public SetUserRoleDtoValidator()
    {
        RuleFor(x => x.RoleName).NotEmpty();
        RuleFor(x => x.UserID).NotEmpty();
    }
}
