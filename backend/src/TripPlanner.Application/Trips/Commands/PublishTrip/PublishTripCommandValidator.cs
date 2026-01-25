using FluentValidation;

namespace TripPlanner.Application.Trips.Commands.PublishTrip;

/// <summary>
/// Validator for <see cref="PublishTripCommand"/>.
/// </summary>
public class PublishTripCommandValidator : AbstractValidator<PublishTripCommand>
{
    public PublishTripCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Trip ID is required");
    }
}
