using FluentValidation;
using TripPlanner.Application.Common.Exceptions;

namespace TripPlanner.WebApi.Middleware;

/// <summary>
/// Middleware for handling exceptions globally and converting them to appropriate HTTP responses.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            await HandleValidationExceptionAsync(context, ex);
        }
        catch (NotFoundException ex)
        {
            await HandleNotFoundExceptionAsync(context, ex);
        }
        catch (ForbiddenAccessException ex)
        {
            await HandleForbiddenAccessExceptionAsync(context, ex);
        }
        catch (ConflictException ex)
        {
            await HandleConflictExceptionAsync(context, ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            await HandleUnauthorizedExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while processing request {Path}", context.Request.Path);
            await HandleGenericExceptionAsync(context);
        }
    }

    /// <summary>
    /// Handles FluentValidation exceptions and returns a 400 Bad Request response.
    /// </summary>
    private static async Task HandleValidationExceptionAsync(
        HttpContext context,
        ValidationException exception)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.ContentType = "application/problem+json";

        var errors = exception.Errors
            .GroupBy(e => ToCamelCase(e.PropertyName))
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray()
            );

        var problemDetails = new
        {
            type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            title = "Validation Failed",
            status = 400,
            errors
        };

        await context.Response.WriteAsJsonAsync(problemDetails);
    }

    /// <summary>
    /// Handles NotFoundException and returns a 404 Not Found response.
    /// </summary>
    private static async Task HandleNotFoundExceptionAsync(
        HttpContext context,
        NotFoundException exception)
    {
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        context.Response.ContentType = "application/problem+json";

        var problemDetails = new
        {
            type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            title = "Not Found",
            status = 404,
            detail = exception.Message
        };

        await context.Response.WriteAsJsonAsync(problemDetails);
    }

    /// <summary>
    /// Handles ForbiddenAccessException and returns a 403 Forbidden response.
    /// </summary>
    private static async Task HandleForbiddenAccessExceptionAsync(
        HttpContext context,
        ForbiddenAccessException exception)
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        context.Response.ContentType = "application/problem+json";

        var problemDetails = new
        {
            type = "https://tools.ietf.org/html/rfc7231#section-6.5.3",
            title = "Forbidden",
            status = 403,
            detail = exception.Message
        };

        await context.Response.WriteAsJsonAsync(problemDetails);
    }

    /// <summary>
    /// Handles ConflictException and returns a 409 Conflict response.
    /// </summary>
    private static async Task HandleConflictExceptionAsync(
        HttpContext context,
        ConflictException exception)
    {
        context.Response.StatusCode = StatusCodes.Status409Conflict;
        context.Response.ContentType = "application/problem+json";

        var problemDetails = new
        {
            type = "https://tools.ietf.org/html/rfc7231#section-6.5.8",
            title = "Conflict",
            status = 409,
            detail = exception.Message
        };

        await context.Response.WriteAsJsonAsync(problemDetails);
    }

    /// <summary>
    /// Handles UnauthorizedAccessException and returns a 401 Unauthorized response.
    /// </summary>
    private static async Task HandleUnauthorizedExceptionAsync(
        HttpContext context,
        UnauthorizedAccessException exception)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/problem+json";

        var problemDetails = new
        {
            type = "https://tools.ietf.org/html/rfc7235#section-3.1",
            title = "Unauthorized",
            status = 401,
            detail = "Authentication is required to access this resource."
        };

        await context.Response.WriteAsJsonAsync(problemDetails);
    }

    /// <summary>
    /// Handles unexpected exceptions and returns a 500 Internal Server Error response.
    /// </summary>
    private static async Task HandleGenericExceptionAsync(HttpContext context)
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";

        var problemDetails = new
        {
            type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            title = "Internal Server Error",
            status = 500,
            detail = "An unexpected error occurred. Please try again later."
        };

        await context.Response.WriteAsJsonAsync(problemDetails);
    }

    /// <summary>
    /// Converts a property name to camelCase for JSON response consistency.
    /// </summary>
    private static string ToCamelCase(string value)
    {
        if (string.IsNullOrEmpty(value) || !char.IsUpper(value[0]))
            return value;

        return char.ToLowerInvariant(value[0]) + value[1..];
    }
}
