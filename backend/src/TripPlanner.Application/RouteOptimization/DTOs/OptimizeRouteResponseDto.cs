namespace TripPlanner.Application.RouteOptimization.DTOs;

/// <summary>
/// Response DTO for the route optimization endpoint.
/// Contains the optimized order of attractions and total distance.
/// </summary>
/// <param name="TripId">The unique identifier of the trip that was optimized.</param>
/// <param name="OptimizedOrder">The list of attractions in optimized visiting order.</param>
/// <param name="TotalDistance">The total distance of the optimized route in meters.</param>
/// <param name="OptimizedAt">The timestamp when the optimization was performed.</param>
public record OptimizeRouteResponseDto(
    Guid TripId,
    IReadOnlyList<OptimizedAttractionItemDto> OptimizedOrder,
    decimal TotalDistance,
    DateTime OptimizedAt
);
