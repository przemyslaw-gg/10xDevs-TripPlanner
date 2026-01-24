using MediatR;
using TripPlanner.Application.Trips.DTOs;

namespace TripPlanner.Application.Trips.Queries.GetTripById;

/// <summary>
/// Query to retrieve a single trip by its ID.
/// </summary>
/// <param name="Id">The unique identifier of the trip.</param>
public record GetTripByIdQuery(Guid Id) : IRequest<TripDto>;
