using MediatR;
using TripPlanner.Application.TripAttractions.DTOs;

namespace TripPlanner.Application.TripAttractions.Queries.GetTripAttractions;

/// <summary>
/// Query to retrieve the complete schedule of attractions for a trip.
/// Returns attractions grouped by day number.
/// </summary>
/// <param name="TripId">The unique identifier of the trip.</param>
public record GetTripAttractionsQuery(
    Guid TripId
) : IRequest<TripScheduleDto>;
