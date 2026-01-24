using MediatR;
using TripPlanner.Application.Auth.DTOs;

namespace TripPlanner.Application.Auth.Commands.Login;

/// <summary>
/// Command to authenticate a user and issue tokens.
/// </summary>
/// <param name="Email">The user's email address.</param>
/// <param name="Password">The user's password.</param>
public record LoginCommand(
    string Email,
    string Password
) : IRequest<AuthResponseDto>;
