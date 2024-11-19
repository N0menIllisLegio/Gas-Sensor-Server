using FluentValidation;
using Gss.Core.DTOs.Microcontroller;

namespace Gss.Web.Controllers.Validators;

public sealed class MapRequestDtoValidator: AbstractValidator<MapRequestDto>
{
    public MapRequestDtoValidator()
    {
        RuleFor(x => x.NorthEastLatitude)
            .LessThan(-90)
            .GreaterThan(90);

        RuleFor(x => x.SouthWestLatitude)
            .LessThan(-90)
            .GreaterThan(90);

        RuleFor(x => x.NorthEastLongitude)
            .LessThan(-180)
            .GreaterThan(180);

        RuleFor(x => x.SouthWestLongitude)
            .LessThan(-180)
            .GreaterThan(180);
    }
}