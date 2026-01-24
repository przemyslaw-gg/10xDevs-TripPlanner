# API Endpoint Implementation Plan: Trips

## 1. Przegląd punktu końcowego

Endpoint `/api/trips` zarządza planami wycieczek użytkowników. Umożliwia pełny CRUD dla trips oraz zmianę statusu publiczny/prywatny. Wszystkie operacje wymagają uwierzytelnienia. Użytkownicy mogą przeglądać publiczne trips innych użytkowników, ale modyfikować tylko własne.

## 2. Szczegóły żądań

### 2.1 GET /api/trips
- **Metoda HTTP:** GET
- **URL:** `/api/trips`
- **Parametry Query:**

| Parametr | Typ | Wymagany | Domyślnie | Opis |
|----------|-----|----------|-----------|------|
| locationId | UUID | Nie | - | Filtruj po lokalizacji |
| onlyMine | bool | Nie | false | Tylko własne trips |
| onlyPublic | bool | Nie | false | Tylko publiczne trips |
| search | string | Nie | - | Szukaj po nazwie |
| page | int | Nie | 1 | Numer strony |
| pageSize | int | Nie | 20 | Rozmiar strony (max 100) |

### 2.2 GET /api/trips/{id}
- **Metoda HTTP:** GET
- **URL:** `/api/trips/{id}`
- **Parametry Path:** `id` (UUID) - identyfikator trip

### 2.3 POST /api/trips
- **Metoda HTTP:** POST
- **URL:** `/api/trips`
- **Request Body:**
```json
{
  "name": "string (max 100)",
  "locationId": "uuid",
  "dailyHours": "int (1-24)",
  "maxExtensionHours": "int (0-8)",
  "startTime": "string (HH:mm)"
}
```

### 2.4 PUT /api/trips/{id}
- **Metoda HTTP:** PUT
- **URL:** `/api/trips/{id}`
- **Parametry Path:** `id` (UUID)
- **Request Body:** (jak POST)

### 2.5 DELETE /api/trips/{id}
- **Metoda HTTP:** DELETE
- **URL:** `/api/trips/{id}`
- **Parametry Path:** `id` (UUID)

### 2.6 PATCH /api/trips/{id}/publish
- **Metoda HTTP:** PATCH
- **URL:** `/api/trips/{id}/publish`
- **Parametry Path:** `id` (UUID)
- **Request Body:**
```json
{
  "isPublic": "bool"
}
```

## 3. Wykorzystywane typy

### 3.1 DTOs

```csharp
// Pełne szczegóły trip (dla GET /{id}, POST, PUT)
public record TripDto(
    Guid Id,
    Guid OwnerId,
    string Name,
    Guid LocationId,
    LocationDetailDto Location,
    bool IsPublic,
    int DailyHours,
    int MaxExtensionHours,
    TimeOnly StartTime,
    bool IsOwner,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

// Element listy trips (dla GET /)
public record TripListItemDto(
    Guid Id,
    Guid OwnerId,
    string Name,
    Guid LocationId,
    LocationSummaryDto Location,
    bool IsPublic,
    int DailyHours,
    int MaxExtensionHours,
    TimeOnly StartTime,
    int AttractionCount,
    int TotalDays,
    bool IsOwner,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

// Dla PATCH /publish
public record TripPublishDto(
    Guid Id,
    bool IsPublic,
    DateTime UpdatedAt
);

// Lokalizacja ze strefą czasową
public record LocationDetailDto(
    Guid Id,
    string Name,
    string Country,
    string Timezone
);
```

### 3.2 Commands

```csharp
public record CreateTripCommand(
    string Name,
    Guid LocationId,
    int DailyHours,
    int MaxExtensionHours,
    string StartTime
) : IRequest<TripDto>;

public record UpdateTripCommand(
    Guid Id,
    string Name,
    Guid LocationId,
    int DailyHours,
    int MaxExtensionHours,
    string StartTime
) : IRequest<TripDto>;

public record DeleteTripCommand(Guid Id) : IRequest<Unit>;

public record PublishTripCommand(
    Guid Id,
    bool IsPublic
) : IRequest<TripPublishDto>;
```

### 3.3 Queries

```csharp
public record GetTripsQuery(
    Guid? LocationId = null,
    bool OnlyMine = false,
    bool OnlyPublic = false,
    string? Search = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<PaginatedList<TripListItemDto>>;

public record GetTripByIdQuery(Guid Id) : IRequest<TripDto>;
```

