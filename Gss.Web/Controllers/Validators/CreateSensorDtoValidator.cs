using FluentValidation;
using Gss.Core.DTOs.Sensor;

namespace Gss.Web.Controllers.Validators;

public sealed class CreateSensorDtoValidator: AbstractValidator<CreateSensorDto>
{
    public CreateSensorDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1800).WithMessage("Description must not exceed 1800 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.TypeId)
            .NotEmpty().WithMessage("TypeID is required.")
            .Must(id => id != Guid.Empty).WithMessage("TypeID must be a valid non-empty GUID.");
    }
}
