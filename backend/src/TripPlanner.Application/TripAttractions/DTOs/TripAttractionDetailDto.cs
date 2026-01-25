namespace TripPlanner.Application.TripAttractions.DTOs;

/// <summary>
/// Detailed trip attraction information including full attraction data.
/// Used in trip schedule responses.
/// </summary>
/// <param name="Id">The unique identifier of the trip attraction record.</param>
/// <param name="AttractionId">The ID of the attraction.</param>
/// <param name="Attraction">Summary information about the attraction.</param>
/// <param name="DayNumber">The day number within the trip (1-based).</param>
/// <param name="OrderIndex">The order of the attraction within the day (1-based).</param>
/// <param name="PlannedStartTime">The planned start time for visiting this attraction.</param>
public record TripAttractionDetailDto(
    Guid Id,
    Guid AttractionId,
    AttractionSummaryDto Attraction,
    int DayNumber,
    int OrderIndex,
    TimeOnly? PlannedStartTime
);
