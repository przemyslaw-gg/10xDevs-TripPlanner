namespace TripPlanner.Application.Auth.DTOs;

/// <summary>
/// Basic user information DTO included in auth responses.
/// </summary>
public record UserInfoDto(
    Guid Id,
    string Email,
    string? DisplayName
);
