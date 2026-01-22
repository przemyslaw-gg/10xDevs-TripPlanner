namespace TripPlanner.WebApi.Endpoints;

/// <summary>
/// Minimal API endpoints for Trip operations.
/// </summary>
public static class TripEndpoints
{
    public static WebApplication MapTripEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/trips")
            .WithTags("Trips");

        group.MapGet("/", GetTrips)
            .WithName("GetTrips")
            .WithDescription("Get all trips for the current user");

        group.MapGet("/public", GetPublicTrips)
            .WithName("GetPublicTrips")
            .WithDescription("Get all public trips");

        group.MapGet("/{id:guid}", GetTripById)
            .WithName("GetTripById")
            .WithDescription("Get a trip by ID with its attractions");

        group.MapPost("/", CreateTrip)
            .WithName("CreateTrip")
            .WithDescription("Create a new trip");

        group.MapPut("/{id:guid}", UpdateTrip)
            .WithName("UpdateTrip")
            .WithDescription("Update an existing trip");

        group.MapDelete("/{id:guid}", DeleteTrip)
            .WithName("DeleteTrip")
            .WithDescription("Delete a trip");

        group.MapPost("/{id:guid}/attractions", AddAttractionToTrip)
            .WithName("AddAttractionToTrip")
            .WithDescription("Add an attraction to a trip");

        group.MapDelete("/{id:guid}/attractions/{attractionId:guid}", RemoveAttractionFromTrip)
            .WithName("RemoveAttractionFromTrip")
            .WithDescription("Remove an attraction from a trip");

        group.MapPut("/{id:guid}/attractions/reorder", ReorderAttractions)
            .WithName("ReorderAttractions")
            .WithDescription("Reorder attractions in a trip");

        group.MapPost("/{id:guid}/optimize", OptimizeRoute)
            .WithName("OptimizeRoute")
            .WithDescription("Optimize the route for a trip using nearest neighbor algorithm");

        return app;
    }

    private static async Task<IResult> GetTrips()
    {
        // TODO: Implement with MediatR query
        return Results.Ok(new { Message = "GetTrips - Not implemented yet" });
    }

    private static async Task<IResult> GetPublicTrips()
    {
        // TODO: Implement with MediatR query
        return Results.Ok(new { Message = "GetPublicTrips - Not implemented yet" });
    }

    private static async Task<IResult> GetTripById(Guid id)
    {
        // TODO: Implement with MediatR query
        return Results.Ok(new { Message = $"GetTripById {id} - Not implemented yet" });
    }

    private static async Task<IResult> CreateTrip()
    {
        // TODO: Implement with MediatR command
        return Results.Ok(new { Message = "CreateTrip - Not implemented yet" });
    }

    private static async Task<IResult> UpdateTrip(Guid id)
    {
        // TODO: Implement with MediatR command
        return Results.Ok(new { Message = $"UpdateTrip {id} - Not implemented yet" });
    }

    private static async Task<IResult> DeleteTrip(Guid id)
    {
        // TODO: Implement with MediatR command
        return Results.Ok(new { Message = $"DeleteTrip {id} - Not implemented yet" });
    }

    private static async Task<IResult> AddAttractionToTrip(Guid id)
    {
        // TODO: Implement with MediatR command
        return Results.Ok(new { Message = $"AddAttractionToTrip {id} - Not implemented yet" });
    }

    private static async Task<IResult> RemoveAttractionFromTrip(Guid id, Guid attractionId)
    {
        // TODO: Implement with MediatR command
        return Results.Ok(new { Message = $"RemoveAttractionFromTrip {id}/{attractionId} - Not implemented yet" });
    }

    private static async Task<IResult> ReorderAttractions(Guid id)
    {
        // TODO: Implement with MediatR command
        return Results.Ok(new { Message = $"ReorderAttractions {id} - Not implemented yet" });
    }

    private static async Task<IResult> OptimizeRoute(Guid id)
    {
        // TODO: Implement with MediatR command
        return Results.Ok(new { Message = $"OptimizeRoute {id} - Not implemented yet" });
    }
}
