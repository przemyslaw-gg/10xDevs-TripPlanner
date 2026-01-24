using MediatR;
using Microsoft.EntityFrameworkCore;
using TripPlanner.Application.Common.Exceptions;
using TripPlanner.Application.Common.Interfaces;
using TripPlanner.Application.Trips.DTOs;
using TripPlanner.Domain.Entities;

namespace TripPlanner.Application.Trips.Commands.CreateTrip;

/// <summary>
/// Handler for <see cref="CreateTripCommand"/>.
/// Creates a new trip owned by the current user.
/// </summary>
public class CreateTripCommandHandler : IRequestHandler<CreateTripCommand, TripDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CreateTripCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<TripDto> Handle(
        CreateTripCommand request,
        CancellationToken cancellationToken)
    {
        // Verify user is authenticated
        if (!_currentUserService.IsAuthenticated || !_currentUserService.UserId.HasValue)
        {
            throw new UnauthorizedAccessException("Authentication is required to create a trip");
        }

        var currentUserId = _currentUserService.UserId.Value;

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

        // Create trip entity
        var trip = new Trip
        {
            Id = Guid.NewGuid(),
            OwnerId = currentUserId,
            Name = request.Name,
            LocationId = request.LocationId,
            IsPublic = false, // Default to private
            DailyHours = request.DailyHours,
            MaxExtensionHours = request.MaxExtensionHours,
            StartTime = startTime
        };

        _context.Trips.Add(trip);
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
            true, // IsOwner - creator is always the owner
            trip.CreatedAt,
            trip.UpdatedAt
        );
    }
}
