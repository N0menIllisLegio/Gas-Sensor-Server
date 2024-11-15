using FluentValidation;
using Gss.Core.DTOs.Sensor;

namespace Gss.Web.Controllers.Validators;

public sealed class SetSensorTypeDtoValidator: AbstractValidator<SetSensorTypeDto>
{
    public SetSensorTypeDtoValidator()
    {
        RuleFor(x => x.SensorID).NotEmpty();
        RuleFor(x => x.SensorTypeID).NotEmpty();
    }
}
