using MediatR;
using TripPlanner.Application.TripAttractions.DTOs;

namespace TripPlanner.Application.TripAttractions.Commands.AddTripAttraction;

/// <summary>
/// Command to add an attraction to a trip.
/// </summary>
/// <param name="TripId">The ID of the trip.</param>
/// <param name="AttractionId">The ID of the attraction to add.</param>
/// <param name="DayNumber">The day number within the trip (1-based).</param>
/// <param name="OrderIndex">The order of the attraction within the day (1-based).</param>
public record AddTripAttractionCommand(
    Guid TripId,
    Guid AttractionId,
    int DayNumber,
    int OrderIndex
) : IRequest<TripAttractionDto>;
