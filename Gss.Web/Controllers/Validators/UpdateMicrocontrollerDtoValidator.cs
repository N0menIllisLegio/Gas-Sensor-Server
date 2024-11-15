using FluentValidation;
using Gss.Core.DTOs.Microcontroller;

namespace Gss.Web.Controllers.Validators;

public sealed class UpdateMicrocontrollerDtoValidator: AbstractValidator<UpdateMicrocontrollerDto>
{
    public UpdateMicrocontrollerDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        RuleFor(x => x.Public)
            .NotNull().WithMessage("Public flag is required.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");

        RuleFor(x => x.SensorIDs)
            .NotEmpty().WithMessage("SensorIDs must not be empty.")
            .Must(list => list.All(id => id != Guid.Empty)).WithMessage("SensorIDs must contain valid non-empty GUIDs.");

        // Optional checks for Latitude and Longitude if needed:
        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90.0, 90.0).When(x => x.Latitude.HasValue)
            .WithMessage("Latitude must be between -90 and 90.");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180.0, 180.0).When(x => x.Longitude.HasValue)
            .WithMessage("Longitude must be between -180 and 180.");
    }
}
