using MediatR;
using TripPlanner.Application.Auth.DTOs;

namespace TripPlanner.Application.Auth.Commands.RegisterUser;

/// <summary>
/// Command to register a new user.
/// </summary>
/// <param name="Email">The user's email address.</param>
/// <param name="Password">The user's password.</param>
/// <param name="DisplayName">Optional display name.</param>
public record RegisterUserCommand(
    string Email,
    string Password,
    string? DisplayName
) : IRequest<RegisteredUserDto>;
