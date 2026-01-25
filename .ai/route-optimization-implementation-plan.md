# API Endpoint Implementation Plan: Optimize Route

## 1. Przegląd punktu końcowego

Endpoint `POST /api/trips/{tripId}/optimize-route` służy do obliczenia i zastosowania optymalnej kolejności odwiedzania atrakcji w ramach wycieczki. Wykorzystuje algorytm najbliższego sąsiada (Nearest Neighbor) do minimalizacji całkowitej odległości podróży między atrakcjami.

**Główne funkcjonalności:**
- Przyjmuje punkt startowy (startingAttractionId) jako początek optymalizacji
- Oblicza optymalną kolejność odwiedzania wszystkich atrakcji w tripie
- Aktualizuje pole `order_index` w tabeli `trip_attractions`
- Zwraca zoptymalizowaną kolejność wraz z całkowitą odległością trasy

## 2. Szczegóły żądania

### Metoda HTTP
`POST`

### Struktura URL
```
/api/trips/{tripId}/optimize-route
```

### Parametry

**Path Parameters:**
| Parametr | Typ | Wymagany | Opis |
|----------|-----|----------|------|
| `tripId` | UUID | Tak | Identyfikator wycieczki |

**Request Body:**
```json
{
  "startingAttractionId": "uuid"
}
```

| Pole | Typ | Wymagany | Opis |
|------|-----|----------|------|
| `startingAttractionId` | UUID | Tak | ID atrakcji, od której rozpoczyna się optymalizacja trasy |

### Headers
| Header | Wartość | Wymagany |
|--------|---------|----------|
| `Authorization` | `Bearer {token}` | Tak |
| `Content-Type` | `application/json` | Tak |

## 3. Wykorzystywane typy

### Request DTO

```csharp
// TripPlanner.WebApi/Endpoints/RouteOptimizationEndpoints.cs
public record OptimizeRouteRequest(
    Guid StartingAttractionId
);
```

### Command Model

```csharp
// TripPlanner.Application/RouteOptimization/Commands/OptimizeRoute/OptimizeRouteCommand.cs
public record OptimizeRouteCommand(
    Guid TripId,
    Guid StartingAttractionId
) : IRequest<OptimizeRouteResponseDto>;
```

### Command Validator

```csharp
// TripPlanner.Application/RouteOptimization/Commands/OptimizeRoute/OptimizeRouteCommandValidator.cs
public class OptimizeRouteCommandValidator : AbstractValidator<OptimizeRouteCommand>
{
    public OptimizeRouteCommandValidator()
    {
        RuleFor(x => x.TripId)
            .NotEmpty()
            .WithMessage("Trip ID is required.");

        RuleFor(x => x.StartingAttractionId)
            .NotEmpty()
            .WithMessage("Starting attraction ID is required.");
    }
}
```

### Response DTOs

```csharp
// TripPlanner.Application/RouteOptimization/DTOs/OptimizeRouteResponseDto.cs
public record OptimizeRouteResponseDto(
    Guid TripId,
    IReadOnlyList<OptimizedAttractionItemDto> OptimizedOrder,
    decimal TotalDistance,
    DateTime OptimizedAt
);

// TripPlanner.Application/RouteOptimization/DTOs/OptimizedAttractionItemDto.cs
public record OptimizedAttractionItemDto(
    Guid AttractionId,
    string AttractionName,
    int DayNumber,
    int OrderIndex
);
```

### Domain Service Interface

```csharp
// TripPlanner.Application/Common/Interfaces/IRouteOptimizationService.cs
public interface IRouteOptimizationService
{
    RouteOptimizationResult OptimizeRoute(
        IReadOnlyList<AttractionWithCoordinates> attractions,
        Guid startingAttractionId
    );
}

public record AttractionWithCoordinates(
    Guid Id,
    string Name,
    decimal Latitude,
    decimal Longitude,
    int CurrentDayNumber
);

public record RouteOptimizationResult(
    IReadOnlyList<OptimizedAttraction> OptimizedAttractions,
    decimal TotalDistanceMeters
);

public record OptimizedAttraction(
    Guid AttractionId,
    string AttractionName,
    int OrderIndex
);
```

