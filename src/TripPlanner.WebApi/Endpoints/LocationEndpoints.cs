namespace TripPlanner.WebApi.Endpoints;

/// <summary>
/// Minimal API endpoints for Location operations.
/// </summary>
public static class LocationEndpoints
{
    public static WebApplication MapLocationEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/locations")
            .WithTags("Locations");

        group.MapGet("/", GetAllLocations)
            .WithName("GetAllLocations")
            .WithDescription("Get all available locations");

        group.MapGet("/{id:guid}", GetLocationById)
            .WithName("GetLocationById")
            .WithDescription("Get a location by ID");

        group.MapGet("/search", SearchLocations)
            .WithName("SearchLocations")
            .WithDescription("Search locations by name");

        return app;
    }

    private static async Task<IResult> GetAllLocations()
    {
        // TODO: Implement with MediatR query
        return Results.Ok(new { Message = "GetAllLocations - Not implemented yet" });
    }

    private static async Task<IResult> GetLocationById(Guid id)
    {
        // TODO: Implement with MediatR query
        return Results.Ok(new { Message = $"GetLocationById {id} - Not implemented yet" });
    }

    private static async Task<IResult> SearchLocations(string? query)
    {
        // TODO: Implement with MediatR query
        return Results.Ok(new { Message = $"SearchLocations '{query}' - Not implemented yet" });
    }
}
