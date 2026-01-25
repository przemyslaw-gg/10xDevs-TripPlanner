using MediatR;

namespace TripPlanner.Application.Attractions.Commands.DeleteAttraction;

/// <summary>
/// Command to delete a user-generated attraction.
/// Only the owner of the attraction can delete it.
/// </summary>
/// <param name="Id">The unique identifier of the attraction to delete.</param>
public record DeleteAttractionCommand(Guid Id) : IRequest<Unit>;
