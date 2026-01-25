using MediatR;
using Microsoft.EntityFrameworkCore;
using TripPlanner.Application.Common.Exceptions;
using TripPlanner.Application.Common.Interfaces;

namespace TripPlanner.Application.TripAttractions.Commands.RemoveTripAttraction;

/// <summary>
/// Handler for <see cref="RemoveTripAttractionCommand"/>.
/// Removes an attraction from a trip. Only the trip owner can remove attractions.
/// </summary>
public class RemoveTripAttractionCommandHandler : IRequestHandler<RemoveTripAttractionCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public RemoveTripAttractionCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(
        RemoveTripAttractionCommand request,
        CancellationToken cancellationToken)
    {
        // Verify user is authenticated
        if (!_currentUserService.IsAuthenticated || !_currentUserService.UserId.HasValue)
        {
            throw new UnauthorizedAccessException("Authentication is required to remove attractions from a trip");
        }

        var currentUserId = _currentUserService.UserId.Value;

        // Verify trip exists and user is owner
        var trip = await _context.Trips
            .FirstOrDefaultAsync(t => t.Id == request.TripId, cancellationToken);

        if (trip == null)
        {
            throw new NotFoundException("Trip", request.TripId);
        }

        if (trip.OwnerId != currentUserId)
        {
            throw new ForbiddenAccessException("Cannot remove attractions from trip owned by another user");
        }

        // Find the trip attraction
        var tripAttraction = await _context.TripAttractions
            .FirstOrDefaultAsync(ta => ta.TripId == request.TripId && ta.AttractionId == request.AttractionId, cancellationToken);

        if (tripAttraction == null)
        {
            throw new NotFoundException("TripAttraction", $"Trip: {request.TripId}, Attraction: {request.AttractionId}");
        }

        // Remove the assignment
        _context.TripAttractions.Remove(tripAttraction);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
