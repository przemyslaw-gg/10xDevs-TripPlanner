namespace TripPlanner.Application.Trips.DTOs;

/// <summary>
/// Location details with timezone information for trip responses.
/// </summary>
/// <param name="Id">The unique identifier of the location.</param>
/// <param name="Name">The name of the location (city/region).</param>
/// <param name="Country">The country where the location is situated.</param>
/// <param name="Timezone">The IANA timezone identifier (e.g., 'Europe/Athens').</param>
public record LocationDetailDto(
    Guid Id,
    string Name,
    string Country,
    string? Timezone
);