## 4. Szczegóły odpowiedzi

### Success Response (200 OK)

```json
{
  "tripId": "550e8400-e29b-41d4-a716-446655440000",
  "optimizedOrder": [
    {
      "attractionId": "550e8400-e29b-41d4-a716-446655440001",
      "attractionName": "Acropolis of Athens",
      "dayNumber": 1,
      "orderIndex": 1
    },
    {
      "attractionId": "550e8400-e29b-41d4-a716-446655440002",
      "attractionName": "Parthenon",
      "dayNumber": 1,
      "orderIndex": 2
    }
  ],
  "totalDistance": 12500.5,
  "optimizedAt": "2026-01-22T11:00:00Z"
}
```

### Error Responses

| Status Code | Scenariusz | Response Body |
|-------------|------------|---------------|
| 400 | Nieprawidłowe dane wejściowe lub startingAttractionId nie jest w tripie | Problem Details |
| 401 | Brak lub nieprawidłowy token autoryzacji | Problem Details |
| 403 | Użytkownik nie jest właścicielem tripu | Problem Details |
| 404 | Trip nie znaleziony | Problem Details |
| 422 | Trip nie ma atrakcji do optymalizacji | Problem Details |

## 5. Przepływ danych

```
┌─────────────┐     ┌──────────────────┐     ┌─────────────────────┐
│   Client    │────>│  RouteOptimize   │────>│  OptimizeRoute      │
│  (Request)  │     │  Endpoint        │     │  CommandHandler     │
└─────────────┘     └──────────────────┘     └─────────────────────┘
                                                       │
                           ┌───────────────────────────┤
                           │                           │
                           ▼                           ▼
                    ┌─────────────────┐    ┌────────────────────────┐
                    │ ICurrentUser    │    │ IApplicationDbContext  │
                    │ Service         │    │ (EF Core)              │
                    └─────────────────┘    └────────────────────────┘
                                                       │
                                                       ▼
                                           ┌────────────────────────┐
                                           │ IRouteOptimization     │
                                           │ Service                │
                                           │ (Nearest Neighbor)     │
                                           └────────────────────────┘
                                                       │
                                                       ▼
                                           ┌────────────────────────┐
                                           │ Update trip_attractions│
                                           │ (order_index)          │
                                           └────────────────────────┘
```

### Szczegółowy przepływ:

1. **Endpoint** odbiera żądanie POST z `tripId` i `startingAttractionId`
2. **Walidacja** - FluentValidation sprawdza poprawność danych wejściowych
3. **Autoryzacja** - sprawdzenie czy użytkownik jest właścicielem tripu
4. **Pobranie danych** - załadowanie tripu z atrakcjami i ich współrzędnymi
5. **Walidacja biznesowa**:
   - Sprawdzenie czy trip istnieje (404)
   - Sprawdzenie czy użytkownik jest właścicielem (403)
   - Sprawdzenie czy startingAttractionId jest w tripie (400)
   - Sprawdzenie czy trip ma atrakcje (422)
6. **Optymalizacja** - wywołanie `IRouteOptimizationService.OptimizeRoute()`
7. **Aktualizacja** - update `order_index` dla wszystkich `trip_attractions`
8. **Odpowiedź** - zwrócenie zoptymalizowanej kolejności

## 6. Względy bezpieczeństwa

### Uwierzytelnianie
- Endpoint wymaga ważnego tokenu JWT w nagłówku `Authorization`
- Token musi zawierać claim `sub` z ID użytkownika
- Konfiguracja: `.RequireAuthorization()` na poziomie endpointu

### Autoryzacja
- Tylko właściciel tripu (trip.OwnerId == currentUserId) może optymalizować trasę
- Weryfikacja wykonywana w Command Handler przed jakąkolwiek operacją

### Walidacja danych wejściowych
- Walidacja UUID formatów (tripId, startingAttractionId)
- Ochrona przed SQL Injection przez użycie EF Core z parametryzowanymi zapytaniami
- Walidacja istnienia zasobów w bazie danych

### Ograniczenia
- Rozważyć limit maksymalnej liczby atrakcji do optymalizacji (np. 100) aby zapobiec DoS
- Rate limiting na poziomie auth endpoints (już skonfigurowany)

