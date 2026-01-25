using MediatR;
using Microsoft.EntityFrameworkCore;
using TripPlanner.Application.Common.Exceptions;
using TripPlanner.Application.Common.Interfaces;
using TripPlanner.Application.RouteOptimization.DTOs;

namespace TripPlanner.Application.RouteOptimization.Commands.OptimizeRoute;

/// <summary>
/// Handler for <see cref="OptimizeRouteCommand"/>.
/// Optimizes the visiting order of attractions in a trip using the Nearest Neighbor algorithm.
/// Only the trip owner can optimize the route.
/// </summary>
public class OptimizeRouteCommandHandler : IRequestHandler<OptimizeRouteCommand, OptimizeRouteResponseDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IRouteOptimizationService _routeOptimizationService;

    public OptimizeRouteCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IRouteOptimizationService routeOptimizationService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _routeOptimizationService = routeOptimizationService;
    }

    public async Task<OptimizeRouteResponseDto> Handle(
        OptimizeRouteCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Verify user is authenticated
        if (!_currentUserService.IsAuthenticated || !_currentUserService.UserId.HasValue)
        {
            throw new UnauthorizedAccessException("Authentication is required to optimize trip route.");
        }

        var currentUserId = _currentUserService.UserId.Value;

        // 2. Get trip with attractions and their details
        var trip = await _context.Trips
            .Include(t => t.TripAttractions)
                .ThenInclude(ta => ta.Attraction)
            .FirstOrDefaultAsync(t => t.Id == request.TripId, cancellationToken);

        if (trip is null)
        {
            throw new NotFoundException("Trip", request.TripId);
        }

        // 3. Authorization check - only owner can optimize
        if (trip.OwnerId != currentUserId)
        {
            throw new ForbiddenAccessException("Cannot optimize route for a trip owned by another user.");
        }

        // 4. Check if trip has attractions to optimize
        if (trip.TripAttractions.Count == 0)
        {
            throw new UnprocessableEntityException("Trip has no attractions to optimize.");
        }

        // 5. Check if starting attraction is in the trip
        var startingAttractionExists = trip.TripAttractions
            .Any(ta => ta.AttractionId == request.StartingAttractionId);

        if (!startingAttractionExists)
        {
            throw new InvalidOperationException(
                $"Starting attraction with ID '{request.StartingAttractionId}' is not assigned to this trip.");
        }

        // 6. Prepare data for optimization
        var attractionsWithCoordinates = trip.TripAttractions
            .Select(ta => new AttractionWithCoordinates(
                ta.AttractionId,
                ta.Attraction.Name,
                ta.Attraction.Latitude,
                ta.Attraction.Longitude,
                ta.DayNumber
            ))
            .ToList();

        // 7. Run optimization algorithm
        var optimizationResult = _routeOptimizationService.OptimizeRoute(
            attractionsWithCoordinates,
            request.StartingAttractionId
        );

        // 8. Update order_index in trip_attractions
        var tripAttractionLookup = trip.TripAttractions.ToDictionary(ta => ta.AttractionId);

        foreach (var optimizedAttraction in optimizationResult.OptimizedAttractions)
        {
            var tripAttraction = tripAttractionLookup[optimizedAttraction.AttractionId];
            tripAttraction.OrderIndex = optimizedAttraction.OrderIndex;
            tripAttraction.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);

        // 9. Build response with current day numbers
        var optimizedOrder = optimizationResult.OptimizedAttractions
            .Select(oa =>
            {
                var tripAttraction = tripAttractionLookup[oa.AttractionId];
                return new OptimizedAttractionItemDto(
                    oa.AttractionId,
                    oa.AttractionName,
                    tripAttraction.DayNumber,
                    oa.OrderIndex
                );
            })
            .OrderBy(x => x.OrderIndex)
            .ToList();

        return new OptimizeRouteResponseDto(
            request.TripId,
            optimizedOrder,
            optimizationResult.TotalDistanceMeters,
            DateTime.UtcNow
        );
    }
}
