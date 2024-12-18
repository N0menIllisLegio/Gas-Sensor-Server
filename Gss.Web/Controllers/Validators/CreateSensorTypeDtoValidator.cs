using FluentValidation;
using Gss.Core.DTOs.SensorType;

namespace Gss.Web.Controllers.Validators;

public sealed class CreateSensorTypeDtoValidator : AbstractValidator<CreateSensorTypeDto>
{
    public CreateSensorTypeDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        RuleFor(x => x.Icon)
            .MaximumLength(8000).WithMessage("Icon must not exceed 8000 characters.")
            .When(x => x.Icon != null);

        RuleFor(x => x.Units)
            .MaximumLength(20).WithMessage("Units must not exceed 20 characters.")
            .When(x => x.Units != null);
    }
}