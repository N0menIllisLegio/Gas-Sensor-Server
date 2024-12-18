using FluentValidation;
using Gss.Core.DTOs;

namespace Gss.Web.Controllers.Validators;

public sealed class PagedInfoDtoValidator : AbstractValidator<PagedInfoDto>
{
    public PagedInfoDtoValidator()
    {
        RuleFor(x => x.PageNumber)
            .NotEmpty()
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .NotEmpty()
            .GreaterThanOrEqualTo(10)
            .LessThanOrEqualTo(50);
    }
}