## 7. Obsługa błędów

### Mapowanie wyjątków na kody HTTP

| Wyjątek | HTTP Status | Opis |
|---------|-------------|------|
| `ValidationException` | 400 | Błąd walidacji FluentValidation |
| `InvalidOperationException` (starting attraction not in trip) | 400 | Atrakcja startowa nie należy do tripu |
| `UnauthorizedAccessException` | 401 | Brak uwierzytelnienia |
| `ForbiddenAccessException` | 403 | Brak uprawnień do tripu |
| `NotFoundException` | 404 | Trip nie znaleziony |
| `UnprocessableEntityException` | 422 | Brak atrakcji do optymalizacji |

### Nowy wyjątek dla 422

```csharp
// TripPlanner.Application/Common/Exceptions/UnprocessableEntityException.cs
public class UnprocessableEntityException : Exception
{
    public UnprocessableEntityException(string message) : base(message) { }
}
```

### Aktualizacja ExceptionHandlingMiddleware

Dodać obsługę `UnprocessableEntityException`:

```csharp
UnprocessableEntityException => (StatusCodes.Status422UnprocessableEntity, "Unprocessable Entity")
```

## 8. Rozważania dotyczące wydajności

### Optymalizacje zapytań
- Eager loading atrakcji z ich współrzędnymi w jednym zapytaniu
- Użycie projekcji (Select) zamiast ładowania pełnych encji
- Batch update dla `trip_attractions` zamiast pojedynczych update'ów

### Złożoność algorytmu
- Algorytm Nearest Neighbor: O(n²) gdzie n = liczba atrakcji
- Dla typowych przypadków (10-50 atrakcji) jest akceptowalny
- Dla większej liczby atrakcji rozważyć:
  - Limit maksymalnej liczby atrakcji
  - Asynchroniczne przetwarzanie z powiadomieniem

### Caching
- Nie ma potrzeby cachowania - wynik zależy od aktualnego stanu tripu
- Współrzędne atrakcji mogłyby być cachowane (rzadko się zmieniają)

## 9. Etapy wdrożenia

### Krok 1: Utworzenie struktury katalogów

```
src/TripPlanner.Application/
├── RouteOptimization/
│   ├── Commands/
│   │   └── OptimizeRoute/
│   │       ├── OptimizeRouteCommand.cs
│   │       ├── OptimizeRouteCommandHandler.cs
│   │       └── OptimizeRouteCommandValidator.cs
│   └── DTOs/
│       ├── OptimizeRouteResponseDto.cs
│       └── OptimizedAttractionItemDto.cs
├── Common/
│   ├── Exceptions/
│   │   └── UnprocessableEntityException.cs  (nowy)
│   └── Interfaces/
│       └── IRouteOptimizationService.cs  (nowy)

src/TripPlanner.Infrastructure/
└── Services/
    └── RouteOptimizationService.cs  (nowy)

src/TripPlanner.WebApi/
└── Endpoints/
    └── RouteOptimizationEndpoints.cs  (nowy)
```

### Krok 2: Implementacja DTOs

**OptimizedAttractionItemDto.cs:**
```csharp
namespace TripPlanner.Application.RouteOptimization.DTOs;

public record OptimizedAttractionItemDto(
    Guid AttractionId,
    string AttractionName,
    int DayNumber,
    int OrderIndex
);
```

**OptimizeRouteResponseDto.cs:**
```csharp
namespace TripPlanner.Application.RouteOptimization.DTOs;

public record OptimizeRouteResponseDto(
    Guid TripId,
    IReadOnlyList<OptimizedAttractionItemDto> OptimizedOrder,
    decimal TotalDistance,
    DateTime OptimizedAt
);
```

### Krok 3: Implementacja wyjątku UnprocessableEntityException

```csharp
namespace TripPlanner.Application.Common.Exceptions;

public class UnprocessableEntityException : Exception
{
    public UnprocessableEntityException(string message)
        : base(message)
    {
    }
}
```

### Krok 4: Implementacja IRouteOptimizationService

