using FluentValidation;

namespace TripPlanner.Application.TripAttractions.Commands.UpdateTripAttraction;

/// <summary>
/// Validator for <see cref="UpdateTripAttractionCommand"/>.
/// </summary>
public class UpdateTripAttractionCommandValidator : AbstractValidator<UpdateTripAttractionCommand>
{
    public UpdateTripAttractionCommandValidator()
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

        RuleFor(x => x.PlannedStartTime)
            .Must(BeValidTimeFormatOrNull)
            .WithMessage("Planned start time must be in HH:mm format");
    }

    private static bool BeValidTimeFormatOrNull(string? time)
    {
        if (string.IsNullOrEmpty(time))
            return true;

        return TimeOnly.TryParse(time, out _);
    }
}
