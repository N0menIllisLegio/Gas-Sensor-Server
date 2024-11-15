using FluentValidation;
using Gss.Core.DTOs.User;

namespace Gss.Web.Controllers.Validators;

public sealed class UpdateUserInfoDtoValidator: AbstractValidator<UpdateUserInfoDto>
{
    public UpdateUserInfoDtoValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\+?[1-9]\d{1,14}$").When(x => !string.IsNullOrEmpty(x.PhoneNumber))
            .WithMessage("Phone number must be a valid phone number.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First Name is required.")
            .Length(2, 20).WithMessage("First Name must be between 2 and 20 characters.");
    }
}