**Interface:**
```csharp
namespace TripPlanner.Application.Common.Interfaces;

public interface IRouteOptimizationService
{
    RouteOptimizationResult OptimizeRoute(
        IReadOnlyList<AttractionWithCoordinates> attractions,
        Guid startingAttractionId
    );

    decimal CalculateHaversineDistance(
        decimal lat1, decimal lon1,
        decimal lat2, decimal lon2
    );
}

public record AttractionWithCoordinates(
    Guid Id,
    string Name,
    decimal Latitude,
    decimal Longitude,
    int CurrentDayNumber
);

public record RouteOptimizationResult(
    IReadOnlyList<OptimizedAttraction> OptimizedAttractions,
    decimal TotalDistanceMeters
);

public record OptimizedAttraction(
    Guid AttractionId,
    string AttractionName,
    int OrderIndex
);
```

**Implementacja:**
```csharp
namespace TripPlanner.Infrastructure.Services;

public class RouteOptimizationService : IRouteOptimizationService
{
    private const double EarthRadiusMeters = 6371000;

    public RouteOptimizationResult OptimizeRoute(
        IReadOnlyList<AttractionWithCoordinates> attractions,
        Guid startingAttractionId)
    {
        if (attractions.Count == 0)
            return new RouteOptimizationResult([], 0);

        if (attractions.Count == 1)
            return new RouteOptimizationResult(
                [new OptimizedAttraction(attractions[0].Id, attractions[0].Name, 1)],
                0
            );

        var visited = new HashSet<Guid>();
        var result = new List<OptimizedAttraction>();
        var totalDistance = 0m;

        // Find starting attraction
        var current = attractions.FirstOrDefault(a => a.Id == startingAttractionId)
            ?? throw new InvalidOperationException("Starting attraction not found in the list.");

        visited.Add(current.Id);
        result.Add(new OptimizedAttraction(current.Id, current.Name, 1));

        // Nearest Neighbor algorithm
        while (visited.Count < attractions.Count)
        {
            AttractionWithCoordinates? nearest = null;
            var minDistance = decimal.MaxValue;

            foreach (var attraction in attractions)
            {
                if (visited.Contains(attraction.Id))
                    continue;

                var distance = CalculateHaversineDistance(
                    current.Latitude, current.Longitude,
                    attraction.Latitude, attraction.Longitude
                );

                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = attraction;
                }
            }

            if (nearest != null)
            {
                visited.Add(nearest.Id);
                totalDistance += minDistance;
                result.Add(new OptimizedAttraction(
                    nearest.Id,
                    nearest.Name,
                    result.Count + 1
                ));
                current = nearest;
            }
        }

        return new RouteOptimizationResult(result, totalDistance);
    }

    public decimal CalculateHaversineDistance(
        decimal lat1, decimal lon1,
        decimal lat2, decimal lon2)
    {
        var dLat = DegreesToRadians((double)(lat2 - lat1));
        var dLon = DegreesToRadians((double)(lon2 - lon1));

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(DegreesToRadians((double)lat1)) *
                Math.Cos(DegreesToRadians((double)lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return (decimal)(EarthRadiusMeters * c);
    }

    private static double DegreesToRadians(double degrees) => degrees * Math.PI / 180;
}
```

### Krok 5: Implementacja Command i Handler

**OptimizeRouteCommand.cs:**
```csharp
namespace TripPlanner.Application.RouteOptimization.Commands.OptimizeRoute;

public record OptimizeRouteCommand(
    Guid TripId,
    Guid StartingAttractionId
) : IRequest<OptimizeRouteResponseDto>;
```

**OptimizeRouteCommandValidator.cs:**
```csharp
namespace TripPlanner.Application.RouteOptimization.Commands.OptimizeRoute;

public class OptimizeRouteCommandValidator : AbstractValidator<OptimizeRouteCommand>
{
    public OptimizeRouteCommandValidator()
    {
        RuleFor(x => x.TripId)
            .NotEmpty()
            .WithMessage("Trip ID is required.");

        RuleFor(x => x.StartingAttractionId)
            .NotEmpty()
            .WithMessage("Starting attraction ID is required.");
    }
}
```

