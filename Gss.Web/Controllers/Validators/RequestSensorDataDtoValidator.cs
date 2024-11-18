using FluentValidation;
using Gss.Core.DTOs.SensorData;

namespace Gss.Web.Controllers.Validators;

public sealed class RequestSensorDataDtoValidator: AbstractValidator<RequestSensorDataDto>
{
    public RequestSensorDataDtoValidator()
    {
        RuleFor(x => x.MicrocontrollerSensorId)
            .NotEmpty()
            .Must(x => x != Guid.Empty);

        RuleFor(x => x.WatchingDates)
            .NotEmpty()
            .Must(x => x.Count is > 0 and < 5)
            .WithMessage("WatchingDates should contain [1, 4] entries");
    }
}
