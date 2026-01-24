namespace TripPlanner.Application.Trips.DTOs;

/// <summary>
/// Full trip details DTO for single trip responses (GET /{id}, POST, PUT).
/// </summary>
/// <param name="Id">The unique identifier of the trip.</param>
/// <param name="OwnerId">The unique identifier of the trip owner.</param>
/// <param name="Name">The name of the trip.</param>
/// <param name="LocationId">The unique identifier of the location (optional).</param>
/// <param name="Location">The location details (optional).</param>
/// <param name="IsPublic">Whether the trip is publicly visible.</param>
/// <param name="DailyHours">The number of hours planned per day.</param>
/// <param name="MaxExtensionHours">Maximum additional hours that can be added per day.</param>
/// <param name="StartTime">The daily start time for the trip.</param>
/// <param name="IsOwner">Whether the current user is the owner of the trip.</param>
/// <param name="CreatedAt">The timestamp when the trip was created.</param>
/// <param name="UpdatedAt">The timestamp when the trip was last updated.</param>
public record TripDto(
    Guid Id,
    Guid OwnerId,
    string Name,
    Guid? LocationId,
    LocationDetailDto? Location,
    bool IsPublic,
    int DailyHours,
    int MaxExtensionHours,
    TimeOnly StartTime,
    bool IsOwner,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
