using FluentValidation;
using Gss.Core.DTOs.Microcontroller;

namespace Gss.Web.Controllers.Validators;

public sealed class SetSensorsCriticalValueDtoValidator: AbstractValidator<SetSensorsCriticalValueDto>
{
    public SetSensorsCriticalValueDtoValidator()
    {
        RuleFor(x => x.MicrocontrollerID).NotEmpty();
        RuleFor(x => x.SensorID).NotEmpty();
    }
}
