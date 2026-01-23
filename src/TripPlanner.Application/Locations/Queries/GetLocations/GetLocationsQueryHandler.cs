using MediatR;
using Microsoft.EntityFrameworkCore;
using TripPlanner.Application.Common.Interfaces;
using TripPlanner.Application.Common.Models;
using TripPlanner.Application.Locations.DTOs;

namespace TripPlanner.Application.Locations.Queries.GetLocations;

/// <summary>
/// Handler for <see cref="GetLocationsQuery"/>.
/// Retrieves a paginated list of locations with optional search filtering.
/// </summary>
public class GetLocationsQueryHandler
    : IRequestHandler<GetLocationsQuery, PaginatedList<LocationListItemDto>>
{
    private readonly IApplicationDbContext _context;

    public GetLocationsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<LocationListItemDto>> Handle(
        GetLocationsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Locations.AsNoTracking();

        // Apply search filter if provided
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchTerm = request.Search.Trim().ToLower();
            query = query.Where(l =>
                l.Name.ToLower().Contains(searchTerm) ||
                l.Country.ToLower().Contains(searchTerm));
        }

        // Order by name for consistent results
        query = query.OrderBy(l => l.Name);

        // Project to DTO before pagination to avoid selecting unnecessary columns
        var dtoQuery = query.Select(l => new LocationListItemDto(
            l.Id,
            l.Name,
            l.Country,
            l.Timezone
        ));

        return await PaginatedList<LocationListItemDto>.CreateAsync(
            dtoQuery,
            request.Page,
            request.PageSize,
            cancellationToken);
    }
}
