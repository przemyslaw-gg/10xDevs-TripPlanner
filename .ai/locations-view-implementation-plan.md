# API Endpoint Implementation Plan: Locations

## 1. Endpoint Overview

This plan covers the implementation of two Location endpoints:

| Endpoint | Method | Purpose |
|----------|--------|---------|
| `/api/locations` | GET | List all locations with optional search and pagination |
| `/api/locations/{id}` | GET | Get a specific location by ID |

Both endpoints are **publicly accessible** (no authentication required) as locations are reference data for the trip planning system.

---

## 2. Request Details

### GET /api/locations

- **HTTP Method:** GET
- **URL Pattern:** `/api/locations`
- **Authentication:** None (public endpoint)

**Query Parameters:**

| Parameter | Type | Required | Default | Validation |
|-----------|------|----------|---------|------------|
| `search` | string | No | null | Max 100 characters |
| `page` | int | No | 1 | Must be >= 1 |
| `pageSize` | int | No | 20 | Must be 1-100 |

### GET /api/locations/{id}

- **HTTP Method:** GET
- **URL Pattern:** `/api/locations/{id:guid}`
- **Authentication:** None (public endpoint)

**Path Parameters:**

| Parameter | Type | Required | Validation |
|-----------|------|----------|------------|
| `id` | Guid | Yes | Valid UUID format |

---

## 3. Types Used

### DTOs (Response Models)

```csharp
// TripPlanner.Application/Locations/DTOs/LocationListItemDto.cs
public record LocationListItemDto(
    Guid Id,
    string Name,
    string Country,
    string? Timezone
);

// TripPlanner.Application/Locations/DTOs/LocationDto.cs
public record LocationDto(
    Guid Id,
    string Name,
    string Country,
    string? Timezone,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

// TripPlanner.Application/Common/Models/PaginatedList.cs
public class PaginatedList<T>
{
    public IReadOnlyList<T> Items { get; }
    public int Page { get; }
    public int PageSize { get; }
    public int TotalItems { get; }
    public int TotalPages { get; }
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}
```

### Query Models (MediatR Requests)

```csharp
// TripPlanner.Application/Locations/Queries/GetLocations/GetLocationsQuery.cs
public record GetLocationsQuery(
    string? Search,
    int Page = 1,
    int PageSize = 20
) : IRequest<PaginatedList<LocationListItemDto>>;

// TripPlanner.Application/Locations/Queries/GetLocationById/GetLocationByIdQuery.cs
public record GetLocationByIdQuery(Guid Id) : IRequest<LocationDto?>;
```

---

## 4. Response Details

### GET /api/locations

**Success Response (200 OK):**

```json
{
  "items": [
    {
      "id": "uuid",
      "name": "Athens",
      "country": "Greece",
      "timezone": "Europe/Athens"
    }
  ],
  "pagination": {
    "page": 1,
    "pageSize": 20,
    "totalItems": 150,
    "totalPages": 8,
    "hasNextPage": true,
    "hasPreviousPage": false
  }
}
```

**Error Response (400 Bad Request):**

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Validation Failed",
  "status": 400,
  "errors": {
    "page": ["Page must be greater than or equal to 1"],
    "pageSize": ["PageSize must be between 1 and 100"]
  }
}
```

### GET /api/locations/{id}

**Success Response (200 OK):**

```json
{
  "id": "uuid",
  "name": "Athens",
  "country": "Greece",
  "timezone": "Europe/Athens",
  "createdAt": "2026-01-01T00:00:00Z",
  "updatedAt": "2026-01-01T00:00:00Z"
}
```

**Error Response (404 Not Found):**

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Not Found",
  "status": 404,
  "detail": "Location with ID 'uuid' was not found"
}
```

---

## 5. Data Flow

### GET /api/locations Flow

```
┌─────────────┐     ┌──────────────┐     ┌─────────────────┐     ┌───────────────┐
│   Client    │────▶│  Endpoint    │────▶│  MediatR        │────▶│  Query        │
│             │     │  (WebApi)    │     │  (Validation)   │     │  Handler      │
└─────────────┘     └──────────────┘     └─────────────────┘     └───────────────┘
                                                                         │
                                                                         ▼
┌─────────────┐     ┌──────────────┐     ┌─────────────────┐     ┌───────────────┐
│   Client    │◀────│  Endpoint    │◀────│  DTO Mapping    │◀────│  DbContext    │
│             │     │  (Response)  │     │                 │     │  (EF Core)    │
└─────────────┘     └──────────────┘     └─────────────────┘     └───────────────┘
```

