using TripPlanner.Application.Attractions.DTOs;

namespace TripPlanner.Application.Trips.DTOs;

/// <summary>
/// Trip list item DTO for paginated list responses (GET /).
/// Includes aggregated data like attraction count and total days.
/// </summary>
/// <param name="Id">The unique identifier of the trip.</param>
/// <param name="OwnerId">The unique identifier of the trip owner.</param>
/// <param name="Name">The name of the trip.</param>
/// <param name="LocationId">The unique identifier of the location (optional).</param>
/// <param name="Location">The location summary (optional).</param>
/// <param name="IsPublic">Whether the trip is publicly visible.</param>
/// <param name="DailyHours">The number of hours planned per day.</param>
/// <param name="MaxExtensionHours">Maximum additional hours that can be added per day.</param>
/// <param name="StartTime">The daily start time for the trip.</param>
/// <param name="AttractionCount">The number of attractions in the trip.</param>
/// <param name="TotalDays">The total number of days the trip spans.</param>
/// <param name="IsOwner">Whether the current user is the owner of the trip.</param>
/// <param name="CreatedAt">The timestamp when the trip was created.</param>
/// <param name="UpdatedAt">The timestamp when the trip was last updated.</param>
public record TripListItemDto(
    Guid Id,
    Guid OwnerId,
    string Name,
    Guid? LocationId,
    LocationSummaryDto? Location,
    bool IsPublic,
    int DailyHours,
    int MaxExtensionHours,
    TimeOnly StartTime,
    int AttractionCount,
    int TotalDays,
    bool IsOwner,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
