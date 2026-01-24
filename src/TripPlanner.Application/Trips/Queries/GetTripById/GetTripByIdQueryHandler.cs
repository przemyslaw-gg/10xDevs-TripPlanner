using MediatR;
using Microsoft.EntityFrameworkCore;
using TripPlanner.Application.Common.Exceptions;
using TripPlanner.Application.Common.Interfaces;
using TripPlanner.Application.Trips.DTOs;

namespace TripPlanner.Application.Trips.Queries.GetTripById;

/// <summary>
/// Handler for <see cref="GetTripByIdQuery"/>.
/// Returns trip details if the user has access (public trip or owner).
/// </summary>
public class GetTripByIdQueryHandler : IRequestHandler<GetTripByIdQuery, TripDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetTripByIdQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<TripDto> Handle(
        GetTripByIdQuery request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        var trip = await _context.Trips
            .AsNoTracking()
            .Include(t => t.Location)
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (trip == null)
        {
            throw new NotFoundException("Trip", request.Id);
        }

        // Check access: must be public or owned by current user
        if (!trip.IsPublic && trip.OwnerId != currentUserId)
        {
            throw new ForbiddenAccessException("Trip is private and not owned by you");
        }

        return new TripDto(
            trip.Id,
            trip.OwnerId,
            trip.Name,
            trip.LocationId,
            trip.Location != null
                ? new LocationDetailDto(
                    trip.Location.Id,
                    trip.Location.Name,
                    trip.Location.Country,
                    trip.Location.Timezone)
                : null,
            trip.IsPublic,
            trip.DailyHours,
            trip.MaxExtensionHours,
            trip.StartTime,
            trip.OwnerId == currentUserId,
            trip.CreatedAt,
            trip.UpdatedAt
        );
    }
}
