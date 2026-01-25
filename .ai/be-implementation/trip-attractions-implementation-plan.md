# API Endpoint Implementation Plan: Trip Attractions

## 1. Przegląd punktu końcowego

Endpoint `/api/trips/{tripId}/attractions` zarządza atrakcjami przypisanymi do wycieczki. Umożliwia przeglądanie harmonogramu pogrupowanego po dniach, dodawanie/usuwanie atrakcji, aktualizację przypisań (dzień, kolejność, planowany czas) oraz masową zmianę kolejności. Wszystkie operacje wymagają uwierzytelnienia, a modyfikacje są dozwolone tylko dla właściciela wycieczki.

## 2. Szczegóły żądań

### 2.1 GET /api/trips/{tripId}/attractions
- **Metoda HTTP:** GET
- **URL:** `/api/trips/{tripId}/attractions`
- **Parametry Path:**

| Parametr | Typ | Wymagany | Opis |
|----------|-----|----------|------|
| tripId | UUID | Tak | Identyfikator wycieczki |

### 2.2 POST /api/trips/{tripId}/attractions
- **Metoda HTTP:** POST
- **URL:** `/api/trips/{tripId}/attractions`
- **Parametry Path:** `tripId` (UUID)
- **Request Body:**
```json
{
  "attractionId": "uuid",
  "dayNumber": "int (>0)",
  "orderIndex": "int (>=0)"
}
```

### 2.3 PUT /api/trips/{tripId}/attractions/{attractionId}
- **Metoda HTTP:** PUT
- **URL:** `/api/trips/{tripId}/attractions/{attractionId}`
- **Parametry Path:** `tripId` (UUID), `attractionId` (UUID)
- **Request Body:**
```json
{
  "dayNumber": "int (>0)",
  "orderIndex": "int (>=0)",
  "plannedStartTime": "string (HH:mm) | null"
}
```

### 2.4 DELETE /api/trips/{tripId}/attractions/{attractionId}
- **Metoda HTTP:** DELETE
- **URL:** `/api/trips/{tripId}/attractions/{attractionId}`
- **Parametry Path:** `tripId` (UUID), `attractionId` (UUID)

### 2.5 POST /api/trips/{tripId}/attractions/reorder
- **Metoda HTTP:** POST
- **URL:** `/api/trips/{tripId}/attractions/reorder`
- **Parametry Path:** `tripId` (UUID)
- **Request Body:**
```json
{
  "attractions": [
    {
      "attractionId": "uuid",
      "dayNumber": "int (>0)",
      "orderIndex": "int (>=0)"
    }
  ]
}
```

## 3. Wykorzystywane typy

### 3.1 DTOs

```csharp
// Skrócone dane atrakcji dla zagnieżdżenia
public record AttractionSummaryDto(
    Guid Id,
    string Name,
    decimal Latitude,
    decimal Longitude,
    decimal? Rating,
    int EstimatedDuration,
    string? ImageUrl
);

// Pojedyncza atrakcja w harmonogramie (z pełnymi danymi attraction)
public record TripAttractionDetailDto(
    Guid Id,
    Guid AttractionId,
    AttractionSummaryDto Attraction,
    int DayNumber,
    int OrderIndex,
    TimeOnly? PlannedStartTime
);

// Dzień wycieczki z atrakcjami
public record TripDayDto(
    int DayNumber,
    int TotalDuration,
    IReadOnlyList<TripAttractionDetailDto> Attractions
);

// Pełny harmonogram wycieczki (response GET)
public record TripScheduleDto(
    Guid TripId,
    int TotalDays,
    int TotalDuration,
    IReadOnlyList<TripDayDto> Days
);

// Response dla POST/PUT pojedynczej atrakcji
public record TripAttractionDto(
    Guid Id,
    Guid TripId,
    Guid AttractionId,
    int DayNumber,
    int OrderIndex,
    TimeOnly? PlannedStartTime,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

// Response dla reorder
public record ReorderResultDto(
    Guid TripId,
    int UpdatedCount,
    DateTime UpdatedAt
);
```

