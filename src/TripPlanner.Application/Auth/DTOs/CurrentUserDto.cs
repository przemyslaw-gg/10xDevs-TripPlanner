namespace TripPlanner.Application.Auth.DTOs;

/// <summary>
/// Full current user information DTO for the /me endpoint.
/// </summary>
public record CurrentUserDto(
    Guid Id,
    string Email,
    string? DisplayName,
    bool EmailVerified,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
