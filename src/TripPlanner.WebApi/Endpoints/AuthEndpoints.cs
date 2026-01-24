using MediatR;
using TripPlanner.Application.Auth.Commands.Login;
using TripPlanner.Application.Auth.Commands.Logout;
using TripPlanner.Application.Auth.Commands.RefreshToken;
using TripPlanner.Application.Auth.Commands.RegisterUser;
using TripPlanner.Application.Auth.DTOs;
using TripPlanner.Application.Auth.Queries.GetCurrentUser;

namespace TripPlanner.WebApi.Endpoints;

/// <summary>
/// Minimal API endpoints for Authentication operations.
/// </summary>
public static class AuthEndpoints
{
    /// <summary>
    /// Maps all authentication-related endpoints to the application.
    /// </summary>
    /// <param name="app">The web application to configure.</param>
    /// <returns>The configured web application.</returns>
    public static WebApplication MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Authentication");

        // Public endpoints with rate limiting
        group.MapPost("/register", Register)
            .WithName("Register")
            .WithDescription("Register a new user account")
            .RequireRateLimiting("auth")
            .Produces<RegisteredUserDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPost("/login", Login)
            .WithName("Login")
            .WithDescription("Authenticate user and receive tokens")
            .RequireRateLimiting("auth")
            .Produces<AuthResponseDto>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        group.MapPost("/refresh", Refresh)
            .WithName("RefreshToken")
            .WithDescription("Refresh access token using a valid refresh token")
            .Produces<TokenResponseDto>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        // Authenticated endpoints
        group.MapPost("/logout", Logout)
            .WithName("Logout")
            .WithDescription("Revoke refresh token and logout")
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        group.MapGet("/me", GetCurrentUser)
            .WithName("GetCurrentUser")
            .WithDescription("Get current authenticated user information")
            .RequireAuthorization()
            .Produces<CurrentUserDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }

    /// <summary>
    /// Registers a new user account.
    /// </summary>
    private static async Task<IResult> Register(
        RegisterRequest request,
        ISender mediator,
        CancellationToken cancellationToken = default)
    {
        var command = new RegisterUserCommand(
            request.Email,
            request.Password,
            request.DisplayName);

        var result = await mediator.Send(command, cancellationToken);

        return Results.Created($"/api/auth/me", result);
    }

    /// <summary>
    /// Authenticates a user and returns JWT tokens.
    /// </summary>
    private static async Task<IResult> Login(
        LoginRequest request,
        ISender mediator,
        CancellationToken cancellationToken = default)
    {
        var command = new LoginCommand(
            request.Email,
            request.Password);

        var result = await mediator.Send(command, cancellationToken);

        return Results.Ok(result);
    }

    /// <summary>
    /// Refreshes an access token using a valid refresh token.
    /// </summary>
    private static async Task<IResult> Refresh(
        RefreshRequest request,
        ISender mediator,
        CancellationToken cancellationToken = default)
    {
        var command = new RefreshTokenCommand(request.RefreshToken);

        var result = await mediator.Send(command, cancellationToken);

        return Results.Ok(result);
    }

    /// <summary>
    /// Logs out the user by revoking the refresh token.
    /// </summary>
    private static async Task<IResult> Logout(
        LogoutRequest request,
        ISender mediator,
        CancellationToken cancellationToken = default)
    {
        var command = new LogoutCommand(request.RefreshToken);

        await mediator.Send(command, cancellationToken);

        return Results.NoContent();
    }

    /// <summary>
    /// Gets the current authenticated user's information.
    /// </summary>
    private static async Task<IResult> GetCurrentUser(
        ISender mediator,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCurrentUserQuery();

        var result = await mediator.Send(query, cancellationToken);

        return Results.Ok(result);
    }
}

/// <summary>
/// Request body for user registration.
/// </summary>
/// <param name="Email">The user's email address.</param>
/// <param name="Password">The user's password (min 8 chars, must contain uppercase, lowercase, and digit).</param>
/// <param name="DisplayName">Optional display name.</param>
public record RegisterRequest(
    string Email,
    string Password,
    string? DisplayName
);

/// <summary>
/// Request body for user login.
/// </summary>
/// <param name="Email">The user's email address.</param>
/// <param name="Password">The user's password.</param>
public record LoginRequest(
    string Email,
    string Password
);

/// <summary>
/// Request body for token refresh.
/// </summary>
/// <param name="RefreshToken">The refresh token.</param>
public record RefreshRequest(
    string RefreshToken
);

/// <summary>
/// Request body for logout.
/// </summary>
/// <param name="RefreshToken">The refresh token to revoke.</param>
public record LogoutRequest(
    string RefreshToken
);
