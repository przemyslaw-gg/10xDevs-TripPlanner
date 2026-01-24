namespace TripPlanner.Domain.Entities;

/// <summary>
/// User account entity with authentication data.
/// </summary>
public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public bool EmailVerified { get; set; }

    // Navigation properties
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public ICollection<Trip> Trips { get; set; } = new List<Trip>();
    public ICollection<Attraction> CreatedAttractions { get; set; } = new List<Attraction>();
}
