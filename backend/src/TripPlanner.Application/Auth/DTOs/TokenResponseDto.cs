namespace TripPlanner.Application.Auth.DTOs;

/// <summary>
/// Token response DTO for refresh token endpoint.
/// </summary>
public record TokenResponseDto(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn,
    string TokenType
);