**OptimizeRouteCommandHandler.cs:**
```csharp
namespace TripPlanner.Application.RouteOptimization.Commands.OptimizeRoute;

public class OptimizeRouteCommandHandler
    : IRequestHandler<OptimizeRouteCommand, OptimizeRouteResponseDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IRouteOptimizationService _routeOptimizationService;

    public OptimizeRouteCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IRouteOptimizationService routeOptimizationService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _routeOptimizationService = routeOptimizationService;
    }

    public async Task<OptimizeRouteResponseDto> Handle(
        OptimizeRouteCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Get trip with attractions
        var trip = await _context.Trips
            .Include(t => t.TripAttractions)
                .ThenInclude(ta => ta.Attraction)
            .FirstOrDefaultAsync(t => t.Id == request.TripId, cancellationToken);

        if (trip is null)
            throw new NotFoundException(nameof(Trip), request.TripId);

        // 2. Authorization check
        if (trip.OwnerId != _currentUserService.UserId)
            throw new ForbiddenAccessException();

        // 3. Check if trip has attractions
        if (trip.TripAttractions.Count == 0)
            throw new UnprocessableEntityException("Trip has no attractions to optimize.");

        // 4. Check if starting attraction is in trip
        var startingAttractionExists = trip.TripAttractions
            .Any(ta => ta.AttractionId == request.StartingAttractionId);

        if (!startingAttractionExists)
            throw new InvalidOperationException(
                $"Starting attraction with ID '{request.StartingAttractionId}' is not assigned to this trip.");

        // 5. Prepare data for optimization
        var attractionsWithCoordinates = trip.TripAttractions
            .Select(ta => new AttractionWithCoordinates(
                ta.AttractionId,
                ta.Attraction.Name,
                ta.Attraction.Latitude,
                ta.Attraction.Longitude,
                ta.DayNumber
            ))
            .ToList();

        // 6. Run optimization
        var optimizationResult = _routeOptimizationService.OptimizeRoute(
            attractionsWithCoordinates,
            request.StartingAttractionId
        );

        // 7. Update order_index in trip_attractions
        foreach (var optimizedAttraction in optimizationResult.OptimizedAttractions)
        {
            var tripAttraction = trip.TripAttractions
                .First(ta => ta.AttractionId == optimizedAttraction.AttractionId);

            tripAttraction.OrderIndex = optimizedAttraction.OrderIndex;
            tripAttraction.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);

        // 8. Build response
        var optimizedOrder = optimizationResult.OptimizedAttractions
            .Select(oa => {
                var tripAttraction = trip.TripAttractions
                    .First(ta => ta.AttractionId == oa.AttractionId);
                return new OptimizedAttractionItemDto(
                    oa.AttractionId,
                    oa.AttractionName,
                    tripAttraction.DayNumber,
                    oa.OrderIndex
                );
            })
            .OrderBy(x => x.OrderIndex)
            .ToList();

        return new OptimizeRouteResponseDto(
            request.TripId,
            optimizedOrder,
            optimizationResult.TotalDistanceMeters,
            DateTime.UtcNow
        );
    }
}
```

### Krok 6: Rejestracja serwisu w DI

**DependencyInjection.cs (Infrastructure):**
```csharp
services.AddScoped<IRouteOptimizationService, RouteOptimizationService>();
```

### Krok 7: Aktualizacja ExceptionHandlingMiddleware

```csharp
// Dodać do metody HandleExceptionAsync
UnprocessableEntityException => (StatusCodes.Status422UnprocessableEntity, "Unprocessable Entity"),
InvalidOperationException when ex.Message.Contains("Starting attraction")
    => (StatusCodes.Status400BadRequest, "Bad Request"),
```

### Krok 8: Implementacja Endpointu

**RouteOptimizationEndpoints.cs:**
```csharp
namespace TripPlanner.WebApi.Endpoints;

public static class RouteOptimizationEndpoints
{
    public static WebApplication MapRouteOptimizationEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/trips/{tripId:guid}")
            .WithTags("Route Optimization")
            .RequireAuthorization();

        group.MapPost("/optimize-route", OptimizeRoute)
            .WithName("OptimizeRoute")
            .WithDescription("Calculate and apply optimal visiting order using nearest neighbor algorithm")
            .Produces<OptimizeRouteResponseDto>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        return app;
    }

    private static async Task<IResult> OptimizeRoute(
        Guid tripId,
        OptimizeRouteRequest request,
        ISender mediator,
        CancellationToken cancellationToken = default)
    {
        var command = new OptimizeRouteCommand(tripId, request.StartingAttractionId);
        var result = await mediator.Send(command, cancellationToken);
        return Results.Ok(result);
    }
}

public record OptimizeRouteRequest(Guid StartingAttractionId);
```

