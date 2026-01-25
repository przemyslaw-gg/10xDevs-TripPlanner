namespace TripPlanner.Domain.Entities;

/// <summary>
/// Junction entity linking trips with attractions, includes scheduling info.
/// </summary>
public class TripAttraction : BaseEntity
{
    public Guid TripId { get; set; }
    public Guid AttractionId { get; set; }
    public int DayNumber { get; set; }
    public int OrderIndex { get; set; }
    public TimeOnly? PlannedStartTime { get; set; }

    // Navigation properties
    public Trip Trip { get; set; } = null!;
    public Attraction Attraction { get; set; } = null!;
}
