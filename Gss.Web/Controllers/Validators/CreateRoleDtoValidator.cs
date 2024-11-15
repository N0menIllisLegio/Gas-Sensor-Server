using FluentValidation;
using Gss.Core.DTOs.Role;

namespace Gss.Web.Controllers.Validators;

public sealed class CreateRoleDtoValidator: AbstractValidator<CreateRoleDto>
{
    public CreateRoleDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(3).WithMessage("Role length can't be less than 3.");
    }
}
