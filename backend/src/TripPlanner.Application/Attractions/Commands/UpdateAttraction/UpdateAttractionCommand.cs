using MediatR;
using TripPlanner.Application.Attractions.DTOs;

namespace TripPlanner.Application.Attractions.Commands.UpdateAttraction;

/// <summary>
/// Command to update an existing user-generated attraction.
/// Only the owner of the attraction can update it.
/// </summary>
/// <param name="Id">The unique identifier of the attraction to update.</param>
/// <param name="Name">The updated name of the attraction.</param>
/// <param name="Description">The updated description of the attraction.</param>
/// <param name="Latitude">The updated latitude coordinate (-90 to 90).</param>
/// <param name="Longitude">The updated longitude coordinate (-180 to 180).</param>
/// <param name="EstimatedDuration">The updated estimated visit duration in minutes.</param>
/// <param name="ImageUrl">The updated URL to the attraction's image.</param>
public record UpdateAttractionCommand(
    Guid Id,
    string Name,
    string? Description,
    decimal Latitude,
    decimal Longitude,
    int? EstimatedDuration,
    string? ImageUrl
) : IRequest<AttractionDto>;
