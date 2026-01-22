using TripPlanner.Domain.Entities;

namespace TripPlanner.Domain.Interfaces;

/// <summary>
/// Repository interface for Trip entity with specific query methods.
/// </summary>
public interface ITripRepository : IRepository<Trip>
{
    Task<IEnumerable<Trip>> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Trip>> GetPublicTripsAsync(CancellationToken cancellationToken = default);
    Task<Trip?> GetWithAttractionsAsync(Guid id, CancellationToken cancellationToken = default);
}
