using FluentValidation;

namespace TripPlanner.Application.RouteOptimization.Commands.OptimizeRoute;

/// <summary>
/// Validator for <see cref="OptimizeRouteCommand"/>.
/// Ensures all required fields are provided.
/// </summary>
public class OptimizeRouteCommandValidator : AbstractValidator<OptimizeRouteCommand>
{
    public OptimizeRouteCommandValidator()
    {
        RuleFor(x => x.TripId)
            .NotEmpty()
            .WithMessage("Trip ID is required.");

        RuleFor(x => x.StartingAttractionId)
            .NotEmpty()
            .WithMessage("Starting attraction ID is required.");
    }
}