1. Client sends GET request with optional query parameters
2. Minimal API endpoint binds parameters and creates `GetLocationsQuery`
3. MediatR dispatches to `ValidationBehavior` (FluentValidation)
4. If validation passes, `GetLocationsQueryHandler` executes
5. Handler queries `IApplicationDbContext.Locations` with EF Core
6. Results mapped to `LocationListItemDto` and wrapped in `PaginatedList<T>`
7. Endpoint returns 200 OK with JSON response

### GET /api/locations/{id} Flow

```
┌─────────────┐     ┌──────────────┐     ┌─────────────────┐     ┌───────────────┐
│   Client    │────▶│  Endpoint    │────▶│  MediatR        │────▶│  Query        │
│             │     │  (WebApi)    │     │  (Dispatch)     │     │  Handler      │
└─────────────┘     └──────────────┘     └─────────────────┘     └───────────────┘
                                                                         │
                                                                         ▼
┌─────────────┐     ┌──────────────┐                             ┌───────────────┐
│   Client    │◀────│  Endpoint    │◀────────────────────────────│  DbContext    │
│             │     │  (200/404)   │                             │  (EF Core)    │
└─────────────┘     └──────────────┘                             └───────────────┘
```

1. Client sends GET request with location ID in path
2. Minimal API endpoint extracts `Guid id` and creates `GetLocationByIdQuery`
3. MediatR dispatches to `GetLocationByIdQueryHandler`
4. Handler queries `IApplicationDbContext.Locations.FindAsync(id)`
5. If found: map to `LocationDto` and return 200 OK
6. If not found: return 404 Not Found

---

## 6. Security Considerations

### Authentication & Authorization

- **No authentication required** - Locations are public reference data
- Matches RLS policy: `"locations_select_all" USING (true)`

### Input Validation

| Parameter | Validation | Reason |
|-----------|------------|--------|
| `search` | Max 100 chars, trimmed, sanitized | Prevent oversized queries |
| `page` | >= 1 | Prevent invalid pagination |
| `pageSize` | 1-100 | Prevent DoS via large result sets |
| `id` | Valid GUID format | Ensure proper DB query |

### SQL Injection Prevention

- EF Core parameterized queries handle all user input
- No raw SQL or string concatenation

### Rate Limiting (Future Enhancement)

- Consider implementing rate limiting for production
- Suggested: 100 requests per minute per IP for public endpoints

---

## 7. Error Handling

| Scenario | HTTP Status | Error Type | Details |
|----------|-------------|------------|---------|
| Invalid `page` parameter | 400 | ValidationException | "Page must be >= 1" |
| Invalid `pageSize` parameter | 400 | ValidationException | "PageSize must be between 1 and 100" |
| Invalid `id` format | 400 | BadRequest | "Invalid UUID format" |
| Location not found | 404 | NotFound | "Location with ID '{id}' was not found" |
| Database connection error | 500 | InternalServerError | Logged, generic message to client |
| Unexpected exception | 500 | InternalServerError | Logged, generic message to client |

### Global Exception Handling

Create a middleware to handle exceptions consistently:

```csharp
// Catch FluentValidation.ValidationException -> 400
// Catch NotFoundException -> 404
// Catch all others -> 500 with logging
```

---

## 8. Performance Considerations

### Database Optimization

- **Existing Index:** `idx_locations_name_country ON locations(name, country)` supports search queries
- **Pagination:** Server-side pagination prevents loading entire dataset
- **Projection:** Select only required columns, not full entity

### Query Optimization

```csharp
// Use AsNoTracking for read-only queries
await _context.Locations
    .AsNoTracking()
    .Where(l => search == null ||
           l.Name.Contains(search) ||
           l.Country.Contains(search))
    .OrderBy(l => l.Name)
    .Skip((page - 1) * pageSize)
    .Take(pageSize)
    .Select(l => new LocationListItemDto(...))
    .ToListAsync();
```

### Caching (Future Enhancement)

- Locations rarely change - good candidate for caching
- Consider: In-memory cache with 5-minute TTL
- Cache invalidation on location updates

---

## 9. Implementation Steps

### Step 1: Create Common Models

**File:** `src/TripPlanner.Application/Common/Models/PaginatedList.cs`

