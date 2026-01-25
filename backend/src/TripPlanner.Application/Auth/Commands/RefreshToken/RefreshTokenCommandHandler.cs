using MediatR;
using Microsoft.EntityFrameworkCore;
using TripPlanner.Application.Auth.DTOs;
using TripPlanner.Application.Common.Interfaces;

namespace TripPlanner.Application.Auth.Commands.RefreshToken;

/// <summary>
/// Handler for <see cref="RefreshTokenCommand"/>.
/// Validates refresh token, revokes old token, and issues new token pair.
/// </summary>
public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, TokenResponseDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ITokenService _tokenService;

    public RefreshTokenCommandHandler(
        IApplicationDbContext context,
        ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    public async Task<TokenResponseDto> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        // Hash the incoming token for comparison
        var tokenHash = _tokenService.HashRefreshToken(request.RefreshToken);

        // Find the refresh token in database
        var storedToken = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash, cancellationToken);

        if (storedToken == null)
        {
            throw new UnauthorizedAccessException("Invalid refresh token");
        }

        // Validate the token is still valid
        if (!storedToken.IsValid)
        {
            throw new UnauthorizedAccessException("Refresh token is expired or revoked");
        }

        // Revoke the old token
        storedToken.RevokedAt = DateTime.UtcNow;

        // Generate new tokens
        var accessToken = _tokenService.GenerateAccessToken(storedToken.User);
        var newRefreshToken = _tokenService.GenerateRefreshToken();
        var newRefreshTokenHash = _tokenService.HashRefreshToken(newRefreshToken);

        // Store new refresh token
        var newRefreshTokenEntity = new Domain.Entities.RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = storedToken.UserId,
            TokenHash = newRefreshTokenHash,
            ExpiresAt = DateTime.UtcNow.AddDays(_tokenService.RefreshTokenExpirationDays),
            RevokedAt = null
        };

        _context.RefreshTokens.Add(newRefreshTokenEntity);
        await _context.SaveChangesAsync(cancellationToken);

        return new TokenResponseDto(
            accessToken,
            newRefreshToken,
            _tokenService.AccessTokenExpirationSeconds,
            "Bearer"
        );
    }
}
