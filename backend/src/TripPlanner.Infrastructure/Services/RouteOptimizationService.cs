using TripPlanner.Application.Common.Interfaces;

namespace TripPlanner.Infrastructure.Services;

/// <summary>
/// Service for optimizing travel routes using the Nearest Neighbor algorithm.
/// Calculates distances using the Haversine formula for geographical coordinates.
/// </summary>
public class RouteOptimizationService : IRouteOptimizationService
{
    /// <summary>
    /// Earth's radius in meters, used for Haversine distance calculations.
    /// </summary>
    private const double EarthRadiusMeters = 6_371_000;

    /// <inheritdoc />
    public RouteOptimizationResult OptimizeRoute(
        IReadOnlyList<AttractionWithCoordinates> attractions,
        Guid startingAttractionId)
    {
        if (attractions.Count == 0)
        {
            return new RouteOptimizationResult([], 0);
        }

        if (attractions.Count == 1)
        {
            var single = attractions[0];
            return new RouteOptimizationResult(
                [new OptimizedAttraction(single.Id, single.Name, 1)],
                0
            );
        }

        // Find starting attraction
        var startingAttraction = attractions.FirstOrDefault(a => a.Id == startingAttractionId);
        if (startingAttraction is null)
        {
            throw new InvalidOperationException(
                $"Starting attraction with ID '{startingAttractionId}' was not found in the provided list.");
        }

        var visited = new HashSet<Guid>();
        var result = new List<OptimizedAttraction>();
        var totalDistance = 0m;

        // Start with the specified attraction
        var current = startingAttraction;
        visited.Add(current.Id);
        result.Add(new OptimizedAttraction(current.Id, current.Name, 1));

        // Nearest Neighbor algorithm: iteratively find the closest unvisited attraction
        while (visited.Count < attractions.Count)
        {
            AttractionWithCoordinates? nearest = null;
            var minDistance = decimal.MaxValue;

            foreach (var attraction in attractions)
            {
                if (visited.Contains(attraction.Id))
                {
                    continue;
                }

                var distance = CalculateHaversineDistance(
                    current.Latitude, current.Longitude,
                    attraction.Latitude, attraction.Longitude
                );

                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = attraction;
                }
            }

            if (nearest is not null)
            {
                visited.Add(nearest.Id);
                totalDistance += minDistance;
                result.Add(new OptimizedAttraction(
                    nearest.Id,
                    nearest.Name,
                    result.Count + 1
                ));
                current = nearest;
            }
        }

        return new RouteOptimizationResult(result, totalDistance);
    }

    /// <inheritdoc />
    public decimal CalculateHaversineDistance(
        decimal lat1, decimal lon1,
        decimal lat2, decimal lon2)
    {
        // Convert decimal degrees to radians
        var dLat = DegreesToRadians((double)(lat2 - lat1));
        var dLon = DegreesToRadians((double)(lon2 - lon1));

        var lat1Rad = DegreesToRadians((double)lat1);
        var lat2Rad = DegreesToRadians((double)lat2);

        // Haversine formula
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        // Distance in meters
        return (decimal)(EarthRadiusMeters * c);
    }

    /// <summary>
    /// Converts degrees to radians.
    /// </summary>
    /// <param name="degrees">The angle in degrees.</param>
    /// <returns>The angle in radians.</returns>
    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }
}
