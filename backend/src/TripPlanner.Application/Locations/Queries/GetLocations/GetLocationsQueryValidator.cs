using FluentValidation;

namespace TripPlanner.Application.Locations.Queries.GetLocations;

/// <summary>
/// Validator for <see cref="GetLocationsQuery"/>.
/// Ensures pagination parameters are within valid ranges.
/// </summary>
public class GetLocationsQueryValidator : AbstractValidator<GetLocationsQuery>
{
    public GetLocationsQueryValidator()
    {
        RuleFor(x => x.Search)
            .MaximumLength(100)
            .WithMessage("Search term must not exceed 100 characters");

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page must be greater than or equal to 1");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("PageSize must be between 1 and 100");
    }
}
