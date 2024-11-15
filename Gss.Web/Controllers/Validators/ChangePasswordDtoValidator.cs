using FluentValidation;
using Gss.Core.DTOs.Authentication;

namespace Gss.Web.Controllers.Validators;

public sealed class ChangePasswordDtoValidator: AbstractValidator<ChangePasswordDto>
{
    public ChangePasswordDtoValidator()
    {
        RuleFor(x => x.UserID)
            .NotEmpty().WithMessage("UserID is required.")
            .NotEqual(Guid.Empty).WithMessage("UserID must be a valid non-empty GUID.");

        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token is required.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .Length(4, 20).WithMessage("Password must be between 4 and 20 characters.");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Confirm Password is required.")
            .Equal(x => x.Password).WithMessage("Confirm Password must match Password.")
            .Length(4, 20).WithMessage("Confirm Password must be between 4 and 20 characters.");
    }
}
