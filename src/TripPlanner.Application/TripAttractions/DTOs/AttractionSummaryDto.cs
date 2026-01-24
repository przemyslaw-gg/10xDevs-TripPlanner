namespace TripPlanner.Application.TripAttractions.DTOs;

/// <summary>
/// Summary information about an attraction for use in trip schedules.
/// Contains only essential data needed for trip planning views.
/// </summary>
/// <param name="Id">The unique identifier of the attraction.</param>
/// <param name="Name">The name of the attraction.</param>
/// <param name="Latitude">The latitude coordinate of the attraction.</param>
/// <param name="Longitude">The longitude coordinate of the attraction.</param>
/// <param name="Rating">The average rating of the attraction (0-5). Null for unrated attractions.</param>
/// <param name="EstimatedDuration">Estimated visit duration in minutes.</param>
/// <param name="ImageUrl">URL to the attraction's image.</param>
public record AttractionSummaryDto(
    Guid Id,
    string Name,
    decimal Latitude,
    decimal Longitude,
    decimal? Rating,
    int EstimatedDuration,
    string? ImageUrl
);
