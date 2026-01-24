using MediatR;
using Microsoft.EntityFrameworkCore;
using TripPlanner.Application.Common.Exceptions;
using TripPlanner.Application.Common.Interfaces;
using TripPlanner.Application.TripAttractions.DTOs;

namespace TripPlanner.Application.TripAttractions.Commands.UpdateTripAttraction;

/// <summary>
/// Handler for <see cref="UpdateTripAttractionCommand"/>.
/// Updates a trip attraction assignment. Only the trip owner can update.
/// </summary>
public class UpdateTripAttractionCommandHandler : IRequestHandler<UpdateTripAttractionCommand, TripAttractionDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdateTripAttractionCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<TripAttractionDto> Handle(
        UpdateTripAttractionCommand request,
        CancellationToken cancellationToken)
    {
        // Verify user is authenticated
        if (!_currentUserService.IsAuthenticated || !_currentUserService.UserId.HasValue)
        {
            throw new UnauthorizedAccessException("Authentication is required to update trip attractions");
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
            throw new ForbiddenAccessException("Cannot update attractions in trip owned by another user");
        }

        // Find the trip attraction
        var tripAttraction = await _context.TripAttractions
            .FirstOrDefaultAsync(ta => ta.TripId == request.TripId && ta.AttractionId == request.AttractionId, cancellationToken);

        if (tripAttraction == null)
        {
            throw new NotFoundException("TripAttraction", $"Trip: {request.TripId}, Attraction: {request.AttractionId}");
        }

        // Update the assignment
        tripAttraction.DayNumber = request.DayNumber;
        tripAttraction.OrderIndex = request.OrderIndex;
        tripAttraction.PlannedStartTime = string.IsNullOrEmpty(request.PlannedStartTime)
            ? null
            : TimeOnly.Parse(request.PlannedStartTime);

        await _context.SaveChangesAsync(cancellationToken);

        return new TripAttractionDto(
            tripAttraction.Id,
            tripAttraction.TripId,
            tripAttraction.AttractionId,
            tripAttraction.DayNumber,
            tripAttraction.OrderIndex,
            tripAttraction.PlannedStartTime,
            tripAttraction.CreatedAt,
            tripAttraction.UpdatedAt);
    }
}
