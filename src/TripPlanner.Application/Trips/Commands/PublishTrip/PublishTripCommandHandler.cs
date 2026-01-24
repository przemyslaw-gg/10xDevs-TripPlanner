using MediatR;
using Microsoft.EntityFrameworkCore;
using TripPlanner.Application.Common.Exceptions;
using TripPlanner.Application.Common.Interfaces;
using TripPlanner.Application.Trips.DTOs;

namespace TripPlanner.Application.Trips.Commands.PublishTrip;

/// <summary>
/// Handler for <see cref="PublishTripCommand"/>.
/// Changes the public visibility of a trip. Only the owner can change visibility.
/// </summary>
public class PublishTripCommandHandler : IRequestHandler<PublishTripCommand, TripPublishDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public PublishTripCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<TripPublishDto> Handle(
        PublishTripCommand request,
        CancellationToken cancellationToken)
    {
        // Verify user is authenticated
        if (!_currentUserService.IsAuthenticated || !_currentUserService.UserId.HasValue)
        {
            throw new UnauthorizedAccessException("Authentication is required to change trip visibility");
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
            throw new ForbiddenAccessException("Cannot change visibility of trip owned by another user");
        }

        // Update visibility
        trip.IsPublic = request.IsPublic;
        await _context.SaveChangesAsync(cancellationToken);

        return new TripPublishDto(
            trip.Id,
            trip.IsPublic,
            trip.UpdatedAt
        );
    }
}
