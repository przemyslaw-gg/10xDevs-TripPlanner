namespace TripPlanner.Domain.Entities;

/// <summary>
/// Tourist attraction with geolocation and metadata.
/// </summary>
public class Attraction : BaseEntity
{
    public Guid LocationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public decimal? Rating { get; set; }
    public int? ReviewCount { get; set; }
    public int? EstimatedDuration { get; set; } // in minutes
    public string? ImageUrl { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public bool IsVerified { get; set; }

    // Navigation properties
    public Location Location { get; set; } = null!;
    public User? CreatedByUser { get; set; }
    public ICollection<TripAttraction> TripAttractions { get; set; } = new List<TripAttraction>();
}
