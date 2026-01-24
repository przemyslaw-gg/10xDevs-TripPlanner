namespace TripPlanner.Application.Auth.DTOs;

/// <summary>
/// Response DTO for user registration.
/// </summary>
public record RegisteredUserDto(
    Guid Id,
    string Email,
    string? DisplayName,
    DateTime CreatedAt
);
