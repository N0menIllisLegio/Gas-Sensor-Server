using FluentValidation;
using Gss.Core.DTOs.Microcontroller;

namespace Gss.Web.Controllers.Validators;

public sealed class AddSensorDtoValidator: AbstractValidator<AddSensorDto>
{
    public AddSensorDtoValidator()
    {
        RuleFor(x => x.MicrocontollerID).NotEmpty();
        RuleFor(x => x.SensorID).NotEmpty();
    }
}
