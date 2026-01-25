namespace TripPlanner.Application.Attractions.DTOs;

/// <summary>
/// Data transfer object for detailed attraction information.
/// Used when retrieving a single attraction by ID or after create/update operations.
/// </summary>
/// <param name="Id">The unique identifier of the attraction.</param>
/// <param name="LocationId">The ID of the location where the attraction is situated.</param>
/// <param name="Location">Summary information about the location.</param>
/// <param name="Name">The name of the attraction.</param>
/// <param name="Description">A description of the attraction.</param>
/// <param name="Latitude">The latitude coordinate of the attraction.</param>
/// <param name="Longitude">The longitude coordinate of the attraction.</param>
/// <param name="Rating">The average rating of the attraction (0-5). Null for unrated attractions.</param>
/// <param name="ReviewCount">The number of reviews. Null for user-generated attractions without reviews.</param>
/// <param name="EstimatedDuration">Estimated visit duration in minutes.</param>
/// <param name="ImageUrl">URL to the attraction's image.</param>
/// <param name="IsVerified">Whether the attraction is verified (system-generated) or user-created.</param>
/// <param name="CreatedByUserId">The ID of the user who created this attraction (null for system attractions).</param>
/// <param name="CreatedAt">The date and time when the attraction was created.</param>
/// <param name="UpdatedAt">The date and time when the attraction was last updated.</param>
public record AttractionDto(
    Guid Id,
    Guid LocationId,
    LocationSummaryDto Location,
    string Name,
    string? Description,
    decimal Latitude,
    decimal Longitude,
    decimal? Rating,
    int? ReviewCount,
    int EstimatedDuration,
    string? ImageUrl,
    bool IsVerified,
    Guid? CreatedByUserId,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
