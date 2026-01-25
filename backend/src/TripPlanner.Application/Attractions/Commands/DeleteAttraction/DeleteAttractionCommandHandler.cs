using MediatR;
using Microsoft.EntityFrameworkCore;
using TripPlanner.Application.Common.Exceptions;
using TripPlanner.Application.Common.Interfaces;

namespace TripPlanner.Application.Attractions.Commands.DeleteAttraction;

/// <summary>
/// Handler for <see cref="DeleteAttractionCommand"/>.
/// Deletes a user-generated attraction.
/// Only the owner can delete their own attractions.
/// Cannot delete if the attraction is used in other users' trips.
/// </summary>
public class DeleteAttractionCommandHandler
    : IRequestHandler<DeleteAttractionCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public DeleteAttractionCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(
        DeleteAttractionCommand request,
        CancellationToken cancellationToken)
    {
        // Verify the user is authenticated
        if (!_currentUserService.IsAuthenticated || !_currentUserService.UserId.HasValue)
        {
            throw new UnauthorizedAccessException("Authentication is required to delete an attraction.");
        }

        var currentUserId = _currentUserService.UserId.Value;

        // Get the attraction
        var attraction = await _context.Attractions
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

        if (attraction == null)
        {
            throw new NotFoundException("Attraction", request.Id);
        }

        // Verify the attraction is user-generated (not a system/verified attraction)
        if (attraction.IsVerified)
        {
            throw new ForbiddenAccessException("Cannot delete verified attractions.");
        }

        // Verify the user is the owner
        if (attraction.CreatedByUserId != currentUserId)
        {
            throw new ForbiddenAccessException("Cannot delete attraction created by another user.");
        }

        // Check if the attraction is used in other users' trips
        var isUsedByOthers = await _context.TripAttractions
            .Include(ta => ta.Trip)
            .AnyAsync(ta =>
                ta.AttractionId == request.Id &&
                ta.Trip.OwnerId != currentUserId,
                cancellationToken);

        if (isUsedByOthers)
        {
            throw new ConflictException("Attraction is used in other users' trips and cannot be deleted.");
        }

        // Remove any trip attractions for the current user's trips first
        var userTripAttractions = await _context.TripAttractions
            .Include(ta => ta.Trip)
            .Where(ta => ta.AttractionId == request.Id && ta.Trip.OwnerId == currentUserId)
            .ToListAsync(cancellationToken);

        if (userTripAttractions.Any())
        {
            _context.TripAttractions.RemoveRange(userTripAttractions);
        }

        // Delete the attraction
        _context.Attractions.Remove(attraction);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
