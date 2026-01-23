using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TripPlanner.Application.Common.Interfaces;
using TripPlanner.Infrastructure.Persistence;

namespace TripPlanner.WebApi.IntegrationTests;

/// <summary>
/// Custom WebApplicationFactory for integration testing.
/// Configures the application to use an in-memory database.
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = $"TestDb_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove ALL EF Core related service descriptors
            var descriptorsToRemove = services
                .Where(d =>
                    d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>) ||
                    d.ServiceType == typeof(DbContextOptions) ||
                    d.ServiceType == typeof(ApplicationDbContext) ||
                    d.ServiceType == typeof(IApplicationDbContext) ||
                    d.ServiceType.FullName?.Contains("EntityFrameworkCore") == true ||
                    d.ServiceType.FullName?.Contains("Npgsql") == true ||
                    d.ImplementationType?.FullName?.Contains("Npgsql") == true ||
                    d.ImplementationType?.FullName?.Contains("EntityFrameworkCore") == true)
                .ToList();

            foreach (var descriptor in descriptorsToRemove)
            {
                services.Remove(descriptor);
            }

            // Build a separate service provider for EF Core with InMemory only
            var efServiceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            // Add DbContext using an in-memory database with its own internal service provider
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase(_dbName);
                options.UseInternalServiceProvider(efServiceProvider);
            });

            // Re-register the interface
            services.AddScoped<IApplicationDbContext>(provider =>
                provider.GetRequiredService<ApplicationDbContext>());
        });

        builder.UseEnvironment("Testing");
    }

    /// <summary>
    /// Seeds the test data. Call this once before running tests.
    /// </summary>
    public void SeedTestData()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Ensure database is created
        db.Database.EnsureCreated();

        // Only seed if the database is empty
        if (db.Locations.Any())
        {
            return;
        }

        db.Locations.AddRange(
            new Domain.Entities.Location
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Athens",
                Country = "Greece",
                Timezone = "Europe/Athens",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Domain.Entities.Location
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "Rome",
                Country = "Italy",
                Timezone = "Europe/Rome",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Domain.Entities.Location
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "Paris",
                Country = "France",
                Timezone = "Europe/Paris",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        );

        db.SaveChanges();
    }
}