```csharp
namespace TripPlanner.Application.Common.Models;

public class PaginatedList<T>
{
    public IReadOnlyList<T> Items { get; }
    public int Page { get; }
    public int PageSize { get; }
    public int TotalItems { get; }
    public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;

    private PaginatedList(IReadOnlyList<T> items, int totalItems, int page, int pageSize)
    {
        Items = items;
        TotalItems = totalItems;
        Page = page;
        PageSize = pageSize;
    }

    public static async Task<PaginatedList<T>> CreateAsync(
        IQueryable<T> source, int page, int pageSize, CancellationToken ct = default)
    {
        var totalItems = await source.CountAsync(ct);
        var items = await source
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PaginatedList<T>(items, totalItems, page, pageSize);
    }
}
```

### Step 2: Create Location DTOs

**File:** `src/TripPlanner.Application/Locations/DTOs/LocationListItemDto.cs`

```csharp
namespace TripPlanner.Application.Locations.DTOs;

public record LocationListItemDto(
    Guid Id,
    string Name,
    string Country,
    string? Timezone
);
```

**File:** `src/TripPlanner.Application/Locations/DTOs/LocationDto.cs`

```csharp
namespace TripPlanner.Application.Locations.DTOs;

public record LocationDto(
    Guid Id,
    string Name,
    string Country,
    string? Timezone,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
```

### Step 3: Create GetLocations Query

**File:** `src/TripPlanner.Application/Locations/Queries/GetLocations/GetLocationsQuery.cs`

```csharp
using MediatR;
using TripPlanner.Application.Common.Models;
using TripPlanner.Application.Locations.DTOs;

namespace TripPlanner.Application.Locations.Queries.GetLocations;

public record GetLocationsQuery(
    string? Search = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<PaginatedList<LocationListItemDto>>;
```

**File:** `src/TripPlanner.Application/Locations/Queries/GetLocations/GetLocationsQueryValidator.cs`

```csharp
using FluentValidation;

namespace TripPlanner.Application.Locations.Queries.GetLocations;

public class GetLocationsQueryValidator : AbstractValidator<GetLocationsQuery>
{
    public GetLocationsQueryValidator()
    {
        RuleFor(x => x.Search)
            .MaximumLength(100)
            .WithMessage("Search term must not exceed 100 characters");

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page must be greater than or equal to 1");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("PageSize must be between 1 and 100");
    }
}
```

**File:** `src/TripPlanner.Application/Locations/Queries/GetLocations/GetLocationsQueryHandler.cs`

```csharp
using MediatR;
using Microsoft.EntityFrameworkCore;
using TripPlanner.Application.Common.Interfaces;
using TripPlanner.Application.Common.Models;
using TripPlanner.Application.Locations.DTOs;

namespace TripPlanner.Application.Locations.Queries.GetLocations;

public class GetLocationsQueryHandler
    : IRequestHandler<GetLocationsQuery, PaginatedList<LocationListItemDto>>
{
    private readonly IApplicationDbContext _context;

    public GetLocationsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<LocationListItemDto>> Handle(
        GetLocationsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Locations.AsNoTracking();

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchTerm = request.Search.Trim().ToLower();
            query = query.Where(l =>
                l.Name.ToLower().Contains(searchTerm) ||
                l.Country.ToLower().Contains(searchTerm));
        }

        // Order by name for consistent results
        query = query.OrderBy(l => l.Name);

        // Project to DTO
        var dtoQuery = query.Select(l => new LocationListItemDto(
            l.Id,
            l.Name,
            l.Country,
            l.Timezone
        ));

        return await PaginatedList<LocationListItemDto>.CreateAsync(
            dtoQuery, request.Page, request.PageSize, cancellationToken);
    }
}
```

### Step 4: Create GetLocationById Query

**File:** `src/TripPlanner.Application/Locations/Queries/GetLocationById/GetLocationByIdQuery.cs`

```csharp
using MediatR;
using TripPlanner.Application.Locations.DTOs;

namespace TripPlanner.Application.Locations.Queries.GetLocationById;

public record GetLocationByIdQuery(Guid Id) : IRequest<LocationDto?>;
```

**File:** `src/TripPlanner.Application/Locations/Queries/GetLocationById/GetLocationByIdQueryHandler.cs`

