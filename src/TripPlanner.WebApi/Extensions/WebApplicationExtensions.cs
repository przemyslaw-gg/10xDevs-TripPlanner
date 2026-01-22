using TripPlanner.WebApi.Endpoints;

namespace TripPlanner.WebApi.Extensions;

/// <summary>
/// Extension methods for configuring the WebApplication.
/// </summary>
public static class WebApplicationExtensions
{
    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        // Development-specific middleware
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "TripPlanner API v1");
            });
        }

        // Global middleware
        app.UseHttpsRedirection();
        app.UseCors();

        // Map endpoints
        app.MapEndpoints();

        return app;
    }

    public static WebApplication MapEndpoints(this WebApplication app)
    {
        // Health check endpoint
        app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }))
            .WithName("HealthCheck")
            .WithTags("Health");

        // Map feature endpoints
        app.MapLocationEndpoints();
        app.MapAttractionEndpoints();
        app.MapTripEndpoints();

        return app;
    }
}
