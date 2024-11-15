using FluentValidation;
using Gss.Core.DTOs.Microcontroller;

namespace Gss.Web.Controllers.Validators;

public sealed class ChangeMicrocontrollerOwnerDtoValidator: AbstractValidator<ChangeMicrocontrollerOwnerDto>
{
    public ChangeMicrocontrollerOwnerDtoValidator()
    {
        RuleFor(x => x.MicrocontrollerID).NotEmpty();
        RuleFor(x => x.UserID).NotEmpty();
    }
}
