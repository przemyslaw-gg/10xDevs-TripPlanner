using MediatR;
using Microsoft.EntityFrameworkCore;
using TripPlanner.Application.Common.Interfaces;

namespace TripPlanner.Application.Auth.Commands.Logout;

/// <summary>
/// Handler for <see cref="LogoutCommand"/>.
/// Revokes the specified refresh token.
/// </summary>
public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly ITokenService _tokenService;

    public LogoutCommandHandler(
        IApplicationDbContext context,
        ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    public async Task<Unit> Handle(
        LogoutCommand request,
        CancellationToken cancellationToken)
    {
        // Hash the incoming token for comparison
        var tokenHash = _tokenService.HashRefreshToken(request.RefreshToken);

        // Find the refresh token in database
        var storedToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash, cancellationToken);

        // If token exists and is not already revoked, revoke it
        if (storedToken != null && storedToken.RevokedAt == null)
        {
            storedToken.RevokedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
        }

        // Always return success (don't reveal whether token existed)
        return Unit.Value;
    }
}
