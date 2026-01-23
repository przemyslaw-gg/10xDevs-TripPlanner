using MediatR;
using TripPlanner.Application.Common.Models;
using TripPlanner.Application.Locations.DTOs;

namespace TripPlanner.Application.Locations.Queries.GetLocations;

/// <summary>
/// Query to retrieve a paginated list of locations with optional search filtering.
/// </summary>
/// <param name="Search">Optional search term to filter locations by name or country.</param>
/// <param name="Page">The page number (1-based). Default is 1.</param>
/// <param name="PageSize">The number of items per page. Default is 20, max is 100.</param>
public record GetLocationsQuery(
    string? Search = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<PaginatedList<LocationListItemDto>>;
