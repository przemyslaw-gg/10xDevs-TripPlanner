using TripPlanner.Application;
using TripPlanner.Infrastructure;

namespace TripPlanner.WebApi.Extensions;

/// <summary>
/// Extension methods for configuring services in the WebApi layer.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add Application layer
        services.AddApplication();

        // Add Infrastructure layer
        services.AddInfrastructure(configuration);

        // Add API-specific services
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        // Add CORS
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        return services;
    }
}
