namespace TripPlanner.Domain.Entities;

/// <summary>
/// Refresh token for OAuth 2.0 authentication.
/// </summary>
public class RefreshToken
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;

    /// <summary>
    /// Check if the token is valid (not expired and not revoked).
    /// </summary>
    public bool IsValid => RevokedAt == null && ExpiresAt > DateTime.UtcNow;
}