### 3.2 Commands

```csharp
public record AddTripAttractionCommand(
    Guid TripId,
    Guid AttractionId,
    int DayNumber,
    int OrderIndex
) : IRequest<TripAttractionDto>;

public record UpdateTripAttractionCommand(
    Guid TripId,
    Guid AttractionId,
    int DayNumber,
    int OrderIndex,
    string? PlannedStartTime
) : IRequest<TripAttractionDto>;

public record RemoveTripAttractionCommand(
    Guid TripId,
    Guid AttractionId
) : IRequest<Unit>;

public record ReorderAttractionItem(
    Guid AttractionId,
    int DayNumber,
    int OrderIndex
);

public record ReorderTripAttractionsCommand(
    Guid TripId,
    IReadOnlyList<ReorderAttractionItem> Attractions
) : IRequest<ReorderResultDto>;
```

### 3.3 Queries

```csharp
public record GetTripAttractionsQuery(
    Guid TripId
) : IRequest<TripScheduleDto>;
```

## 4. Szczegóły odpowiedzi

### 4.1 GET /api/trips/{tripId}/attractions - 200 OK
```json
{
  "tripId": "uuid",
  "totalDays": 3,
  "totalDuration": 1440,
  "days": [
    {
      "dayNumber": 1,
      "totalDuration": 480,
      "attractions": [
        {
          "id": "uuid",
          "attractionId": "uuid",
          "attraction": {
            "id": "uuid",
            "name": "string",
            "latitude": 0.0,
            "longitude": 0.0,
            "rating": 4.8,
            "estimatedDuration": 180,
            "imageUrl": "string"
          },
          "dayNumber": 1,
          "orderIndex": 0,
          "plannedStartTime": "09:00:00"
        }
      ]
    }
  ]
}
```

### 4.2 POST /api/trips/{tripId}/attractions - 201 Created
```json
{
  "id": "uuid",
  "tripId": "uuid",
  "attractionId": "uuid",
  "dayNumber": 1,
  "orderIndex": 3,
  "plannedStartTime": null,
  "createdAt": "2026-01-22T10:00:00Z",
  "updatedAt": null
}
```
Header: `Location: /api/trips/{tripId}/attractions/{attractionId}`

### 4.3 PUT /api/trips/{tripId}/attractions/{attractionId} - 200 OK
```json
{
  "id": "uuid",
  "tripId": "uuid",
  "attractionId": "uuid",
  "dayNumber": 2,
  "orderIndex": 0,
  "plannedStartTime": "10:00:00",
  "createdAt": "2026-01-22T10:00:00Z",
  "updatedAt": "2026-01-22T11:00:00Z"
}
```

### 4.4 DELETE /api/trips/{tripId}/attractions/{attractionId} - 204 No Content

### 4.5 POST /api/trips/{tripId}/attractions/reorder - 200 OK
```json
{
  "tripId": "uuid",
  "updatedCount": 3,
  "updatedAt": "2026-01-22T11:00:00Z"
}
```

## 5. Przepływ danych

```
┌─────────────────────┐     ┌──────────────────┐     ┌───────────────────┐
│   WebApi Layer      │────▶│ Application Layer│────▶│ Infrastructure    │
│ TripAttractionEndpts│     │ (MediatR/CQRS)   │     │ (EF Core/DB)      │
└─────────────────────┘     └──────────────────┘     └───────────────────┘
```

### Przepływ dla GET /api/trips/{tripId}/attractions:
1. Request → TripAttractionEndpoints.GetTripAttractions()
2. Mapowanie path param → GetTripAttractionsQuery
3. GetTripAttractionsQueryHandler:
   - Sprawdza istnienie Trip
   - Weryfikuje dostęp (publiczny LUB właściciel)
   - Pobiera TripAttractions z Include(Attraction)
   - Grupuje po DayNumber
   - Oblicza TotalDuration per day i total
   - Zwraca TripScheduleDto
4. Response 200 OK

