namespace TripPlanner.Application.Common.Interfaces;

/// <summary>
/// Service interface for route optimization using geographical coordinates.
/// </summary>
public interface IRouteOptimizationService
{
    /// <summary>
    /// Optimizes the visiting order of attractions using the Nearest Neighbor algorithm.
    /// </summary>
    /// <param name="attractions">List of attractions with their coordinates.</param>
    /// <param name="startingAttractionId">The ID of the attraction to start from.</param>
    /// <returns>The optimization result containing ordered attractions and total distance.</returns>
    RouteOptimizationResult OptimizeRoute(
        IReadOnlyList<AttractionWithCoordinates> attractions,
        Guid startingAttractionId
    );

    /// <summary>
    /// Calculates the distance between two geographical points using the Haversine formula.
    /// </summary>
    /// <param name="lat1">Latitude of the first point.</param>
    /// <param name="lon1">Longitude of the first point.</param>
    /// <param name="lat2">Latitude of the second point.</param>
    /// <param name="lon2">Longitude of the second point.</param>
    /// <returns>The distance in meters.</returns>
    decimal CalculateHaversineDistance(
        decimal lat1, decimal lon1,
        decimal lat2, decimal lon2
    );
}

/// <summary>
/// Represents an attraction with its geographical coordinates for route optimization.
/// </summary>
/// <param name="Id">The unique identifier of the attraction.</param>
/// <param name="Name">The name of the attraction.</param>
/// <param name="Latitude">The latitude coordinate.</param>
/// <param name="Longitude">The longitude coordinate.</param>
/// <param name="CurrentDayNumber">The current day number assignment in the trip.</param>
public record AttractionWithCoordinates(
    Guid Id,
    string Name,
    decimal Latitude,
    decimal Longitude,
    int CurrentDayNumber
);

/// <summary>
/// Result of the route optimization algorithm.
/// </summary>
/// <param name="OptimizedAttractions">The list of attractions in optimized order.</param>
/// <param name="TotalDistanceMeters">The total distance of the optimized route in meters.</param>
public record RouteOptimizationResult(
    IReadOnlyList<OptimizedAttraction> OptimizedAttractions,
    decimal TotalDistanceMeters
);

/// <summary>
/// Represents an attraction in the optimized route order.
/// </summary>
/// <param name="AttractionId">The unique identifier of the attraction.</param>
/// <param name="AttractionName">The name of the attraction.</param>
/// <param name="OrderIndex">The position in the optimized route (1-based).</param>
public record OptimizedAttraction(
    Guid AttractionId,
    string AttractionName,
    int OrderIndex
);
