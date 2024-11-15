using FluentValidation;
using Gss.Core.DTOs.Role;

namespace Gss.Web.Controllers.Validators;

public sealed class UpdateRoleDtoValidator: AbstractValidator<UpdateRoleDto>
{
    public UpdateRoleDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}
