using FluentValidation;

namespace TripPlanner.Application.TripAttractions.Commands.AddTripAttraction;

/// <summary>
/// Validator for <see cref="AddTripAttractionCommand"/>.
/// </summary>
public class AddTripAttractionCommandValidator : AbstractValidator<AddTripAttractionCommand>
{
    public AddTripAttractionCommandValidator()
    {
        RuleFor(x => x.TripId)
            .NotEmpty()
            .WithMessage("Trip ID is required");

        RuleFor(x => x.AttractionId)
            .NotEmpty()
            .WithMessage("Attraction ID is required");

        RuleFor(x => x.DayNumber)
            .GreaterThan(0)
            .WithMessage("Day number must be greater than 0");

        RuleFor(x => x.OrderIndex)
            .GreaterThan(0)
            .WithMessage("Order index must be greater than 0");
    }
}
