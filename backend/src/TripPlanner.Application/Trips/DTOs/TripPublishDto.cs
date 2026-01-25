namespace TripPlanner.Application.Trips.DTOs;

/// <summary>
/// Response DTO for PATCH /api/trips/{id}/publish endpoint.
/// Returns minimal trip information after visibility change.
/// </summary>
/// <param name="Id">The unique identifier of the trip.</param>
/// <param name="IsPublic">The new public visibility status.</param>
/// <param name="UpdatedAt">The timestamp when the trip was updated.</param>
public record TripPublishDto(
    Guid Id,
    bool IsPublic,
    DateTime UpdatedAt
);