```csharp
using MediatR;
using Microsoft.EntityFrameworkCore;
using TripPlanner.Application.Common.Interfaces;
using TripPlanner.Application.Locations.DTOs;

namespace TripPlanner.Application.Locations.Queries.GetLocationById;

public class GetLocationByIdQueryHandler
    : IRequestHandler<GetLocationByIdQuery, LocationDto?>
{
    private readonly IApplicationDbContext _context;

    public GetLocationByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<LocationDto?> Handle(
        GetLocationByIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.Locations
            .AsNoTracking()
            .Where(l => l.Id == request.Id)
            .Select(l => new LocationDto(
                l.Id,
                l.Name,
                l.Country,
                l.Timezone,
                l.CreatedAt,
                l.UpdatedAt
            ))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
```

### Step 5: Create API Response Models

**File:** `src/TripPlanner.WebApi/Contracts/PaginatedResponse.cs`

```csharp
namespace TripPlanner.WebApi.Contracts;

public record PaginatedResponse<T>(
    IReadOnlyList<T> Items,
    PaginationMetadata Pagination
);

public record PaginationMetadata(
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages,
    bool HasNextPage,
    bool HasPreviousPage
);
```

### Step 6: Update Location Endpoints

**File:** `src/TripPlanner.WebApi/Endpoints/LocationEndpoints.cs`

```csharp
using MediatR;
using TripPlanner.Application.Locations.DTOs;
using TripPlanner.Application.Locations.Queries.GetLocations;
using TripPlanner.Application.Locations.Queries.GetLocationById;
using TripPlanner.WebApi.Contracts;

namespace TripPlanner.WebApi.Endpoints;

public static class LocationEndpoints
{
    public static WebApplication MapLocationEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/locations")
            .WithTags("Locations")
            .WithOpenApi();

        group.MapGet("/", GetLocations)
            .WithName("GetLocations")
            .WithDescription("List all locations with optional search and pagination")
            .Produces<PaginatedResponse<LocationListItemDto>>(StatusCodes.Status200OK)
            .ProducesValidationProblem();

        group.MapGet("/{id:guid}", GetLocationById)
            .WithName("GetLocationById")
            .WithDescription("Get a specific location by ID")
            .Produces<LocationDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }

    private static async Task<IResult> GetLocations(
        ISender mediator,
        string? search = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetLocationsQuery(search, page, pageSize);
        var result = await mediator.Send(query, cancellationToken);

        var response = new PaginatedResponse<LocationListItemDto>(
            result.Items,
            new PaginationMetadata(
                result.Page,
                result.PageSize,
                result.TotalItems,
                result.TotalPages,
                result.HasNextPage,
                result.HasPreviousPage
            )
        );

        return Results.Ok(response);
    }

    private static async Task<IResult> GetLocationById(
        Guid id,
        ISender mediator,
        CancellationToken cancellationToken = default)
    {
        var query = new GetLocationByIdQuery(id);
        var result = await mediator.Send(query, cancellationToken);

        return result is not null
            ? Results.Ok(result)
            : Results.Problem(
                title: "Not Found",
                detail: $"Location with ID '{id}' was not found",
                statusCode: StatusCodes.Status404NotFound);
    }
}
```

### Step 7: Add Global Exception Handler

**File:** `src/TripPlanner.WebApi/Middleware/ExceptionHandlingMiddleware.cs`

```csharp
using FluentValidation;
using System.Text.Json;

namespace TripPlanner.WebApi.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            await HandleValidationException(context, ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred");
            await HandleGenericException(context);
        }
    }

    private static async Task HandleValidationException(
        HttpContext context, ValidationException exception)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.ContentType = "application/problem+json";

        var errors = exception.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray()
            );

        var problemDetails = new
        {
            type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            title = "Validation Failed",
            status = 400,
            errors
        };

        await context.Response.WriteAsJsonAsync(problemDetails);
    }

    private static async Task HandleGenericException(HttpContext context)
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";

        var problemDetails = new
        {
            type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            title = "Internal Server Error",
            status = 500,
            detail = "An unexpected error occurred. Please try again later."
        };

        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}
```

### Step 8: Register Middleware

**Update:** `src/TripPlanner.WebApi/Extensions/WebApplicationExtensions.cs`

```csharp
public static WebApplication ConfigurePipeline(this WebApplication app)
{
    // Add exception handling middleware first
    app.UseMiddleware<ExceptionHandlingMiddleware>();

    // ... rest of configuration
}
```

### Step 9: Delete Old Endpoint File

Remove the old placeholder file:
- Delete `src/TripPlanner.WebApi/Endpoints/LocationEndpointsOld.cs`

### Step 10: Write Unit Tests

**File:** `tests/TripPlanner.Application.UnitTests/Locations/Queries/GetLocationsQueryValidatorTests.cs`

