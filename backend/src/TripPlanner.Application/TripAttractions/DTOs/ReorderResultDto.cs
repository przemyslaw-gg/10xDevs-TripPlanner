namespace TripPlanner.Application.TripAttractions.DTOs;

/// <summary>
/// Result of a reorder operation on trip attractions.
/// </summary>
/// <param name="TripId">The ID of the trip that was reordered.</param>
/// <param name="UpdatedCount">Number of attractions that were updated.</param>
/// <param name="UpdatedAt">When the reorder operation completed.</param>
public record ReorderResultDto(
    Guid TripId,
    int UpdatedCount,
    DateTime UpdatedAt
);
