using MediatR;
using Microsoft.EntityFrameworkCore;
using TripPlanner.Application.Common.Exceptions;
using TripPlanner.Application.Common.Interfaces;
using TripPlanner.Application.Trips.DTOs;
using TripPlanner.Domain.Entities;

namespace TripPlanner.Application.Trips.Commands.UpdateTrip;

/// <summary>
/// Handler for <see cref="UpdateTripCommand"/>.
/// Updates an existing trip. Only the owner can update the trip.
/// </summary>
public class UpdateTripCommandHandler : IRequestHandler<UpdateTripCommand, TripDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdateTripCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<TripDto> Handle(
        UpdateTripCommand request,
        CancellationToken cancellationToken)
    {
        // Verify user is authenticated
        if (!_currentUserService.IsAuthenticated || !_currentUserService.UserId.HasValue)
        {
            throw new UnauthorizedAccessException("Authentication is required to update a trip");
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
            throw new ForbiddenAccessException("Cannot modify trip owned by another user");
        }

        // Verify location exists if provided
        Location? location = null;
        if (request.LocationId.HasValue)
        {
            location = await _context.Locations
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Id == request.LocationId.Value, cancellationToken);

            if (location == null)
            {
                throw new NotFoundException("Location", request.LocationId.Value);
            }
        }

        // Parse start time
        var startTime = TimeOnly.Parse(request.StartTime);

        // Update trip
        trip.Name = request.Name;
        trip.LocationId = request.LocationId;
        trip.DailyHours = request.DailyHours;
        trip.MaxExtensionHours = request.MaxExtensionHours;
        trip.StartTime = startTime;

        await _context.SaveChangesAsync(cancellationToken);

        return new TripDto(
            trip.Id,
            trip.OwnerId,
            trip.Name,
            trip.LocationId,
            location != null
                ? new LocationDetailDto(location.Id, location.Name, location.Country, location.Timezone)
                : null,
            trip.IsPublic,
            trip.DailyHours,
            trip.MaxExtensionHours,
            trip.StartTime,
            true, // IsOwner - only owner can update
            trip.CreatedAt,
            trip.UpdatedAt
        );
    }
}
