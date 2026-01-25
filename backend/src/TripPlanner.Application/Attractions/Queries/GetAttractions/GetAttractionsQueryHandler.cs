using MediatR;
using Microsoft.EntityFrameworkCore;
using TripPlanner.Application.Attractions.DTOs;
using TripPlanner.Application.Common.Interfaces;
using TripPlanner.Application.Common.Models;
using TripPlanner.Domain.Entities;

namespace TripPlanner.Application.Attractions.Queries.GetAttractions;

/// <summary>
/// Handler for <see cref="GetAttractionsQuery"/>.
/// Retrieves a paginated list of attractions with filtering and sorting.
/// </summary>
public class GetAttractionsQueryHandler
    : IRequestHandler<GetAttractionsQuery, PaginatedList<AttractionListItemDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAttractionsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<AttractionListItemDto>> Handle(
        GetAttractionsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Attractions.AsNoTracking();

        // Apply filters
        query = ApplyFilters(query, request);

        // Apply sorting
        query = ApplySorting(query, request.SortBy, request.SortOrder);

        // Project to DTO before pagination
        var dtoQuery = query.Select(a => new AttractionListItemDto(
            a.Id,
            a.LocationId,
            a.Name,
            a.Description,
            a.Latitude,
            a.Longitude,
            a.Rating,
            a.ReviewCount,
            a.EstimatedDuration ?? 60,
            a.ImageUrl,
            a.IsVerified,
            a.CreatedByUserId
        ));

        return await PaginatedList<AttractionListItemDto>.CreateAsync(
            dtoQuery,
            request.Page,
            request.PageSize,
            cancellationToken);
    }

    private static IQueryable<Attraction> ApplyFilters(
        IQueryable<Attraction> query,
        GetAttractionsQuery request)
    {
        // Filter by location
        if (request.LocationId.HasValue)
        {
            query = query.Where(a => a.LocationId == request.LocationId.Value);
        }

        // Filter by search term (name)
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchTerm = request.Search.Trim().ToLower();
            query = query.Where(a => a.Name.ToLower().Contains(searchTerm));
        }

        // Filter by verified status
        // Note: IsVerified in API = !IsUserGenerated in DB (verified = system attractions)
        if (request.IsVerified.HasValue)
        {
            query = query.Where(a => a.IsVerified == request.IsVerified.Value);
        }

        return query;
    }

    private static IQueryable<Attraction> ApplySorting(
        IQueryable<Attraction> query,
        string sortBy,
        string sortOrder)
    {
        var isDescending = sortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase);

        return sortBy.ToLowerInvariant() switch
        {
            "name" => isDescending
                ? query.OrderByDescending(a => a.Name)
                : query.OrderBy(a => a.Name),
            "reviewcount" => isDescending
                ? query.OrderByDescending(a => a.ReviewCount ?? 0)
                : query.OrderBy(a => a.ReviewCount ?? 0),
            _ => isDescending // default: rating
                ? query.OrderByDescending(a => a.Rating ?? 0)
                : query.OrderBy(a => a.Rating ?? 0)
        };
    }
}
