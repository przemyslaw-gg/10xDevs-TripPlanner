using MediatR;
using TripPlanner.Application.Locations.DTOs;

namespace TripPlanner.Application.Locations.Queries.GetLocationById;

/// <summary>
/// Query to retrieve a single location by its unique identifier.
/// </summary>
/// <param name="Id">The unique identifier of the location to retrieve.</param>
public record GetLocationByIdQuery(Guid Id) : IRequest<LocationDto?>;
