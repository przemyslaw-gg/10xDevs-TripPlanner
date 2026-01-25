using MediatR;
using TripPlanner.Application.Attractions.DTOs;

namespace TripPlanner.Application.Attractions.Queries.GetAttractionById;

/// <summary>
/// Query to retrieve a single attraction by its unique identifier.
/// </summary>
/// <param name="Id">The unique identifier of the attraction to retrieve.</param>
public record GetAttractionByIdQuery(Guid Id) : IRequest<AttractionDto?>;
