using FluentValidation;

namespace TripPlanner.Application.Trips.Commands.UpdateTrip;

/// <summary>
/// Validator for <see cref="UpdateTripCommand"/>.
/// </summary>
public class UpdateTripCommandValidator : AbstractValidator<UpdateTripCommand>
{
    public UpdateTripCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Trip ID is required");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(100)
            .WithMessage("Name must not exceed 100 characters");

        RuleFor(x => x.DailyHours)
            .InclusiveBetween(1, 24)
            .WithMessage("Daily hours must be between 1 and 24");

        RuleFor(x => x.MaxExtensionHours)
            .InclusiveBetween(0, 8)
            .WithMessage("Max extension hours must be between 0 and 8");

        RuleFor(x => x.StartTime)
            .NotEmpty()
            .WithMessage("Start time is required")
            .Must(BeValidTimeFormat)
            .WithMessage("Start time must be in HH:mm format");
    }

    private static bool BeValidTimeFormat(string time)
    {
        if (string.IsNullOrEmpty(time))
            return false;

        return TimeOnly.TryParse(time, out _);
    }
}
