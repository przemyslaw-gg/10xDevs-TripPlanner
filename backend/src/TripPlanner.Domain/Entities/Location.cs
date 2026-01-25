namespace TripPlanner.Domain.Entities;

/// <summary>
/// Tourist destination (city, region).
/// </summary>
public class Location : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string? Timezone { get; set; }

    // Navigation properties
    public ICollection<Attraction> Attractions { get; set; } = new List<Attraction>();
    public ICollection<Trip> Trips { get; set; } = new List<Trip>();
}