## 4. Szczegóły odpowiedzi

### 4.1 GET /api/trips - 200 OK
```json
{
  "items": [TripListItemDto],
  "pagination": {
    "page": 1,
    "pageSize": 20,
    "totalItems": 5,
    "totalPages": 1,
    "hasNextPage": false,
    "hasPreviousPage": false
  }
}
```

### 4.2 GET /api/trips/{id} - 200 OK
```json
TripDto
```

### 4.3 POST /api/trips - 201 Created
```json
TripDto
```
Header: `Location: /api/trips/{id}`

### 4.4 PUT /api/trips/{id} - 200 OK
```json
TripDto
```

### 4.5 DELETE /api/trips/{id} - 204 No Content

### 4.6 PATCH /api/trips/{id}/publish - 200 OK
```json
TripPublishDto
```

## 5. Przepływ danych

```
┌─────────────────┐     ┌──────────────────┐     ┌───────────────────┐
│   WebApi Layer  │────▶│ Application Layer│────▶│ Infrastructure    │
│   (Endpoints)   │     │ (MediatR/CQRS)   │     │ (EF Core/DB)      │
└─────────────────┘     └──────────────────┘     └───────────────────┘
        │                       │                        │
        ▼                       ▼                        ▼
   TripEndpoints          Commands/Queries         ApplicationDbContext
        │                       │                        │
        │                       ▼                        │
        │               Validators (FluentValidation)    │
        │                       │                        │
        │                       ▼                        │
        │               Handlers                         │
        │                       │                        │
        └───────────────────────┴────────────────────────┘
```

### Przepływ dla GET /api/trips:
1. Request → TripEndpoints.GetTrips()
2. Mapowanie query params → GetTripsQuery
3. ValidationBehavior waliduje query
4. GetTripsQueryHandler:
   - Pobiera CurrentUserId
   - Buduje query z filtrami (publiczne LUB własne)
   - Aplikuje filtry (locationId, search, onlyMine, onlyPublic)
   - Oblicza AttractionCount, TotalDays
   - Zwraca PaginatedList<TripListItemDto>
5. Endpoint mapuje na PaginatedResponse
6. Response 200 OK

### Przepływ dla POST /api/trips:
1. Request body → CreateTripRequest
2. Mapowanie → CreateTripCommand
3. ValidationBehavior waliduje command
4. CreateTripCommandHandler:
   - Sprawdza uwierzytelnienie
   - Weryfikuje istnienie Location
   - Tworzy Trip entity
   - Zapisuje w DB
   - Zwraca TripDto
5. Response 201 Created

## 6. Względy bezpieczeństwa

### 6.1 Uwierzytelnianie
- Wszystkie endpointy wymagają `.RequireAuthorization()`
- Token JWT w headerze `Authorization: Bearer {token}`
- Brak tokenu → 401 Unauthorized

### 6.2 Autoryzacja (poziom handlera)
```csharp
// Weryfikacja właściciela w Update/Delete/Publish
if (trip.OwnerId != _currentUserService.UserId)
    throw new ForbiddenAccessException("Cannot modify trip owned by another user");

// Weryfikacja dostępu w GetById
if (!trip.IsPublic && trip.OwnerId != _currentUserService.UserId)
    throw new ForbiddenAccessException("Trip is private and not owned by user");
```

### 6.3 Ochrona przed IDOR
- Zawsze sprawdzaj ownership przed modyfikacją
- Filtruj listę: `IsPublic || OwnerId == currentUserId`

### 6.4 Walidacja danych wejściowych
- FluentValidation przed dotarciem do handlera
- Sanityzacja search query (trim, escape)

## 7. Obsługa błędów

| Wyjątek | HTTP Status | Warunek |
|---------|-------------|---------|
| `UnauthorizedAccessException` | 401 | Brak uwierzytelnienia |
| `ForbiddenAccessException` | 403 | Brak uprawnień do zasobu |
| `NotFoundException` | 404 | Trip/Location nie istnieje |
| `ValidationException` | 400 | Błędy walidacji FluentValidation |
| `Exception` | 500 | Nieoczekiwany błąd serwera |

