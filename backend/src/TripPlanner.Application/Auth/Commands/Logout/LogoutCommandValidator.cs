using FluentValidation;

namespace TripPlanner.Application.Auth.Commands.Logout;

/// <summary>
/// Validator for <see cref="LogoutCommand"/>.
/// </summary>
public class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage("Refresh token is required");
    }
}
