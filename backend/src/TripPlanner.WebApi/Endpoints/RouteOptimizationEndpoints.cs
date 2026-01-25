using MediatR;
using TripPlanner.Application.RouteOptimization.Commands.OptimizeRoute;
using TripPlanner.Application.RouteOptimization.DTOs;

namespace TripPlanner.WebApi.Endpoints;

/// <summary>
/// Minimal API endpoints for Route Optimization operations.
/// Provides route optimization using the Nearest Neighbor algorithm.
/// All endpoints require authentication.
/// </summary>
public static class RouteOptimizationEndpoints
{
    /// <summary>
    /// Maps all route optimization endpoints to the application.
    /// </summary>
    public static WebApplication MapRouteOptimizationEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/trips/{tripId:guid}")
            .WithTags("Route Optimization")
            .RequireAuthorization();

        // POST /api/trips/{tripId}/optimize-route - Calculate and apply optimal visiting order
        group.MapPost("/optimize-route", OptimizeRoute)
            .WithName("OptimizeRoute")
            .WithDescription("Calculate and apply optimal visiting order using nearest neighbor algorithm")
            .Produces<OptimizeRouteResponseDto>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        return app;
    }

    /// <summary>
    /// Optimizes the visiting order of attractions in a trip using the Nearest Neighbor algorithm.
    /// </summary>
    /// <param name="tripId">The ID of the trip to optimize.</param>
    /// <param name="request">The optimization request containing the starting attraction ID.</param>
    /// <param name="mediator">The MediatR sender.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The optimization result with the new order and total distance.</returns>
    private static async Task<IResult> OptimizeRoute(
        Guid tripId,
        OptimizeRouteRequest request,
        ISender mediator,
        CancellationToken cancellationToken = default)
    {
        var command = new OptimizeRouteCommand(tripId, request.StartingAttractionId);
        var result = await mediator.Send(command, cancellationToken);

        return Results.Ok(result);
    }
}

/// <summary>
/// Request body for route optimization.
/// </summary>
/// <param name="StartingAttractionId">The ID of the attraction to start the optimized route from.</param>
public record OptimizeRouteRequest(
    Guid StartingAttractionId
);
