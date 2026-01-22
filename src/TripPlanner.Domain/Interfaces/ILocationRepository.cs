using TripPlanner.Domain.Entities;

namespace TripPlanner.Domain.Interfaces;

/// <summary>
/// Repository interface for Location entity with specific query methods.
/// </summary>
public interface ILocationRepository : IRepository<Location>
{
    Task<IEnumerable<Location>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<Location?> GetByNameAndCountryAsync(string name, string country, CancellationToken cancellationToken = default);
}
