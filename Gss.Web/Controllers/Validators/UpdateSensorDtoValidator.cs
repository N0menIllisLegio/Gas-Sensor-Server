using FluentValidation;
using Gss.Core.DTOs.Sensor;

namespace Gss.Web.Controllers.Validators;

public sealed class UpdateSensorDtoValidator: AbstractValidator<UpdateSensorDto>
{
    public UpdateSensorDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1800).WithMessage("Description must not exceed 1800 characters.")
            .When(x => x.Description != null);

        RuleFor(x => x.TypeID)
            .NotEmpty().WithMessage("TypeID is required.");
    }
}
