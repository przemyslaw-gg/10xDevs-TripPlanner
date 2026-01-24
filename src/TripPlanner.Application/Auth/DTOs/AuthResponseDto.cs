namespace TripPlanner.Application.Auth.DTOs;

/// <summary>
/// Full authentication response DTO including tokens and user info.
/// </summary>
public record AuthResponseDto(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn,
    string TokenType,
    UserInfoDto User
);
