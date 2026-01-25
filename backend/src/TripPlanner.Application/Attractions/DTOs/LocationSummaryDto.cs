namespace TripPlanner.Application.Attractions.DTOs;

/// <summary>
/// Simplified location information for embedding in attraction responses.
/// </summary>
/// <param name="Id">The unique identifier of the location.</param>
/// <param name="Name">The name of the location (city/region).</param>
/// <param name="Country">The country where the location is situated.</param>
public record LocationSummaryDto(
    Guid Id,
    string Name,
    string Country
);