### Krok 9: Rejestracja Endpointu

**Program.cs lub WebApplicationExtensions.cs:**
```csharp
app.MapRouteOptimizationEndpoints();
```

### Krok 10: Testy jednostkowe

**RouteOptimizationServiceTests.cs:**
```csharp
[TestClass]
public class RouteOptimizationServiceTests
{
    private RouteOptimizationService _service;

    [TestInitialize]
    public void Setup()
    {
        _service = new RouteOptimizationService();
    }

    [TestMethod]
    public void CalculateHaversineDistance_ShouldReturnCorrectDistance()
    {
        // Athens to Thessaloniki ~300km
        var distance = _service.CalculateHaversineDistance(
            37.9838m, 23.7275m,  // Athens
            40.6401m, 22.9444m   // Thessaloniki
        );

        Assert.IsTrue(distance > 290000 && distance < 320000);
    }

    [TestMethod]
    public void OptimizeRoute_WithSingleAttraction_ShouldReturnIt()
    {
        var attractions = new List<AttractionWithCoordinates>
        {
            new(Guid.NewGuid(), "Acropolis", 37.9715m, 23.7257m, 1)
        };

        var result = _service.OptimizeRoute(attractions, attractions[0].Id);

        Assert.AreEqual(1, result.OptimizedAttractions.Count);
        Assert.AreEqual(0, result.TotalDistanceMeters);
    }

    [TestMethod]
    public void OptimizeRoute_ShouldStartWithSpecifiedAttraction()
    {
        var startId = Guid.NewGuid();
        var attractions = new List<AttractionWithCoordinates>
        {
            new(Guid.NewGuid(), "A", 37.0m, 23.0m, 1),
            new(startId, "B", 38.0m, 24.0m, 1),
            new(Guid.NewGuid(), "C", 39.0m, 25.0m, 1)
        };

        var result = _service.OptimizeRoute(attractions, startId);

        Assert.AreEqual(startId, result.OptimizedAttractions[0].AttractionId);
    }
}
```

### Krok 11: Testy integracyjne

**OptimizeRouteEndpointTests.cs:**
```csharp
[TestClass]
public class OptimizeRouteEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    [TestMethod]
    public async Task OptimizeRoute_WithValidRequest_ShouldReturn200()
    {
        // Arrange - create trip with attractions
        // Act - POST /api/trips/{tripId}/optimize-route
        // Assert - 200 OK with optimized order
    }

    [TestMethod]
    public async Task OptimizeRoute_WithNonExistentTrip_ShouldReturn404()
    {
        // ...
    }

    [TestMethod]
    public async Task OptimizeRoute_WithUnauthorizedUser_ShouldReturn403()
    {
        // ...
    }

    [TestMethod]
    public async Task OptimizeRoute_WithEmptyTrip_ShouldReturn422()
    {
        // ...
    }
}
```

## 10. Checklist przed wdrożeniem

- [ ] Utworzone wszystkie pliki DTO
- [ ] Zaimplementowany `IRouteOptimizationService` z algorytmem Nearest Neighbor
- [ ] Zaimplementowany `OptimizeRouteCommandHandler` z pełną walidacją
- [ ] Dodany `UnprocessableEntityException` i jego obsługa w middleware
- [ ] Zarejestrowany serwis w DI
- [ ] Utworzony endpoint w `RouteOptimizationEndpoints.cs`
- [ ] Zarejestrowany endpoint w `Program.cs`
- [ ] Napisane testy jednostkowe dla `RouteOptimizationService`
- [ ] Napisane testy integracyjne dla endpointu
- [ ] Przetestowane ręcznie przez Swagger/Postman
- [ ] Sprawdzone logowanie błędów
- [ ] Zaktualizowana dokumentacja API (jeśli dotyczy)
