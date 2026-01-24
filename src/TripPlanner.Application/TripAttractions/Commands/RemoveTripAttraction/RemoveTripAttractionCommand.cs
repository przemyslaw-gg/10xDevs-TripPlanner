using MediatR;

namespace TripPlanner.Application.TripAttractions.Commands.RemoveTripAttraction;

/// <summary>
/// Command to remove an attraction from a trip.
/// </summary>
/// <param name="TripId">The ID of the trip.</param>
/// <param name="AttractionId">The ID of the attraction to remove.</param>
public record RemoveTripAttractionCommand(
    Guid TripId,
    Guid AttractionId
) : IRequest<Unit>;
