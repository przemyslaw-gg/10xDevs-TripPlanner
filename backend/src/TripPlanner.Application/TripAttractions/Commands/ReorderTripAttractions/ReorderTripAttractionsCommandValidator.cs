using FluentValidation;

namespace TripPlanner.Application.TripAttractions.Commands.ReorderTripAttractions;

/// <summary>
/// Validator for <see cref="ReorderTripAttractionsCommand"/>.
/// </summary>
public class ReorderTripAttractionsCommandValidator : AbstractValidator<ReorderTripAttractionsCommand>
{
    public ReorderTripAttractionsCommandValidator()
    {
        RuleFor(x => x.TripId)
            .NotEmpty()
            .WithMessage("Trip ID is required");

        RuleFor(x => x.Attractions)
            .NotEmpty()
            .WithMessage("At least one attraction must be provided for reordering");

        RuleForEach(x => x.Attractions)
            .ChildRules(item =>
            {
                item.RuleFor(x => x.AttractionId)
                    .NotEmpty()
                    .WithMessage("Attraction ID is required");

                item.RuleFor(x => x.DayNumber)
                    .GreaterThan(0)
                    .WithMessage("Day number must be greater than 0");

                item.RuleFor(x => x.OrderIndex)
                    .GreaterThanOrEqualTo(0)
                    .WithMessage("Order index must be 0 or greater");
            });
    }
}