### Format błędu (Problem Details):
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Validation Failed",
  "status": 400,
  "errors": {
    "name": ["The Name field is required."],
    "dailyHours": ["Daily hours must be between 1 and 24."]
  }
}
```

## 8. Wydajność

### 8.1 Optymalizacje zapytań
- `AsNoTracking()` dla wszystkich zapytań odczytu
- Projekcja do DTO przed paginacją
- Eager loading Location dla listy trips

### 8.2 Indeksy (do dodania w migracji)
```sql
CREATE INDEX idx_trips_owner_id ON trips(owner_id);
CREATE INDEX idx_trips_location_id ON trips(location_id);
CREATE INDEX idx_trips_is_public ON trips(is_public);
CREATE INDEX idx_trips_name ON trips(name);
CREATE INDEX idx_trips_owner_public ON trips(owner_id, is_public);
```

### 8.3 Paginacja
- Domyślnie 20 elementów
- Maksymalnie 100 elementów
- Obliczanie TotalDays i AttractionCount w zapytaniu SQL

## 9. Kroki implementacji

### Krok 1: DTOs (Application/Trips/DTOs/)
1. Utwórz `TripDto.cs`
2. Utwórz `TripListItemDto.cs`
3. Utwórz `TripPublishDto.cs`
4. Utwórz `LocationDetailDto.cs` (lub rozszerz istniejące LocationSummaryDto)

### Krok 2: Queries (Application/Trips/Queries/)

**GetTrips/**
1. `GetTripsQuery.cs` - record z parametrami
2. `GetTripsQueryHandler.cs` - logika filtrowania, paginacji
3. `GetTripsQueryValidator.cs` - walidacja page, pageSize, search

**GetTripById/**
1. `GetTripByIdQuery.cs`
2. `GetTripByIdQueryHandler.cs` - sprawdzenie dostępu (public/owner)

### Krok 3: Commands (Application/Trips/Commands/)

**CreateTrip/**
1. `CreateTripCommand.cs`
2. `CreateTripCommandHandler.cs`
3. `CreateTripCommandValidator.cs`

**UpdateTrip/**
1. `UpdateTripCommand.cs`
2. `UpdateTripCommandHandler.cs`
3. `UpdateTripCommandValidator.cs`

**DeleteTrip/**
1. `DeleteTripCommand.cs`
2. `DeleteTripCommandHandler.cs`

**PublishTrip/**
1. `PublishTripCommand.cs`
2. `PublishTripCommandHandler.cs`
3. `PublishTripCommandValidator.cs`

### Krok 4: Endpoint (WebApi/Endpoints/)
1. Utwórz `TripEndpoints.cs`
2. Zdefiniuj wszystkie 6 endpointów
3. Zarejestruj w `WebApplicationExtensions.MapEndpoints()`

### Krok 5: Testy
1. Testy jednostkowe dla handlerów
2. Testy walidatorów
3. Testy integracyjne endpointów

### Krok 6: Migracja (opcjonalnie)
1. Dodaj indeksy dla wydajności

---

## 10. Struktura plików do utworzenia

```
src/TripPlanner.Application/
└── Trips/
    ├── DTOs/
    │   ├── TripDto.cs
    │   ├── TripListItemDto.cs
    │   ├── TripPublishDto.cs
    │   └── LocationDetailDto.cs
    ├── Commands/
    │   ├── CreateTrip/
    │   │   ├── CreateTripCommand.cs
    │   │   ├── CreateTripCommandHandler.cs
    │   │   └── CreateTripCommandValidator.cs
    │   ├── UpdateTrip/
    │   │   ├── UpdateTripCommand.cs
    │   │   ├── UpdateTripCommandHandler.cs
    │   │   └── UpdateTripCommandValidator.cs
    │   ├── DeleteTrip/
    │   │   ├── DeleteTripCommand.cs
    │   │   └── DeleteTripCommandHandler.cs
    │   └── PublishTrip/
    │       ├── PublishTripCommand.cs
    │       ├── PublishTripCommandHandler.cs
    │       └── PublishTripCommandValidator.cs
    └── Queries/
        ├── GetTrips/
        │   ├── GetTripsQuery.cs
        │   ├── GetTripsQueryHandler.cs
        │   └── GetTripsQueryValidator.cs
        └── GetTripById/
            ├── GetTripByIdQuery.cs
            └── GetTripByIdQueryHandler.cs

src/TripPlanner.WebApi/
└── Endpoints/
    └── TripEndpoints.cs
```
