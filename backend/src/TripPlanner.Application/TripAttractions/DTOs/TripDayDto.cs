namespace TripPlanner.Application.TripAttractions.DTOs;

/// <summary>
/// Represents a single day in a trip schedule with its attractions.
/// </summary>
/// <param name="DayNumber">The day number (1-based).</param>
/// <param name="TotalDuration">Total estimated duration for this day in minutes.</param>
/// <param name="Attractions">List of attractions scheduled for this day, ordered by OrderIndex.</param>
public record TripDayDto(
    int DayNumber,
    int TotalDuration,
    IReadOnlyList<TripAttractionDetailDto> Attractions
);
