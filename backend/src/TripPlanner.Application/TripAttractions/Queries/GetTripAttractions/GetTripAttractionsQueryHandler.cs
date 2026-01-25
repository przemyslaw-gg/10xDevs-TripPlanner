using MediatR;
using Microsoft.EntityFrameworkCore;
using TripPlanner.Application.Common.Exceptions;
using TripPlanner.Application.Common.Interfaces;
using TripPlanner.Application.TripAttractions.DTOs;

namespace TripPlanner.Application.TripAttractions.Queries.GetTripAttractions;

/// <summary>
/// Handler for <see cref="GetTripAttractionsQuery"/>.
/// Returns the trip schedule with attractions grouped by day.
/// Access is allowed for public trips or if the user is the owner.
/// </summary>
public class GetTripAttractionsQueryHandler : IRequestHandler<GetTripAttractionsQuery, TripScheduleDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetTripAttractionsQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<TripScheduleDto> Handle(
        GetTripAttractionsQuery request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        // Verify trip exists and user has access
        var trip = await _context.Trips
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == request.TripId, cancellationToken);

        if (trip == null)
        {
            throw new NotFoundException("Trip", request.TripId);
        }

        // Check access: must be public or owned by current user
        if (!trip.IsPublic && trip.OwnerId != currentUserId)
        {
            throw new ForbiddenAccessException("Trip is private and not owned by you");
        }

        // Fetch all trip attractions with their attraction data
        var tripAttractions = await _context.TripAttractions
            .AsNoTracking()
            .Include(ta => ta.Attraction)
            .Where(ta => ta.TripId == request.TripId)
            .OrderBy(ta => ta.DayNumber)
            .ThenBy(ta => ta.OrderIndex)
            .ToListAsync(cancellationToken);

        // Calculate totals
        var totalDays = tripAttractions.Any()
            ? tripAttractions.Max(ta => ta.DayNumber)
            : 0;

        var totalDuration = tripAttractions
            .Sum(ta => ta.Attraction.EstimatedDuration ?? 0);

        // Group attractions by day
        var days = tripAttractions
            .GroupBy(ta => ta.DayNumber)
            .Select(g => new TripDayDto(
                g.Key,
                g.Sum(ta => ta.Attraction.EstimatedDuration ?? 0),
                g.Select(ta => new TripAttractionDetailDto(
                    ta.Id,
                    ta.AttractionId,
                    new AttractionSummaryDto(
                        ta.Attraction.Id,
                        ta.Attraction.Name,
                        ta.Attraction.Latitude,
                        ta.Attraction.Longitude,
                        ta.Attraction.Rating,
                        ta.Attraction.EstimatedDuration ?? 0,
                        ta.Attraction.ImageUrl),
                    ta.DayNumber,
                    ta.OrderIndex,
                    ta.PlannedStartTime))
                .OrderBy(a => a.OrderIndex)
                .ToList()))
            .OrderBy(d => d.DayNumber)
            .ToList();

        return new TripScheduleDto(
            request.TripId,
            totalDays,
            totalDuration,
            days);
    }
}
