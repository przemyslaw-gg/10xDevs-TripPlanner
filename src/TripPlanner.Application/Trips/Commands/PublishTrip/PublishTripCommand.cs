using MediatR;
using TripPlanner.Application.Trips.DTOs;

namespace TripPlanner.Application.Trips.Commands.PublishTrip;

/// <summary>
/// Command to change the public visibility of a trip.
/// </summary>
/// <param name="Id">The unique identifier of the trip.</param>
/// <param name="IsPublic">The new public visibility status.</param>
public record PublishTripCommand(
    Guid Id,
    bool IsPublic
) : IRequest<TripPublishDto>;
