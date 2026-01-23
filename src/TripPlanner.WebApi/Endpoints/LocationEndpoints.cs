using MediatR;
using TripPlanner.Application.Locations.DTOs;
using TripPlanner.Application.Locations.Queries.GetLocations;
using TripPlanner.Application.Locations.Queries.GetLocationById;
using TripPlanner.WebApi.Contracts;

namespace TripPlanner.WebApi.Endpoints;

/// <summary>
/// Minimal API endpoints for Location operations.
/// </summary>
public static class LocationEndpoints
{
    /// <summary>
    /// Maps all location-related endpoints to the application.
    /// </summary>
    /// <param name="app">The web application to configure.</param>
    /// <returns>The configured web application.</returns>
    public static WebApplication MapLocationEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/locations")
            .WithTags("Locations")
            .WithOpenApi();

        group.MapGet("/", GetLocations)
            .WithName("GetLocations")
            .WithDescription("List all locations with optional search and pagination")
            .Produces<PaginatedResponse<LocationListItemDto>>(StatusCodes.Status200OK)
            .ProducesValidationProblem();

        group.MapGet("/{id:guid}", GetLocationById)
            .WithName("GetLocationById")
            .WithDescription("Get a specific location by ID")
            .Produces<LocationDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }

    /// <summary>
    /// Retrieves a paginated list of locations with optional search filtering.
    /// </summary>
    /// <param name="mediator">The MediatR sender for dispatching queries.</param>
    /// <param name="search">Optional search term to filter by name or country.</param>
    /// <param name="page">The page number (default: 1).</param>
    /// <param name="pageSize">The number of items per page (default: 20, max: 100).</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A paginated response containing location items.</returns>
    private static async Task<IResult> GetLocations(
        ISender mediator,
        string? search = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetLocationsQuery(search, page, pageSize);
        var result = await mediator.Send(query, cancellationToken);

        var response = new PaginatedResponse<LocationListItemDto>(
            result.Items,
            new PaginationMetadata(
                result.Page,
                result.PageSize,
                result.TotalItems,
                result.TotalPages,
                result.HasNextPage,
                result.HasPreviousPage
            )
        );

        return Results.Ok(response);
    }

    /// <summary>
    /// Retrieves a single location by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the location.</param>
    /// <param name="mediator">The MediatR sender for dispatching queries.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The location details or a 404 Not Found response.</returns>
    private static async Task<IResult> GetLocationById(
        Guid id,
        ISender mediator,
        CancellationToken cancellationToken = default)
    {
        var query = new GetLocationByIdQuery(id);
        var result = await mediator.Send(query, cancellationToken);

        return result is not null
            ? Results.Ok(result)
            : Results.Problem(
                title: "Not Found",
                detail: $"Location with ID '{id}' was not found",
                statusCode: StatusCodes.Status404NotFound);
    }
}
