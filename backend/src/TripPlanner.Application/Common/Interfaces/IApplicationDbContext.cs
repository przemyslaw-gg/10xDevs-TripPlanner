using Microsoft.EntityFrameworkCore;
using TripPlanner.Domain.Entities;

namespace TripPlanner.Application.Common.Interfaces;

/// <summary>
/// Database context interface for the application.
/// </summary>
public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<Location> Locations { get; }
    DbSet<Attraction> Attractions { get; }
    DbSet<Trip> Trips { get; }
    DbSet<TripAttraction> TripAttractions { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
