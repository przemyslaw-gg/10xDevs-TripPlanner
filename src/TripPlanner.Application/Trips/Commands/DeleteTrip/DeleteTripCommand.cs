using MediatR;

namespace TripPlanner.Application.Trips.Commands.DeleteTrip;

/// <summary>
/// Command to delete a trip.
/// </summary>
/// <param name="Id">The unique identifier of the trip to delete.</param>
public record DeleteTripCommand(Guid Id) : IRequest<Unit>;
