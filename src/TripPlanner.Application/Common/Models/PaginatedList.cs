using Microsoft.EntityFrameworkCore;

namespace TripPlanner.Application.Common.Models;

/// <summary>
/// Represents a paginated list of items with metadata for pagination navigation.
/// </summary>
/// <typeparam name="T">The type of items in the list.</typeparam>
public class PaginatedList<T>
{
    /// <summary>
    /// Gets the items for the current page.
    /// </summary>
    public IReadOnlyList<T> Items { get; }

    /// <summary>
    /// Gets the current page number (1-based).
    /// </summary>
    public int Page { get; }

    /// <summary>
    /// Gets the number of items per page.
    /// </summary>
    public int PageSize { get; }

    /// <summary>
    /// Gets the total number of items across all pages.
    /// </summary>
    public int TotalItems { get; }

    /// <summary>
    /// Gets the total number of pages.
    /// </summary>
    public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);

    /// <summary>
    /// Gets a value indicating whether there is a next page.
    /// </summary>
    public bool HasNextPage => Page < TotalPages;

    /// <summary>
    /// Gets a value indicating whether there is a previous page.
    /// </summary>
    public bool HasPreviousPage => Page > 1;

    private PaginatedList(IReadOnlyList<T> items, int totalItems, int page, int pageSize)
    {
        Items = items;
        TotalItems = totalItems;
        Page = page;
        PageSize = pageSize;
    }

    /// <summary>
    /// Creates a new paginated list asynchronously from an IQueryable source.
    /// </summary>
    /// <param name="source">The queryable source to paginate.</param>
    /// <param name="page">The page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A paginated list containing the items for the specified page.</returns>
    public static async Task<PaginatedList<T>> CreateAsync(
        IQueryable<T> source,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var totalItems = await source.CountAsync(cancellationToken);
        var items = await source
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<T>(items, totalItems, page, pageSize);
    }

    /// <summary>
    /// Creates a new paginated list from an in-memory collection.
    /// Useful for testing or when data is already loaded.
    /// </summary>
    /// <param name="items">The items for the current page.</param>
    /// <param name="totalItems">The total number of items across all pages.</param>
    /// <param name="page">The page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A paginated list containing the specified items.</returns>
    public static PaginatedList<T> Create(
        IReadOnlyList<T> items,
        int totalItems,
        int page,
        int pageSize)
    {
        return new PaginatedList<T>(items, totalItems, page, pageSize);
    }
}
