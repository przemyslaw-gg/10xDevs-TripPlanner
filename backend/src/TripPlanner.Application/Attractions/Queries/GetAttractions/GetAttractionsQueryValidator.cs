using FluentValidation;

namespace TripPlanner.Application.Attractions.Queries.GetAttractions;

/// <summary>
/// Validator for <see cref="GetAttractionsQuery"/>.
/// Ensures all parameters are within valid ranges and values.
/// </summary>
public class GetAttractionsQueryValidator : AbstractValidator<GetAttractionsQuery>
{
    private static readonly string[] AllowedSortFields = { "rating", "name", "reviewcount" };
    private static readonly string[] AllowedSortOrders = { "asc", "desc" };

    public GetAttractionsQueryValidator()
    {
        RuleFor(x => x.Search)
            .MaximumLength(100)
            .WithMessage("Search term must not exceed 100 characters");

        RuleFor(x => x.SortBy)
            .Must(value => AllowedSortFields.Contains(value.ToLowerInvariant()))
            .WithMessage("SortBy must be one of: rating, name, reviewCount");

        RuleFor(x => x.SortOrder)
            .Must(value => AllowedSortOrders.Contains(value.ToLowerInvariant()))
            .WithMessage("SortOrder must be 'asc' or 'desc'");

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page must be greater than or equal to 1");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("PageSize must be between 1 and 100");
    }
}
