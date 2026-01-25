using MediatR;
using TripPlanner.Application.Trips.Commands.CreateTrip;
using TripPlanner.Application.Trips.Commands.DeleteTrip;
using TripPlanner.Application.Trips.Commands.PublishTrip;
using TripPlanner.Application.Trips.Commands.UpdateTrip;
using TripPlanner.Application.Trips.DTOs;
using TripPlanner.Application.Trips.Queries.GetTripById;
using TripPlanner.Application.Trips.Queries.GetTrips;
using TripPlanner.WebApi.Contracts;

namespace TripPlanner.WebApi.Endpoints;

/// <summary>
/// Minimal API endpoints for Trip operations.
/// All endpoints require authentication.
/// </summary>
public static class TripEndpoints
{
    /// <summary>
    /// Maps all trip-related endpoints to the application.
    /// </summary>
    public static WebApplication MapTripEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/trips")
            .WithTags("Trips")
            .RequireAuthorization();

        // GET /api/trips - List trips with filtering and pagination
        group.MapGet("/", GetTrips)
            .WithName("GetTrips")
            .WithDescription("List trips with optional filtering by location, ownership, and search")
            .Produces<PaginatedResponse<TripListItemDto>>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        // GET /api/trips/{id} - Get a specific trip
        group.MapGet("/{id:guid}", GetTripById)
            .WithName("GetTripById")
            .WithDescription("Get a specific trip by ID")
            .Produces<TripDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);

        // POST /api/trips - Create a new trip
        group.MapPost("/", CreateTrip)
            .WithName("CreateTrip")
            .WithDescription("Create a new trip")
            .Produces<TripDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);

        // PUT /api/trips/{id} - Update an existing trip
        group.MapPut("/{id:guid}", UpdateTrip)
            .WithName("UpdateTrip")
            .WithDescription("Update an existing trip (owner only)")
            .Produces<TripDto>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);

        // DELETE /api/trips/{id} - Delete a trip
        group.MapDelete("/{id:guid}", DeleteTrip)
            .WithName("DeleteTrip")
            .WithDescription("Delete a trip (owner only)")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);

        // PATCH /api/trips/{id}/publish - Change trip visibility
        group.MapPatch("/{id:guid}/publish", PublishTrip)
            .WithName("PublishTrip")
            .WithDescription("Change trip public/private visibility (owner only)")
            .Produces<TripPublishDto>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }

    /// <summary>
    /// Retrieves a paginated list of trips with optional filtering.
    /// </summary>
    private static async Task<IResult> GetTrips(
        ISender mediator,
        Guid? locationId = null,
        bool onlyMine = false,
        bool onlyPublic = false,
        string? search = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetTripsQuery(locationId, onlyMine, onlyPublic, search, page, pageSize);
        var result = await mediator.Send(query, cancellationToken);

        var response = new PaginatedResponse<TripListItemDto>(
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
    /// Retrieves a single trip by its ID.
    /// </summary>
    private static async Task<IResult> GetTripById(
        Guid id,
        ISender mediator,
        CancellationToken cancellationToken = default)
    {
        var query = new GetTripByIdQuery(id);
        var result = await mediator.Send(query, cancellationToken);

        return Results.Ok(result);
    }

    /// <summary>
    /// Creates a new trip.
    /// </summary>
    private static async Task<IResult> CreateTrip(
        CreateTripRequest request,
        ISender mediator,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateTripCommand(
            request.Name,
            request.LocationId,
            request.DailyHours,
            request.MaxExtensionHours,
            request.StartTime);

        var result = await mediator.Send(command, cancellationToken);

        return Results.Created($"/api/trips/{result.Id}", result);
    }

    /// <summary>
    /// Updates an existing trip.
    /// </summary>
    private static async Task<IResult> UpdateTrip(
        Guid id,
        UpdateTripRequest request,
        ISender mediator,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateTripCommand(
            id,
            request.Name,
            request.LocationId,
            request.DailyHours,
            request.MaxExtensionHours,
            request.StartTime);

        var result = await mediator.Send(command, cancellationToken);

        return Results.Ok(result);
    }

    /// <summary>
    /// Deletes a trip.
    /// </summary>
    private static async Task<IResult> DeleteTrip(
        Guid id,
        ISender mediator,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteTripCommand(id);
        await mediator.Send(command, cancellationToken);

        return Results.NoContent();
    }

    /// <summary>
    /// Changes the public visibility of a trip.
    /// </summary>
    private static async Task<IResult> PublishTrip(
        Guid id,
        PublishTripRequest request,
        ISender mediator,
        CancellationToken cancellationToken = default)
    {
        var command = new PublishTripCommand(id, request.IsPublic);
        var result = await mediator.Send(command, cancellationToken);

        return Results.Ok(result);
    }
}

/// <summary>
/// Request body for creating a new trip.
/// </summary>
public record CreateTripRequest(
    string Name,
    Guid? LocationId,
    int DailyHours,
    int MaxExtensionHours,
    string StartTime
);

/// <summary>
/// Request body for updating an existing trip.
/// </summary>
public record UpdateTripRequest(
    string Name,
    Guid? LocationId,
    int DailyHours,
    int MaxExtensionHours,
    string StartTime
);

/// <summary>
/// Request body for changing trip visibility.
/// </summary>
public record PublishTripRequest(
    bool IsPublic
);
