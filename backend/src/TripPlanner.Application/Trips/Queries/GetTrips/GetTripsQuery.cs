using MediatR;
using TripPlanner.Application.Common.Models;
using TripPlanner.Application.Trips.DTOs;

namespace TripPlanner.Application.Trips.Queries.GetTrips;

/// <summary>
/// Query to retrieve a paginated list of trips with optional filtering.
/// </summary>
/// <param name="LocationId">Optional filter by location ID.</param>
/// <param name="OnlyMine">If true, only return trips owned by the current user.</param>
/// <param name="OnlyPublic">If true, only return public trips.</param>
/// <param name="Search">Optional search term for trip name.</param>
/// <param name="Page">Page number (1-based).</param>
/// <param name="PageSize">Number of items per page (max 100).</param>
public record GetTripsQuery(
    Guid? LocationId = null,
    bool OnlyMine = false,
    bool OnlyPublic = false,
    string? Search = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<PaginatedList<TripListItemDto>>;
