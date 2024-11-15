using FluentValidation;
using Gss.Core.DTOs.Microcontroller;

namespace Gss.Web.Controllers.Validators;

public sealed class RemoveSensorDtoValidator: AbstractValidator<RemoveSensorDto>
{
    public RemoveSensorDtoValidator()
    {
        RuleFor(x => x.MicrocontollerID).NotEmpty();
        RuleFor(x => x.SensorID).NotEmpty();
    }
}