```csharp
using FluentValidation.TestHelper;
using TripPlanner.Application.Locations.Queries.GetLocations;

namespace TripPlanner.Application.UnitTests.Locations.Queries;

[TestClass]
[TestCategory("Unit")]
public class GetLocationsQueryValidatorTests
{
    private GetLocationsQueryValidator _validator = null!;

    [TestInitialize]
    public void Setup()
    {
        _validator = new GetLocationsQueryValidator();
    }

    [TestMethod]
    public void Should_Pass_With_Default_Values()
    {
        var query = new GetLocationsQuery();
        var result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [TestMethod]
    public void Should_Fail_When_Page_Is_Zero()
    {
        var query = new GetLocationsQuery(Page: 0);
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.Page);
    }

    [TestMethod]
    public void Should_Fail_When_PageSize_Exceeds_100()
    {
        var query = new GetLocationsQuery(PageSize: 101);
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }

    [TestMethod]
    public void Should_Fail_When_Search_Exceeds_100_Characters()
    {
        var query = new GetLocationsQuery(Search: new string('a', 101));
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.Search);
    }
}
```

### Step 11: Write Integration Tests

**File:** `tests/TripPlanner.WebApi.IntegrationTests/Endpoints/LocationEndpointsTests.cs`

```csharp
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TripPlanner.Application.Locations.DTOs;
using TripPlanner.WebApi.Contracts;

namespace TripPlanner.WebApi.IntegrationTests.Endpoints;

[TestClass]
[TestCategory("Integration")]
public class LocationEndpointsTests
{
    private static WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    [ClassInitialize]
    public static void ClassSetup(TestContext context)
    {
        _factory = new WebApplicationFactory<Program>();
    }

    [TestInitialize]
    public void Setup()
    {
        _client = _factory.CreateClient();
    }

    [TestMethod]
    public async Task GetLocations_ReturnsOk_WithPaginatedResponse()
    {
        // Act
        var response = await _client.GetAsync("/api/locations");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content
            .ReadFromJsonAsync<PaginatedResponse<LocationListItemDto>>();
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Items);
        Assert.IsNotNull(result.Pagination);
    }

    [TestMethod]
    public async Task GetLocations_WithInvalidPage_ReturnsBadRequest()
    {
        // Act
        var response = await _client.GetAsync("/api/locations?page=0");

        // Assert
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [TestMethod]
    public async Task GetLocationById_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/locations/{nonExistentId}");

        // Assert
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        _factory?.Dispose();
    }
}
```

---

## 10. File Structure Summary

```
src/
├── TripPlanner.Application/
│   ├── Common/
│   │   └── Models/
│   │       └── PaginatedList.cs
│   └── Locations/
│       ├── DTOs/
│       │   ├── LocationListItemDto.cs
│       │   └── LocationDto.cs
│       └── Queries/
│           ├── GetLocations/
│           │   ├── GetLocationsQuery.cs
│           │   ├── GetLocationsQueryValidator.cs
│           │   └── GetLocationsQueryHandler.cs
│           └── GetLocationById/
│               ├── GetLocationByIdQuery.cs
│               └── GetLocationByIdQueryHandler.cs
└── TripPlanner.WebApi/
    ├── Contracts/
    │   └── PaginatedResponse.cs
    ├── Endpoints/
    │   └── LocationEndpoints.cs
    └── Middleware/
        └── ExceptionHandlingMiddleware.cs

tests/
├── TripPlanner.Application.UnitTests/
│   └── Locations/
│       └── Queries/
│           └── GetLocationsQueryValidatorTests.cs
└── TripPlanner.WebApi.IntegrationTests/
    └── Endpoints/
        └── LocationEndpointsTests.cs
```

---

## 11. Checklist

- [ ] Create `PaginatedList<T>` common model
- [ ] Create `LocationListItemDto` and `LocationDto` records
- [ ] Create `GetLocationsQuery` with validator and handler
- [ ] Create `GetLocationByIdQuery` with handler
- [ ] Create `PaginatedResponse<T>` API contract
- [ ] Implement `LocationEndpoints` with Minimal API
- [ ] Create `ExceptionHandlingMiddleware`
- [ ] Register middleware in pipeline
- [ ] Delete old `LocationEndpointsOld.cs`
- [ ] Write unit tests for validator
- [ ] Write integration tests for endpoints
- [ ] Test with Swagger UI
- [ ] Verify database index usage with EXPLAIN ANALYZE
