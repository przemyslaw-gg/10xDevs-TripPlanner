using MediatR;
using TripPlanner.Application.TripAttractions.DTOs;

namespace TripPlanner.Application.TripAttractions.Commands.UpdateTripAttraction;

/// <summary>
/// Command to update a trip attraction assignment.
/// </summary>
/// <param name="TripId">The ID of the trip.</param>
/// <param name="AttractionId">The ID of the attraction to update.</param>
/// <param name="DayNumber">The new day number within the trip (1-based).</param>
/// <param name="OrderIndex">The new order of the attraction within the day (1-based).</param>
/// <param name="PlannedStartTime">The planned start time in HH:mm format, or null to clear.</param>
public record UpdateTripAttractionCommand(
    Guid TripId,
    Guid AttractionId,
    int DayNumber,
    int OrderIndex,
    string? PlannedStartTime
) : IRequest<TripAttractionDto>;
