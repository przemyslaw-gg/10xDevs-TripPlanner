using TripPlanner.Domain.Entities;

namespace TripPlanner.Domain.Interfaces;

/// <summary>
/// Repository interface for Attraction entity with specific query methods.
/// </summary>
public interface IAttractionRepository : IRepository<Attraction>
{
    Task<IEnumerable<Attraction>> GetByLocationIdAsync(Guid locationId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Attraction>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> IsUsedInOtherUsersTripsAsync(Guid attractionId, Guid userId, CancellationToken cancellationToken = default);
}
