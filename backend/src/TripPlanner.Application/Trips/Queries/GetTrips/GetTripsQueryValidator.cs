using FluentValidation;

namespace TripPlanner.Application.Trips.Queries.GetTrips;

/// <summary>
/// Validator for <see cref="GetTripsQuery"/>.
/// Validates pagination parameters and search term length.
/// </summary>
public class GetTripsQueryValidator : AbstractValidator<GetTripsQuery>
{
    public GetTripsQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page must be greater than or equal to 1");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("PageSize must be between 1 and 100");

        RuleFor(x => x.Search)
            .MaximumLength(100)
            .WithMessage("Search term must not exceed 100 characters")
            .When(x => x.Search != null);
    }
}
