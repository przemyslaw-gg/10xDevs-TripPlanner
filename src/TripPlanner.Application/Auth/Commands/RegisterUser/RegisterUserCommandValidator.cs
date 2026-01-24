using FluentValidation;

namespace TripPlanner.Application.Auth.Commands.RegisterUser;

/// <summary>
/// Validator for <see cref="RegisterUserCommand"/>.
/// Validates email format, password complexity, and optional display name.
/// </summary>
public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .MaximumLength(255)
            .WithMessage("Email must not exceed 255 characters")
            .EmailAddress()
            .WithMessage("Email must be a valid email address");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long")
            .Matches("[A-Z]")
            .WithMessage("Password must contain at least one uppercase letter")
            .Matches("[a-z]")
            .WithMessage("Password must contain at least one lowercase letter")
            .Matches("[0-9]")
            .WithMessage("Password must contain at least one digit");

        RuleFor(x => x.DisplayName)
            .MaximumLength(100)
            .WithMessage("Display name must not exceed 100 characters")
            .When(x => x.DisplayName != null);
    }
}
