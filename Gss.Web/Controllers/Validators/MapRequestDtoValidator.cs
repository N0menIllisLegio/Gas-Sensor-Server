using FluentValidation;
using Gss.Core.DTOs.Microcontroller;

namespace Gss.Web.Controllers.Validators;

public sealed class MapRequestDtoValidator: AbstractValidator<MapRequestDto>
{
    public MapRequestDtoValidator()
    {
        RuleFor(x => x.NorthEastLatitude)
            .GreaterThanOrEqualTo(-90)
            .LessThanOrEqualTo(90);

        RuleFor(x => x.SouthWestLatitude)
            .GreaterThanOrEqualTo(-90)
            .LessThanOrEqualTo(90);

        RuleFor(x => x.NorthEastLongitude)
            .GreaterThanOrEqualTo(-180)
            .LessThanOrEqualTo(180);

        RuleFor(x => x.SouthWestLongitude)
            .GreaterThanOrEqualTo(-180)
            .LessThanOrEqualTo(180);
    }
}