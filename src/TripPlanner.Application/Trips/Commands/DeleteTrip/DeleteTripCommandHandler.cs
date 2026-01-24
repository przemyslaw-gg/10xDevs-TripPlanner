using MediatR;
using Microsoft.EntityFrameworkCore;
using TripPlanner.Application.Common.Exceptions;
using TripPlanner.Application.Common.Interfaces;

namespace TripPlanner.Application.Trips.Commands.DeleteTrip;

/// <summary>
/// Handler for <see cref="DeleteTripCommand"/>.
/// Deletes a trip. Only the owner can delete the trip.
/// </summary>
public class DeleteTripCommandHandler : IRequestHandler<DeleteTripCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public DeleteTripCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(
        DeleteTripCommand request,
        CancellationToken cancellationToken)
    {
        // Verify user is authenticated
        if (!_currentUserService.IsAuthenticated || !_currentUserService.UserId.HasValue)
        {
            throw new UnauthorizedAccessException("Authentication is required to delete a trip");
        }

        var currentUserId = _currentUserService.UserId.Value;

        // Find the trip
        var trip = await _context.Trips
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (trip == null)
        {
            throw new NotFoundException("Trip", request.Id);
        }

        // Verify ownership
        if (trip.OwnerId != currentUserId)
        {
            throw new ForbiddenAccessException("Cannot delete trip owned by another user");
        }

        // Delete the trip (cascade will handle TripAttractions)
        _context.Trips.Remove(trip);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
