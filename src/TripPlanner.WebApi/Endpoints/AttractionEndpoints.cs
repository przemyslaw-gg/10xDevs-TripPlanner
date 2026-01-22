namespace TripPlanner.WebApi.Endpoints;

/// <summary>
/// Minimal API endpoints for Attraction operations.
/// </summary>
public static class AttractionEndpoints
{
    public static WebApplication MapAttractionEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/attractions")
            .WithTags("Attractions");

        group.MapGet("/", GetAttractions)
            .WithName("GetAttractions")
            .WithDescription("Get attractions, optionally filtered by location");

        group.MapGet("/{id:guid}", GetAttractionById)
            .WithName("GetAttractionById")
            .WithDescription("Get an attraction by ID");

        group.MapPost("/", CreateAttraction)
            .WithName("CreateAttraction")
            .WithDescription("Create a new attraction");

        group.MapPut("/{id:guid}", UpdateAttraction)
            .WithName("UpdateAttraction")
            .WithDescription("Update an existing attraction");

        group.MapDelete("/{id:guid}", DeleteAttraction)
            .WithName("DeleteAttraction")
            .WithDescription("Delete an attraction");

        return app;
    }

    private static async Task<IResult> GetAttractions(Guid? locationId)
    {
        // TODO: Implement with MediatR query
        return Results.Ok(new { Message = $"GetAttractions for location {locationId} - Not implemented yet" });
    }

    private static async Task<IResult> GetAttractionById(Guid id)
    {
        // TODO: Implement with MediatR query
        return Results.Ok(new { Message = $"GetAttractionById {id} - Not implemented yet" });
    }

    private static async Task<IResult> CreateAttraction()
    {
        // TODO: Implement with MediatR command
        return Results.Ok(new { Message = "CreateAttraction - Not implemented yet" });
    }

    private static async Task<IResult> UpdateAttraction(Guid id)
    {
        // TODO: Implement with MediatR command
        return Results.Ok(new { Message = $"UpdateAttraction {id} - Not implemented yet" });
    }

    private static async Task<IResult> DeleteAttraction(Guid id)
    {
        // TODO: Implement with MediatR command
        return Results.Ok(new { Message = $"DeleteAttraction {id} - Not implemented yet" });
    }
}
