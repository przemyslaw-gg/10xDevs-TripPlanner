using MediatR;
using TripPlanner.Application.TripAttractions.Commands.AddTripAttraction;
using TripPlanner.Application.TripAttractions.Commands.RemoveTripAttraction;
using TripPlanner.Application.TripAttractions.Commands.ReorderTripAttractions;
using TripPlanner.Application.TripAttractions.Commands.UpdateTripAttraction;
using TripPlanner.Application.TripAttractions.DTOs;
using TripPlanner.Application.TripAttractions.Queries.GetTripAttractions;

namespace TripPlanner.WebApi.Endpoints;

/// <summary>
/// Minimal API endpoints for Trip Attraction operations.
/// Manages attractions assigned to trips (schedule, add, update, remove, reorder).
/// All endpoints require authentication.
/// </summary>
public static class TripAttractionEndpoints
{
    /// <summary>
    /// Maps all trip attraction-related endpoints to the application.
    /// </summary>
    public static WebApplication MapTripAttractionEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/trips/{tripId:guid}/attractions")
            .WithTags("Trip Attractions")
            .RequireAuthorization();

        // GET /api/trips/{tripId}/attractions - Get trip schedule with attractions grouped by day
        group.MapGet("/", GetTripAttractions)
            .WithName("GetTripAttractions")
            .WithDescription("Get trip schedule with attractions grouped by day")
            .Produces<TripScheduleDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);

        // POST /api/trips/{tripId}/attractions - Add an attraction to the trip
        group.MapPost("/", AddTripAttraction)
            .WithName("AddTripAttraction")
            .WithDescription("Add an attraction to the trip (owner only)")
            .Produces<TripAttractionDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        // PUT /api/trips/{tripId}/attractions/{attractionId} - Update attraction assignment
        group.MapPut("/{attractionId:guid}", UpdateTripAttraction)
            .WithName("UpdateTripAttraction")
            .WithDescription("Update attraction day, order, or planned time (owner only)")
            .Produces<TripAttractionDto>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);

        // DELETE /api/trips/{tripId}/attractions/{attractionId} - Remove attraction from trip
        group.MapDelete("/{attractionId:guid}", RemoveTripAttraction)
            .WithName("RemoveTripAttraction")
            .WithDescription("Remove an attraction from the trip (owner only)")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);

        // POST /api/trips/{tripId}/attractions/reorder - Batch reorder attractions
        group.MapPost("/reorder", ReorderTripAttractions)
            .WithName("ReorderTripAttractions")
            .WithDescription("Batch update day and order for multiple attractions (owner only)")
            .Produces<ReorderResultDto>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }

    /// <summary>
    /// Retrieves the trip schedule with attractions grouped by day.
    /// </summary>
    private static async Task<IResult> GetTripAttractions(
        Guid tripId,
        ISender mediator,
        CancellationToken cancellationToken = default)
    {
        var query = new GetTripAttractionsQuery(tripId);
        var result = await mediator.Send(query, cancellationToken);

        return Results.Ok(result);
    }

    /// <summary>
    /// Adds an attraction to the trip.
    /// </summary>
    private static async Task<IResult> AddTripAttraction(
        Guid tripId,
        AddTripAttractionRequest request,
        ISender mediator,
        CancellationToken cancellationToken = default)
    {
        var command = new AddTripAttractionCommand(
            tripId,
            request.AttractionId,
            request.DayNumber,
            request.OrderIndex);

        var result = await mediator.Send(command, cancellationToken);

        return Results.Created(
            $"/api/trips/{tripId}/attractions/{request.AttractionId}",
            result);
    }

    /// <summary>
    /// Updates an attraction assignment in the trip.
    /// </summary>
    private static async Task<IResult> UpdateTripAttraction(
        Guid tripId,
        Guid attractionId,
        UpdateTripAttractionRequest request,
        ISender mediator,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateTripAttractionCommand(
            tripId,
            attractionId,
            request.DayNumber,
            request.OrderIndex,
            request.PlannedStartTime);

        var result = await mediator.Send(command, cancellationToken);

        return Results.Ok(result);
    }

    /// <summary>
    /// Removes an attraction from the trip.
    /// </summary>
    private static async Task<IResult> RemoveTripAttraction(
        Guid tripId,
        Guid attractionId,
        ISender mediator,
        CancellationToken cancellationToken = default)
    {
        var command = new RemoveTripAttractionCommand(tripId, attractionId);
        await mediator.Send(command, cancellationToken);

        return Results.NoContent();
    }

    /// <summary>
    /// Batch reorders attractions in the trip.
    /// </summary>
    private static async Task<IResult> ReorderTripAttractions(
        Guid tripId,
        ReorderTripAttractionsRequest request,
        ISender mediator,
        CancellationToken cancellationToken = default)
    {
        var attractions = request.Attractions
            .Select(a => new ReorderAttractionItem(a.AttractionId, a.DayNumber, a.OrderIndex))
            .ToList();

        var command = new ReorderTripAttractionsCommand(tripId, attractions);
        var result = await mediator.Send(command, cancellationToken);

        return Results.Ok(result);
    }
}

/// <summary>
/// Request body for adding an attraction to a trip.
/// </summary>
public record AddTripAttractionRequest(
    Guid AttractionId,
    int DayNumber,
    int OrderIndex
);

/// <summary>
/// Request body for updating a trip attraction assignment.
/// </summary>
public record UpdateTripAttractionRequest(
    int DayNumber,
    int OrderIndex,
    string? PlannedStartTime
);

/// <summary>
/// Single item in a reorder request.
/// </summary>
public record ReorderAttractionItemRequest(
    Guid AttractionId,
    int DayNumber,
    int OrderIndex
);

/// <summary>
/// Request body for batch reordering attractions.
/// </summary>
public record ReorderTripAttractionsRequest(
    IReadOnlyList<ReorderAttractionItemRequest> Attractions
);
