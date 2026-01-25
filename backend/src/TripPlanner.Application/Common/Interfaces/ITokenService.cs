using TripPlanner.Domain.Entities;

namespace TripPlanner.Application.Common.Interfaces;

/// <summary>
/// Interface for JWT and refresh token operations.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generates a JWT access token for the specified user.
    /// </summary>
    /// <param name="user">The user to generate the token for.</param>
    /// <returns>The JWT access token string.</returns>
    string GenerateAccessToken(User user);

    /// <summary>
    /// Generates a cryptographically secure refresh token.
    /// </summary>
    /// <returns>The refresh token string.</returns>
    string GenerateRefreshToken();

    /// <summary>
    /// Creates a SHA256 hash of the refresh token for secure storage.
    /// </summary>
    /// <param name="token">The refresh token to hash.</param>
    /// <returns>The hashed token.</returns>
    string HashRefreshToken(string token);

    /// <summary>
    /// Validates a refresh token against its stored hash using constant-time comparison.
    /// </summary>
    /// <param name="token">The refresh token to validate.</param>
    /// <param name="hash">The stored hash to validate against.</param>
    /// <returns>True if the token matches the hash, false otherwise.</returns>
    bool ValidateRefreshTokenHash(string token, string hash);

    /// <summary>
    /// Gets the access token expiration time in seconds.
    /// </summary>
    int AccessTokenExpirationSeconds { get; }

    /// <summary>
    /// Gets the refresh token expiration time in days.
    /// </summary>
    int RefreshTokenExpirationDays { get; }
}
