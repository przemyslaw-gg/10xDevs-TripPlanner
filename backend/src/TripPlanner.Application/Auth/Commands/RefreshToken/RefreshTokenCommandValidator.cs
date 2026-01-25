using FluentValidation;

namespace TripPlanner.Application.Auth.Commands.RefreshToken;

/// <summary>
/// Validator for <see cref="RefreshTokenCommand"/>.
/// </summary>
public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage("Refresh token is required");
    }
}
