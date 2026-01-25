using MediatR;
using TripPlanner.Application.RouteOptimization.DTOs;

namespace TripPlanner.Application.RouteOptimization.Commands.OptimizeRoute;

/// <summary>
/// Command to optimize the visiting order of attractions in a trip using the Nearest Neighbor algorithm.
/// </summary>
/// <param name="TripId">The ID of the trip to optimize.</param>
/// <param name="StartingAttractionId">The ID of the attraction to start the optimized route from.</param>
public record OptimizeRouteCommand(
    Guid TripId,
    Guid StartingAttractionId
) : IRequest<OptimizeRouteResponseDto>;
