using MediatR;
using TripPlanner.Application.Attractions.DTOs;

namespace TripPlanner.Application.Attractions.Commands.CreateAttraction;

/// <summary>
/// Command to create a new user-generated attraction.
/// </summary>
/// <param name="LocationId">The ID of the location where the attraction is situated.</param>
/// <param name="Name">The name of the attraction.</param>
/// <param name="Description">Optional description of the attraction.</param>
/// <param name="Latitude">The latitude coordinate (-90 to 90).</param>
/// <param name="Longitude">The longitude coordinate (-180 to 180).</param>
/// <param name="EstimatedDuration">Optional estimated visit duration in minutes.</param>
/// <param name="ImageUrl">Optional URL to the attraction's image.</param>
public record CreateAttractionCommand(
    Guid LocationId,
    string Name,
    string? Description,
    decimal Latitude,
    decimal Longitude,
    int? EstimatedDuration,
    string? ImageUrl
) : IRequest<AttractionDto>;