### Przepływ dla POST (Add):
1. Request → AddTripAttractionCommand
2. ValidationBehavior waliduje command
3. AddTripAttractionCommandHandler:
   - Sprawdza uwierzytelnienie
   - Weryfikuje istnienie Trip + ownership
   - Weryfikuje istnienie Attraction
   - Sprawdza unikalność (trip_id, attraction_id)
   - Tworzy TripAttraction entity
   - Zapisuje w DB
   - Zwraca TripAttractionDto
4. Response 201 Created

### Przepływ dla Reorder:
1. Request → ReorderTripAttractionsCommand
2. Handler:
   - Weryfikuje ownership
   - Pobiera wszystkie TripAttractions dla trip
   - Waliduje że wszystkie attractionId z request istnieją w trip
   - Aktualizuje DayNumber i OrderIndex dla każdego
   - Zapisuje batch w transakcji
   - Zwraca ReorderResultDto

## 6. Względy bezpieczeństwa

### 6.1 Uwierzytelnianie
- Wszystkie endpointy wymagają `.RequireAuthorization()`
- Token JWT w headerze `Authorization: Bearer {token}`
- Brak tokenu → 401 Unauthorized

### 6.2 Autoryzacja (poziom handlera)
```csharp
// Wspólna metoda weryfikacji ownership (można wyodrębnić)
private async Task<Trip> GetTripAndVerifyOwnership(Guid tripId, CancellationToken ct)
{
    var trip = await _context.Trips
        .FirstOrDefaultAsync(t => t.Id == tripId, ct);

    if (trip == null)
        throw new NotFoundException("Trip", tripId);

    if (trip.OwnerId != _currentUserService.UserId)
        throw new ForbiddenAccessException("Cannot modify trip owned by another user");

    return trip;
}

// Dla GET - pozwala na publiczne lub własne
private async Task<Trip> GetTripAndVerifyAccess(Guid tripId, CancellationToken ct)
{
    var trip = await _context.Trips
        .FirstOrDefaultAsync(t => t.Id == tripId, ct);

    if (trip == null)
        throw new NotFoundException("Trip", tripId);

    if (!trip.IsPublic && trip.OwnerId != _currentUserService.UserId)
        throw new ForbiddenAccessException("Trip is private and not owned by user");

    return trip;
}
```

### 6.3 Ochrona przed IDOR
- Zawsze sprawdzaj że TripAttraction należy do podanego Trip
- Nie pozwalaj na modyfikację przez attraction_id bez weryfikacji trip_id

### 6.4 Walidacja danych wejściowych
- FluentValidation dla wszystkich commands
- Constraint CHECK w bazie jako ostatnia linia obrony

## 7. Obsługa błędów

| Wyjątek | HTTP Status | Warunek |
|---------|-------------|---------|
| `UnauthorizedAccessException` | 401 | Brak uwierzytelnienia |
| `ForbiddenAccessException` | 403 | Brak uprawnień do trip |
| `NotFoundException` | 404 | Trip/Attraction/TripAttraction nie istnieje |
| `ValidationException` | 400 | Błędy walidacji FluentValidation |
| `ConflictException` | 409 | Attraction już istnieje w trip (UNIQUE violation) |
| `Exception` | 500 | Nieoczekiwany błąd serwera |

### Format błędu 409 Conflict:
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.8",
  "title": "Conflict",
  "status": 409,
  "detail": "Attraction already exists in this trip"
}
```

## 8. Wydajność

### 8.1 Optymalizacje zapytań
- `AsNoTracking()` dla GET query
- `Include(ta => ta.Attraction)` - eager loading
- Grupowanie w pamięci po pobraniu (lub GroupBy w LINQ to Entities)

### 8.2 Indeksy (już w schemacie + dodatkowe)
```sql
-- Już istnieje z UNIQUE constraint
-- CREATE UNIQUE INDEX ON trip_attractions(trip_id, attraction_id);

