namespace TripPlanner.Application.Locations.DTOs;

/// <summary>
/// Data transfer object for detailed location information.
/// Used when retrieving a single location by ID.
/// </summary>
/// <param name="Id">The unique identifier of the location.</param>
/// <param name="Name">The name of the location (city/region).</param>
/// <param name="Country">The country where the location is situated.</param>
/// <param name="Timezone">The IANA timezone identifier (e.g., 'Europe/Athens').</param>
/// <param name="CreatedAt">The date and time when the location was created.</param>
/// <param name="UpdatedAt">The date and time when the location was last updated.</param>
public record LocationDto(
    Guid Id,
    string Name,
    string Country,
    string? Timezone,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
