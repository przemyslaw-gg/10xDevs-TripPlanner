using MediatR;
using TripPlanner.Application.TripAttractions.DTOs;

namespace TripPlanner.Application.TripAttractions.Commands.ReorderTripAttractions;

/// <summary>
/// Item representing a single attraction's new position in the reorder operation.
/// </summary>
/// <param name="AttractionId">The ID of the attraction.</param>
/// <param name="DayNumber">The new day number (1-based).</param>
/// <param name="OrderIndex">The new order within the day (1-based).</param>
public record ReorderAttractionItem(
    Guid AttractionId,
    int DayNumber,
    int OrderIndex
);

/// <summary>
/// Command to batch reorder attractions in a trip.
/// </summary>
/// <param name="TripId">The ID of the trip.</param>
/// <param name="Attractions">List of attractions with their new positions.</param>
public record ReorderTripAttractionsCommand(
    Guid TripId,
    IReadOnlyList<ReorderAttractionItem> Attractions
) : IRequest<ReorderResultDto>;
