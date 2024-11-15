using FluentValidation;
using Gss.Core.DTOs;

namespace Gss.Web.Controllers.Validators;

public sealed class EmailDtoValidator: AbstractValidator<EmailDto>
{
    public EmailDtoValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}
