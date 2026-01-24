namespace TripPlanner.Application.TripAttractions.DTOs;

/// <summary>
/// Complete trip schedule with attractions grouped by days.
/// Response for GET /api/trips/{tripId}/attractions.
/// </summary>
/// <param name="TripId">The ID of the trip.</param>
/// <param name="TotalDays">Total number of days in the schedule.</param>
/// <param name="TotalDuration">Total estimated duration for all attractions in minutes.</param>
/// <param name="Days">List of days with their attractions.</param>
public record TripScheduleDto(
    Guid TripId,
    int TotalDays,
    int TotalDuration,
    IReadOnlyList<TripDayDto> Days
);
