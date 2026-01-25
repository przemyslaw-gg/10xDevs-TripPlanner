namespace TripPlanner.Application.TripAttractions.DTOs;

/// <summary>
/// Data transfer object for a trip attraction assignment.
/// Used as response for POST and PUT operations.
/// </summary>
/// <param name="Id">The unique identifier of the trip attraction record.</param>
/// <param name="TripId">The ID of the trip.</param>
/// <param name="AttractionId">The ID of the attraction.</param>
/// <param name="DayNumber">The day number within the trip (1-based).</param>
/// <param name="OrderIndex">The order of the attraction within the day (1-based).</param>
/// <param name="PlannedStartTime">The planned start time for visiting this attraction.</param>
/// <param name="CreatedAt">When the assignment was created.</param>
/// <param name="UpdatedAt">When the assignment was last updated.</param>
public record TripAttractionDto(
    Guid Id,
    Guid TripId,
    Guid AttractionId,
    int DayNumber,
    int OrderIndex,
    TimeOnly? PlannedStartTime,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
