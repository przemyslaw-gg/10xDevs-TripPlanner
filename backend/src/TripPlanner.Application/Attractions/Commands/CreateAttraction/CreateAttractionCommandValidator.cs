using FluentValidation;

namespace TripPlanner.Application.Attractions.Commands.CreateAttraction;

/// <summary>
/// Validator for <see cref="CreateAttractionCommand"/>.
/// Ensures all required fields are valid and within acceptable ranges.
/// </summary>
public class CreateAttractionCommandValidator : AbstractValidator<CreateAttractionCommand>
{
    public CreateAttractionCommandValidator()
    {
        RuleFor(x => x.LocationId)
            .NotEmpty()
            .WithMessage("LocationId is required");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(255)
            .WithMessage("Name must not exceed 255 characters");

        RuleFor(x => x.Description)
            .MaximumLength(5000)
            .WithMessage("Description must not exceed 5000 characters")
            .When(x => x.Description != null);

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90m, 90m)
            .WithMessage("Latitude must be between -90 and 90");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180m, 180m)
            .WithMessage("Longitude must be between -180 and 180");

        RuleFor(x => x.EstimatedDuration)
            .GreaterThan(0)
            .WithMessage("EstimatedDuration must be greater than 0")
            .When(x => x.EstimatedDuration.HasValue);

        RuleFor(x => x.ImageUrl)
            .Must(BeAValidUrl)
            .WithMessage("ImageUrl must be a valid URL")
            .When(x => !string.IsNullOrEmpty(x.ImageUrl));
    }

    private static bool BeAValidUrl(string? url)
    {
        if (string.IsNullOrEmpty(url))
            return true;

        return Uri.TryCreate(url, UriKind.Absolute, out var result)
               && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }
}
