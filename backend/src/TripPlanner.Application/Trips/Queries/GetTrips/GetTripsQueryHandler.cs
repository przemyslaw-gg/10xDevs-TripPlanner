using MediatR;
using Microsoft.EntityFrameworkCore;
using TripPlanner.Application.Attractions.DTOs;
using TripPlanner.Application.Common.Interfaces;
using TripPlanner.Application.Common.Models;
using TripPlanner.Application.Trips.DTOs;

namespace TripPlanner.Application.Trips.Queries.GetTrips;

/// <summary>
/// Handler for <see cref="GetTripsQuery"/>.
/// Returns a paginated list of trips filtered by various criteria.
/// Users can see public trips and their own private trips.
/// </summary>
public class GetTripsQueryHandler : IRequestHandler<GetTripsQuery, PaginatedList<TripListItemDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetTripsQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<PaginatedList<TripListItemDto>> Handle(
        GetTripsQuery request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        var query = _context.Trips
            .AsNoTracking()
            .Include(t => t.Location)
            .Include(t => t.TripAttractions)
            .AsQueryable();

        // Base access filter: public trips OR user's own trips
        query = query.Where(t => t.IsPublic || t.OwnerId == currentUserId);

        // Apply OnlyMine filter
        if (request.OnlyMine && currentUserId.HasValue)
        {
            query = query.Where(t => t.OwnerId == currentUserId.Value);
        }

        // Apply OnlyPublic filter
        if (request.OnlyPublic)
        {
            query = query.Where(t => t.IsPublic);
        }

        // Apply LocationId filter
        if (request.LocationId.HasValue)
        {
            query = query.Where(t => t.LocationId == request.LocationId.Value);
        }

        // Apply Search filter
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchTerm = request.Search.Trim().ToLower();
            query = query.Where(t => t.Name.ToLower().Contains(searchTerm));
        }

        // Order by creation date descending (newest first)
        query = query.OrderByDescending(t => t.CreatedAt);

        // Project to DTO
        var dtoQuery = query.Select(t => new TripListItemDto(
            t.Id,
            t.OwnerId,
            t.Name,
            t.LocationId,
            t.Location != null
                ? new LocationSummaryDto(t.Location.Id, t.Location.Name, t.Location.Country)
                : null,
            t.IsPublic,
            t.DailyHours,
            t.MaxExtensionHours,
            t.StartTime,
            t.TripAttractions.Count,
            CalculateTotalDays(
                t.TripAttractions.Sum(ta => ta.Attraction.EstimatedDuration ?? 0),
                t.DailyHours),
            t.OwnerId == currentUserId,
            t.CreatedAt,
            t.UpdatedAt
        ));

        return await PaginatedList<TripListItemDto>.CreateAsync(
            dtoQuery, request.Page, request.PageSize, cancellationToken);
    }

    /// <summary>
    /// Calculates the total number of days needed for the trip based on total minutes and daily hours.
    /// </summary>
    private static int CalculateTotalDays(int totalMinutes, int dailyHours)
    {
        if (totalMinutes <= 0) return 0;
        var dailyMinutes = dailyHours * 60;
        return (int)Math.Ceiling((double)totalMinutes / dailyMinutes);
    }
}
