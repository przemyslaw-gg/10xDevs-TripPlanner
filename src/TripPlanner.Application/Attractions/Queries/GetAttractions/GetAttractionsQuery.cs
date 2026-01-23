using MediatR;
using TripPlanner.Application.Attractions.DTOs;
using TripPlanner.Application.Common.Models;

namespace TripPlanner.Application.Attractions.Queries.GetAttractions;

/// <summary>
/// Query to retrieve a paginated list of attractions with filtering and sorting options.
/// </summary>
/// <param name="LocationId">Optional filter by location ID.</param>
/// <param name="Search">Optional search term to filter attractions by name.</param>
/// <param name="SortBy">Sort field: rating, name, or reviewCount. Default is rating.</param>
/// <param name="SortOrder">Sort order: asc or desc. Default is desc.</param>
/// <param name="IsVerified">Optional filter for verified (system) or unverified (user-created) attractions.</param>
/// <param name="Page">The page number (1-based). Default is 1.</param>
/// <param name="PageSize">The number of items per page. Default is 20, max is 100.</param>
public record GetAttractionsQuery(
    Guid? LocationId = null,
    string? Search = null,
    string SortBy = "rating",
    string SortOrder = "desc",
    bool? IsVerified = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<PaginatedList<AttractionListItemDto>>;