-- Dodatkowe dla wydajności
CREATE INDEX idx_trip_attractions_trip_day
ON trip_attractions(trip_id, day_number, order_index);
```

### 8.3 Batch operations
- Reorder używa pojedynczej transakcji dla wszystkich updates
- `SaveChangesAsync()` wywołane raz na końcu

### 8.4 Obliczanie TotalDays i TotalDuration
```csharp
// W query handler
var totalDays = tripAttractions.Any()
    ? tripAttractions.Max(ta => ta.DayNumber)
    : 0;

var totalDuration = tripAttractions
    .Sum(ta => ta.Attraction.EstimatedDuration);
```

## 9. Kroki implementacji

### Krok 1: DTOs (Application/TripAttractions/DTOs/)
1. Utwórz `AttractionSummaryDto.cs`
2. Utwórz `TripAttractionDto.cs`
3. Utwórz `TripAttractionDetailDto.cs`
4. Utwórz `TripDayDto.cs`
5. Utwórz `TripScheduleDto.cs`
6. Utwórz `ReorderResultDto.cs`

### Krok 2: Query (Application/TripAttractions/Queries/)

**GetTripAttractions/**
1. `GetTripAttractionsQuery.cs`
2. `GetTripAttractionsQueryHandler.cs` - logika grupowania po dniach

### Krok 3: Commands (Application/TripAttractions/Commands/)

**AddTripAttraction/**
1. `AddTripAttractionCommand.cs`
2. `AddTripAttractionCommandHandler.cs`
3. `AddTripAttractionCommandValidator.cs`

**UpdateTripAttraction/**
1. `UpdateTripAttractionCommand.cs`
2. `UpdateTripAttractionCommandHandler.cs`
3. `UpdateTripAttractionCommandValidator.cs`

**RemoveTripAttraction/**
1. `RemoveTripAttractionCommand.cs`
2. `RemoveTripAttractionCommandHandler.cs`

**ReorderTripAttractions/**
1. `ReorderTripAttractionsCommand.cs`
2. `ReorderTripAttractionsCommandHandler.cs`
3. `ReorderTripAttractionsCommandValidator.cs`

### Krok 4: Endpoint (WebApi/Endpoints/)
1. Utwórz `TripAttractionEndpoints.cs`
2. Zdefiniuj wszystkie 5 endpointów
3. Zarejestruj w `WebApplicationExtensions.MapEndpoints()`

### Krok 5: Exception handling
1. Dodaj `ConflictException` do Application/Common/Exceptions/ (jeśli nie istnieje)
2. Upewnij się że middleware obsługuje 409

### Krok 6: Testy
1. Testy jednostkowe dla handlerów
2. Testy walidatorów
3. Testy integracyjne endpointów

---

## 10. Struktura plików do utworzenia

```
src/TripPlanner.Application/
└── TripAttractions/
    ├── DTOs/
    │   ├── AttractionSummaryDto.cs
    │   ├── TripAttractionDto.cs
    │   ├── TripAttractionDetailDto.cs
    │   ├── TripDayDto.cs
    │   ├── TripScheduleDto.cs
    │   └── ReorderResultDto.cs
    ├── Commands/
    │   ├── AddTripAttraction/
    │   │   ├── AddTripAttractionCommand.cs
    │   │   ├── AddTripAttractionCommandHandler.cs
    │   │   └── AddTripAttractionCommandValidator.cs
    │   ├── UpdateTripAttraction/
    │   │   ├── UpdateTripAttractionCommand.cs
    │   │   ├── UpdateTripAttractionCommandHandler.cs
    │   │   └── UpdateTripAttractionCommandValidator.cs
    │   ├── RemoveTripAttraction/
    │   │   ├── RemoveTripAttractionCommand.cs
    │   │   └── RemoveTripAttractionCommandHandler.cs
    │   └── ReorderTripAttractions/
    │       ├── ReorderTripAttractionsCommand.cs
    │       ├── ReorderTripAttractionsCommandHandler.cs
    │       └── ReorderTripAttractionsCommandValidator.cs
    └── Queries/
        └── GetTripAttractions/
            ├── GetTripAttractionsQuery.cs
            └── GetTripAttractionsQueryHandler.cs

src/TripPlanner.WebApi/
└── Endpoints/
    └── TripAttractionEndpoints.cs
```
