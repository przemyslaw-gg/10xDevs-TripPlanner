using FluentValidation;

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
