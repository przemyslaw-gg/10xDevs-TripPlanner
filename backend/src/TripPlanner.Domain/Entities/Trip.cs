namespace TripPlanner.Domain.Entities;

/// <summary>
/// User trip plan with scheduling parameters.
/// </summary>
public class Trip : BaseEntity
{
    public Guid OwnerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid? LocationId { get; set; }
    public bool IsPublic { get; set; }
    public int DailyHours { get; set; } = 8;
    public int MaxExtensionHours { get; set; } = 2;
    public TimeOnly StartTime { get; set; } = new TimeOnly(9, 0);

    // Navigation properties
    public User Owner { get; set; } = null!;
    public Location? Location { get; set; }
    public ICollection<TripAttraction> TripAttractions { get; set; } = new List<TripAttraction>();
}
