namespace TripPlanner.WebApi.Contracts;

/// <summary>
/// API response wrapper for paginated data.
/// </summary>
/// <typeparam name="T">The type of items in the response.</typeparam>
/// <param name="Items">The items for the current page.</param>
/// <param name="Pagination">The pagination metadata.</param>
public record PaginatedResponse<T>(
    IReadOnlyList<T> Items,
    PaginationMetadata Pagination
);

/// <summary>
/// Pagination metadata included in paginated API responses.
/// </summary>
/// <param name="Page">The current page number (1-based).</param>
/// <param name="PageSize">The number of items per page.</param>
/// <param name="TotalItems">The total number of items across all pages.</param>
/// <param name="TotalPages">The total number of pages.</param>
/// <param name="HasNextPage">Whether there is a next page available.</param>
/// <param name="HasPreviousPage">Whether there is a previous page available.</param>
public record PaginationMetadata(
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages,
    bool HasNextPage,
    bool HasPreviousPage
);
