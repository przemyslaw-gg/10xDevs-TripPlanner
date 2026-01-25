using MediatR;

namespace TripPlanner.Application.Auth.Commands.Logout;

/// <summary>
/// Command to logout a user by revoking their refresh token.
/// </summary>
/// <param name="RefreshToken">The refresh token to revoke.</param>
public record LogoutCommand(
    string RefreshToken
) : IRequest<Unit>;
