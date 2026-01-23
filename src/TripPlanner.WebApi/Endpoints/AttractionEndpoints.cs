using MediatR;
using TripPlanner.Application.Attractions.Commands.CreateAttraction;
using TripPlanner.Application.Attractions.Commands.DeleteAttraction;
using TripPlanner.Application.Attractions.Commands.UpdateAttraction;
using TripPlanner.Application.Attractions.DTOs;
using TripPlanner.Application.Attractions.Queries.GetAttractionById;
using TripPlanner.Application.Attractions.Queries.GetAttractions;
using TripPlanner.WebApi.Contracts;

namespace TripPlanner.WebApi.Endpoints;

/// <summary>
/// Minimal API endpoints for Attraction operations.
/// </summary>
public static class AttractionEndpoints
{
    /// <summary>
    /// Maps all attraction-related endpoints to the application.
    /// </summary>
    /// <param name="app">The web application to configure.</param>
    /// <returns>The configured web application.</returns>
    public static WebApplication MapAttractionEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/attractions")
            .WithTags("Attractions");

        // Public endpoints
        group.MapGet("/", GetAttractions)
            .WithName("GetAttractions")
            .WithDescription("List attractions with filtering, sorting, and pagination")
            .Produces<PaginatedResponse<AttractionListItemDto>>(StatusCodes.Status200OK)
            .ProducesValidationProblem();

        group.MapGet("/{id:guid}", GetAttractionById)
            .WithName("GetAttractionById")
            .WithDescription("Get a specific attraction by ID")
            .Produces<AttractionDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        // Authenticated endpoints
        group.MapPost("/", CreateAttraction)
            .WithName("CreateAttraction")
            .WithDescription("Create a new custom attraction (user-generated)")
            .RequireAuthorization()
            .Produces<AttractionDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPut("/{id:guid}", UpdateAttraction)
            .WithName("UpdateAttraction")
            .WithDescription("Update a custom attraction (only owner can update)")
            .RequireAuthorization()
            .Produces<AttractionDto>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:guid}", DeleteAttraction)
            .WithName("DeleteAttraction")
            .WithDescription("Delete a custom attraction (only owner can delete)")
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        return app;
    }

    /// <summary>
    /// Retrieves a paginated list of attractions with filtering and sorting.
    /// </summary>
    private static async Task<IResult> GetAttractions(
        ISender mediator,
        Guid? locationId = null,
        string? search = null,
        string sortBy = "rating",
        string sortOrder = "desc",
        bool? isVerified = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAttractionsQuery(
            locationId,
            search,
            sortBy,
            sortOrder,
            isVerified,
            page,
            pageSize);

        var result = await mediator.Send(query, cancellationToken);

        var response = new PaginatedResponse<AttractionListItemDto>(
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
    /// Retrieves a single attraction by its unique identifier.
    /// </summary>
    private static async Task<IResult> GetAttractionById(
        Guid id,
        ISender mediator,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAttractionByIdQuery(id);
        var result = await mediator.Send(query, cancellationToken);

        return result is not null
            ? Results.Ok(result)
            : Results.Problem(
                title: "Not Found",
                detail: $"Attraction with ID '{id}' was not found",
                statusCode: StatusCodes.Status404NotFound);
    }

    /// <summary>
    /// Creates a new user-generated attraction.
    /// </summary>
    private static async Task<IResult> CreateAttraction(
        CreateAttractionRequest request,
        ISender mediator,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateAttractionCommand(
            request.LocationId,
            request.Name,
            request.Description,
            request.Latitude,
            request.Longitude,
            request.EstimatedDuration,
            request.ImageUrl);

        var result = await mediator.Send(command, cancellationToken);

        return Results.Created($"/api/attractions/{result.Id}", result);
    }

    /// <summary>
    /// Updates an existing user-generated attraction.
    /// </summary>
    private static async Task<IResult> UpdateAttraction(
        Guid id,
        UpdateAttractionRequest request,
        ISender mediator,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateAttractionCommand(
            id,
            request.Name,
            request.Description,
            request.Latitude,
            request.Longitude,
            request.EstimatedDuration,
            request.ImageUrl);

        var result = await mediator.Send(command, cancellationToken);

        return Results.Ok(result);
    }

    /// <summary>
    /// Deletes a user-generated attraction.
    /// </summary>
    private static async Task<IResult> DeleteAttraction(
        Guid id,
        ISender mediator,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteAttractionCommand(id);
        await mediator.Send(command, cancellationToken);

        return Results.NoContent();
    }
}

/// <summary>
/// Request body for creating a new attraction.
/// </summary>
/// <param name="LocationId">The ID of the location where the attraction is situated.</param>
/// <param name="Name">The name of the attraction.</param>
/// <param name="Description">Optional description of the attraction.</param>
/// <param name="Latitude">The latitude coordinate (-90 to 90).</param>
/// <param name="Longitude">The longitude coordinate (-180 to 180).</param>
/// <param name="EstimatedDuration">Optional estimated visit duration in minutes.</param>
/// <param name="ImageUrl">Optional URL to the attraction's image.</param>
public record CreateAttractionRequest(
    Guid LocationId,
    string Name,
    string? Description,
    decimal Latitude,
    decimal Longitude,
    int? EstimatedDuration,
    string? ImageUrl
);

/// <summary>
/// Request body for updating an existing attraction.
/// </summary>
/// <param name="Name">The updated name of the attraction.</param>
/// <param name="Description">The updated description of the attraction.</param>
/// <param name="Latitude">The updated latitude coordinate (-90 to 90).</param>
/// <param name="Longitude">The updated longitude coordinate (-180 to 180).</param>
/// <param name="EstimatedDuration">The updated estimated visit duration in minutes.</param>
/// <param name="ImageUrl">The updated URL to the attraction's image.</param>
public record UpdateAttractionRequest(
    string Name,
    string? Description,
    decimal Latitude,
    decimal Longitude,
    int? EstimatedDuration,
    string? ImageUrl
);
