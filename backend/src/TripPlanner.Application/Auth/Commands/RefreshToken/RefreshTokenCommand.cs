using MediatR;
using TripPlanner.Application.Auth.DTOs;

namespace TripPlanner.Application.Auth.Commands.RefreshToken;

/// <summary>
/// Command to refresh an access token using a valid refresh token.
/// </summary>
/// <param name="RefreshToken">The refresh token to use.</param>
public record RefreshTokenCommand(
    string RefreshToken
) : IRequest<TokenResponseDto>;
