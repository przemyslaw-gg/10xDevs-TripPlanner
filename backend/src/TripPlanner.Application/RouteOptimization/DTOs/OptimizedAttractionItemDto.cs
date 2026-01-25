namespace TripPlanner.Application.RouteOptimization.DTOs;

/// <summary>
/// Data transfer object representing a single attraction in the optimized route order.
/// </summary>
/// <param name="AttractionId">The unique identifier of the attraction.</param>
/// <param name="AttractionName">The name of the attraction.</param>
/// <param name="DayNumber">The day number within the trip (1-based).</param>
/// <param name="OrderIndex">The optimized order index within the route (1-based).</param>
public record OptimizedAttractionItemDto(
    Guid AttractionId,
    string AttractionName,
    int DayNumber,
    int OrderIndex
);
