namespace TripPlanner.Domain.Entities;

/// <summary>
/// User profile entity extending Supabase Auth user data.
/// </summary>
public class Profile : BaseEntity
{
    public string? DisplayName { get; set; }

    // Navigation properties
    public ICollection<Trip> Trips { get; set; } = new List<Trip>();
    public ICollection<Attraction> CreatedAttractions { get; set; } = new List<Attraction>();
}
