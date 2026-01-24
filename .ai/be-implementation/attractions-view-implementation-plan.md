# API Endpoint Implementation Plan: Attractions

## 1. Przegląd punktu końcowego

Moduł Attractions obsługuje operacje CRUD na atrakcjach turystycznych. Zawiera dwa typy atrakcji:
- **Systemowe (zweryfikowane)** - predefiniowane atrakcje z ocenami i recenzjami
- **Użytkownika (custom)** - atrakcje dodane przez użytkowników, bez weryfikacji

Endpointy:
| Metoda | URL | Opis | Auth |
|--------|-----|------|------|
| GET | /api/attractions | Lista atrakcji z filtrowaniem | Nie |
| GET | /api/attractions/{id} | Szczegóły atrakcji | Nie |
| POST | /api/attractions | Utworzenie custom atrakcji | Tak |
| PUT | /api/attractions/{id} | Aktualizacja custom atrakcji | Tak (owner) |
| DELETE | /api/attractions/{id} | Usunięcie custom atrakcji | Tak (owner) |

---

## 2. Szczegóły żądania

### GET /api/attractions

**Metoda HTTP:** GET
**URL:** `/api/attractions`

**Query Parameters:**
| Parametr | Typ | Wymagany | Domyślna | Walidacja |
|----------|-----|----------|----------|-----------|
| locationId | UUID | Nie | - | Poprawny format GUID |
| search | string | Nie | - | Max 100 znaków |
| sortBy | string | Nie | rating | Enum: rating, name, reviewCount |
| sortOrder | string | Nie | desc | Enum: asc, desc |
| isVerified | bool | Nie | - | true/false |
| page | int | Nie | 1 | >= 1 |
| pageSize | int | Nie | 20 | 1-100 |

### GET /api/attractions/{id}

**Metoda HTTP:** GET
**URL:** `/api/attractions/{id}`

**Path Parameters:**
| Parametr | Typ | Wymagany | Walidacja |
|----------|-----|----------|-----------|
| id | UUID | Tak | Poprawny format GUID |

### POST /api/attractions

**Metoda HTTP:** POST
**URL:** `/api/attractions`
**Headers:** `Authorization: Bearer {token}`

**Request Body:**
```json
{
  "locationId": "uuid",
  "name": "string (1-255 znaków)",
  "description": "string (opcjonalne)",
  "latitude": "decimal (-90 do 90)",
  "longitude": "decimal (-180 do 180)",
  "estimatedDuration": "int (> 0, opcjonalne, default: 60)",
  "imageUrl": "string (opcjonalne, valid URL)"
}
```

### PUT /api/attractions/{id}

**Metoda HTTP:** PUT
**URL:** `/api/attractions/{id}`
**Headers:** `Authorization: Bearer {token}`

**Path Parameters:**
| Parametr | Typ | Wymagany |
|----------|-----|----------|
| id | UUID | Tak |

**Request Body:**
```json
{
  "name": "string (1-255 znaków)",
  "description": "string (opcjonalne)",
  "latitude": "decimal (-90 do 90)",
  "longitude": "decimal (-180 do 180)",
  "estimatedDuration": "int (> 0, opcjonalne)",
  "imageUrl": "string (opcjonalne, valid URL)"
}
```

### DELETE /api/attractions/{id}

**Metoda HTTP:** DELETE
**URL:** `/api/attractions/{id}`
**Headers:** `Authorization: Bearer {token}`

**Path Parameters:**
| Parametr | Typ | Wymagany |
|----------|-----|----------|
| id | UUID | Tak |

---

## 3. Wykorzystywane typy

### 3.1. Domain Entity

**Lokalizacja:** `src/TripPlanner.Domain/Entities/Attraction.cs`

```csharp
public class Attraction
{
    public Guid Id { get; set; }
    public Guid LocationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public decimal Rating { get; set; }
    public int ReviewCount { get; set; }
    public int EstimatedDurationMinutes { get; set; } = 60;
    public string? ImageUrl { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public bool IsUserGenerated { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public Location Location { get; set; } = null!;
}
```

### 3.2. DTOs

**Lokalizacja:** `src/TripPlanner.Application/Attractions/DTOs/`

```csharp
// AttractionListItemDto.cs
public record AttractionListItemDto(
    Guid Id,
    Guid LocationId,
    string Name,
    string? Description,
    decimal Latitude,
    decimal Longitude,
    decimal? Rating,
    int? ReviewCount,
    int EstimatedDuration,
    string? ImageUrl,
    bool IsVerified,
    Guid? CreatedByUserId
);

// AttractionDto.cs
public record AttractionDto(
    Guid Id,
    Guid LocationId,
    LocationSummaryDto Location,
    string Name,
    string? Description,
    decimal Latitude,
    decimal Longitude,
    decimal? Rating,
    int? ReviewCount,
    int EstimatedDuration,
    string? ImageUrl,
    bool IsVerified,
    Guid? CreatedByUserId,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

// LocationSummaryDto.cs
public record LocationSummaryDto(
    Guid Id,
    string Name,
    string Country
);
```

### 3.3. Queries

**Lokalizacja:** `src/TripPlanner.Application/Attractions/Queries/`

```csharp
// GetAttractions/GetAttractionsQuery.cs
public record GetAttractionsQuery(
    Guid? LocationId = null,
    string? Search = null,
    string SortBy = "rating",
    string SortOrder = "desc",
    bool? IsVerified = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<PaginatedList<AttractionListItemDto>>;

// GetAttractionById/GetAttractionByIdQuery.cs
public record GetAttractionByIdQuery(Guid Id) : IRequest<AttractionDto>;
```

### 3.4. Commands

**Lokalizacja:** `src/TripPlanner.Application/Attractions/Commands/`

```csharp
// CreateAttraction/CreateAttractionCommand.cs
public record CreateAttractionCommand(
    Guid LocationId,
    string Name,
    string? Description,
    decimal Latitude,
    decimal Longitude,
    int? EstimatedDuration,
    string? ImageUrl
) : IRequest<AttractionDto>;

// UpdateAttraction/UpdateAttractionCommand.cs
public record UpdateAttractionCommand(
    Guid Id,
    string Name,
    string? Description,
    decimal Latitude,
    decimal Longitude,
    int? EstimatedDuration,
    string? ImageUrl
) : IRequest<AttractionDto>;

// DeleteAttraction/DeleteAttractionCommand.cs
public record DeleteAttractionCommand(Guid Id) : IRequest<Unit>;
```

### 3.5. Validators

**Lokalizacja:** Wraz z odpowiednimi Query/Command

```csharp
// GetAttractionsQueryValidator.cs
public class GetAttractionsQueryValidator : AbstractValidator<GetAttractionsQuery>
{
    private static readonly string[] AllowedSortFields = { "rating", "name", "reviewCount" };
    private static readonly string[] AllowedSortOrders = { "asc", "desc" };

    public GetAttractionsQueryValidator()
    {
        RuleFor(x => x.Search)
            .MaximumLength(100)
            .When(x => x.Search != null);

        RuleFor(x => x.SortBy)
            .Must(x => AllowedSortFields.Contains(x.ToLowerInvariant()))
            .WithMessage("SortBy must be one of: rating, name, reviewCount");

        RuleFor(x => x.SortOrder)
            .Must(x => AllowedSortOrders.Contains(x.ToLowerInvariant()))
            .WithMessage("SortOrder must be 'asc' or 'desc'");

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);
    }
}

// CreateAttractionCommandValidator.cs
public class CreateAttractionCommandValidator : AbstractValidator<CreateAttractionCommand>
{
    public CreateAttractionCommandValidator()
    {
        RuleFor(x => x.LocationId)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(x => x.Description)
            .MaximumLength(5000)
            .When(x => x.Description != null);

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90m, 90m);

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180m, 180m);

        RuleFor(x => x.EstimatedDuration)
            .GreaterThan(0)
            .When(x => x.EstimatedDuration.HasValue);

        RuleFor(x => x.ImageUrl)
            .Must(BeAValidUrl)
            .When(x => !string.IsNullOrEmpty(x.ImageUrl))
            .WithMessage("ImageUrl must be a valid URL");
    }

    private static bool BeAValidUrl(string? url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var result)
               && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }
}

// UpdateAttractionCommandValidator.cs
public class UpdateAttractionCommandValidator : AbstractValidator<UpdateAttractionCommand>
{
    public UpdateAttractionCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(x => x.Description)
            .MaximumLength(5000)
            .When(x => x.Description != null);

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90m, 90m);

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180m, 180m);

        RuleFor(x => x.EstimatedDuration)
            .GreaterThan(0)
            .When(x => x.EstimatedDuration.HasValue);

        RuleFor(x => x.ImageUrl)
            .Must(BeAValidUrl)
            .When(x => !string.IsNullOrEmpty(x.ImageUrl))
            .WithMessage("ImageUrl must be a valid URL");
    }

    private static bool BeAValidUrl(string? url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var result)
               && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }
}
```

---

## 4. Szczegóły odpowiedzi

### GET /api/attractions - 200 OK

```json
{
  "items": [
    {
      "id": "uuid",
      "locationId": "uuid",
      "name": "Acropolis of Athens",
      "description": "Ancient citadel...",
      "latitude": 37.9715323,
      "longitude": 23.7257492,
      "rating": 4.8,
      "reviewCount": 95420,
      "estimatedDuration": 180,
      "imageUrl": "https://example.com/acropolis.jpg",
      "isVerified": true,
      "createdByUserId": null
    }
  ],
  "pagination": {
    "page": 1,
    "pageSize": 20,
    "totalItems": 45,
    "totalPages": 3,
    "hasNextPage": true,
    "hasPreviousPage": false
  }
}
```

### GET /api/attractions/{id} - 200 OK

```json
{
  "id": "uuid",
  "locationId": "uuid",
  "location": {
    "id": "uuid",
    "name": "Athens",
    "country": "Greece"
  },
  "name": "Acropolis of Athens",
  "description": "Ancient citadel...",
  "latitude": 37.9715323,
  "longitude": 23.7257492,
  "rating": 4.8,
  "reviewCount": 95420,
  "estimatedDuration": 180,
  "imageUrl": "https://example.com/acropolis.jpg",
  "isVerified": true,
  "createdByUserId": null,
  "createdAt": "2026-01-01T00:00:00Z",
  "updatedAt": "2026-01-01T00:00:00Z"
}
```

### POST /api/attractions - 201 Created

Struktura jak GET by ID, z `isVerified: false` i `createdByUserId` ustawionym na ID zalogowanego użytkownika.

### PUT /api/attractions/{id} - 200 OK

Struktura jak GET by ID, z zaktualizowanymi polami i nowym `updatedAt`.

### DELETE /api/attractions/{id} - 204 No Content

Brak body w odpowiedzi.

### Kody błędów

| Kod | Typ | Opis |
|-----|-----|------|
| 400 | Bad Request | Błąd walidacji danych wejściowych |
| 401 | Unauthorized | Brak lub nieprawidłowy token autoryzacji |
| 403 | Forbidden | Brak uprawnień (nie jest właścicielem) |
| 404 | Not Found | Zasób nie istnieje |
| 409 | Conflict | Konflikt (atrakcja używana w tripach) |
| 500 | Internal Server Error | Błąd serwera |

---

## 5. Przepływ danych

### GET /api/attractions

```
Request → AttractionEndpoints.GetAttractions()
    → MediatR.Send(GetAttractionsQuery)
    → GetAttractionsQueryValidator (walidacja)
    → GetAttractionsQueryHandler
        → IApplicationDbContext.Attractions
        → Filtrowanie (locationId, search, isVerified)
        → Sortowanie (sortBy, sortOrder)
        → Paginacja (Skip, Take)
        → Mapowanie do AttractionListItemDto
    → PaginatedResponse<AttractionListItemDto>
```

### GET /api/attractions/{id}

```
Request → AttractionEndpoints.GetAttractionById(id)
    → MediatR.Send(GetAttractionByIdQuery)
    → GetAttractionByIdQueryHandler
        → IApplicationDbContext.Attractions
            .Include(Location)
            .FirstOrDefaultAsync(id)
        → NotFoundException jeśli null
        → Mapowanie do AttractionDto
    → AttractionDto
```

### POST /api/attractions

```
Request → [Authorize] AttractionEndpoints.CreateAttraction()
    → ICurrentUserService.UserId (pobranie ID użytkownika)
    → MediatR.Send(CreateAttractionCommand)
    → CreateAttractionCommandValidator (walidacja)
    → CreateAttractionCommandHandler
        → Sprawdzenie czy Location istnieje
        → NotFoundException jeśli nie
        → Utworzenie Attraction entity
        → IApplicationDbContext.Attractions.Add()
        → SaveChangesAsync()
        → Mapowanie do AttractionDto
    → AttractionDto (201 Created)
```

### PUT /api/attractions/{id}

```
Request → [Authorize] AttractionEndpoints.UpdateAttraction(id)
    → ICurrentUserService.UserId
    → MediatR.Send(UpdateAttractionCommand)
    → UpdateAttractionCommandValidator (walidacja)
    → UpdateAttractionCommandHandler
        → Pobranie Attraction z Include(Location)
        → NotFoundException jeśli null
        → ForbiddenAccessException jeśli !IsUserGenerated lub CreatedByUserId != CurrentUserId
        → Aktualizacja pól
        → UpdatedAt = DateTime.UtcNow
        → SaveChangesAsync()
        → Mapowanie do AttractionDto
    → AttractionDto (200 OK)
```

### DELETE /api/attractions/{id}

```
Request → [Authorize] AttractionEndpoints.DeleteAttraction(id)
    → ICurrentUserService.UserId
    → MediatR.Send(DeleteAttractionCommand)
    → DeleteAttractionCommandHandler
        → Pobranie Attraction
        → NotFoundException jeśli null
        → ForbiddenAccessException jeśli !IsUserGenerated lub CreatedByUserId != CurrentUserId
        → Sprawdzenie czy używane w tripach innych użytkowników
        → ConflictException jeśli tak
        → IApplicationDbContext.Attractions.Remove()
        → SaveChangesAsync()
    → 204 No Content
```

---

## 6. Względy bezpieczeństwa

### 6.1. Uwierzytelnianie

- **GET** - publiczne, bez wymagania tokenu
- **POST/PUT/DELETE** - wymagają `[Authorize]` attribute
- Token JWT w nagłówku `Authorization: Bearer {token}`
- Walidacja tokenu przez ASP.NET Core Identity / JWT middleware

### 6.2. Autoryzacja

**PUT /api/attractions/{id}:**
- Tylko właściciel może modyfikować (CreatedByUserId == CurrentUserId)
- Tylko atrakcje user-generated mogą być modyfikowane
- Atrakcje systemowe (isVerified=true) nie mogą być modyfikowane

**DELETE /api/attractions/{id}:**
- Tylko właściciel może usuwać
- Dodatkowa walidacja: atrakcja nie może być używana w tripach innych użytkowników

### 6.3. Walidacja danych wejściowych

- FluentValidation dla wszystkich Commands i Queries
- Automatyczna walidacja przez ValidationBehavior w pipeline MediatR
- Sanityzacja parametru `search` (EF Core automatycznie parametryzuje zapytania)
- Walidacja formatu UUID dla identyfikatorów
- Walidacja zakresów dla latitude/longitude

### 6.4. Ochrona przed atakami

| Atak | Zabezpieczenie |
|------|----------------|
| SQL Injection | Parametryzowane zapytania EF Core |
| XSS | Brak renderowania HTML, JSON API |
| CSRF | Token-based auth (stateless) |
| Mass Assignment | Explicit DTOs/Commands |

---

## 7. Obsługa błędów

### 7.1. Custom Exceptions

**Lokalizacja:** `src/TripPlanner.Application/Common/Exceptions/`

```csharp
// NotFoundException.cs (jeśli nie istnieje)
public class NotFoundException : Exception
{
    public NotFoundException(string name, object key)
        : base($"Entity \"{name}\" ({key}) was not found.")
    {
    }
}

// ForbiddenAccessException.cs
public class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException(string message = "Access denied.")
        : base(message)
    {
    }
}

// ConflictException.cs
public class ConflictException : Exception
{
    public ConflictException(string message)
        : base(message)
    {
    }
}
```

### 7.2. Mapowanie wyjątków w Middleware

Rozszerzenie `ExceptionHandlingMiddleware`:

```csharp
private static (int statusCode, string message) MapException(Exception exception)
{
    return exception switch
    {
        ValidationException ve => (400, FormatValidationErrors(ve)),
        NotFoundException => (404, exception.Message),
        UnauthorizedAccessException => (401, "Unauthorized"),
        ForbiddenAccessException => (403, exception.Message),
        ConflictException => (409, exception.Message),
        _ => (500, "An error occurred while processing your request.")
    };
}
```

### 7.3. Scenariusze błędów

| Endpoint | Scenariusz | Kod | Response |
|----------|------------|-----|----------|
| GET list | Nieprawidłowy sortBy | 400 | `{"errors":{"sortBy":["SortBy must be one of: rating, name, reviewCount"]}}` |
| GET list | pageSize > 100 | 400 | `{"errors":{"pageSize":["PageSize must be between 1 and 100"]}}` |
| GET /{id} | Atrakcja nie istnieje | 404 | `{"message":"Entity \"Attraction\" (id) was not found."}` |
| POST | Location nie istnieje | 404 | `{"message":"Entity \"Location\" (id) was not found."}` |
| POST | Brak tokenu | 401 | `{"message":"Unauthorized"}` |
| PUT | Nie jest właścicielem | 403 | `{"message":"Cannot modify attraction created by another user"}` |
| PUT | Atrakcja systemowa | 403 | `{"message":"Cannot modify verified attractions"}` |
| DELETE | Używane w tripach | 409 | `{"message":"Attraction is used in other users' trips"}` |

---

## 8. Rozważania dotyczące wydajności

### 8.1. Indeksy bazy danych

Wymagane indeksy dla tabeli `attractions`:

```sql
-- Filtrowanie po lokalizacji
CREATE INDEX idx_attractions_location_id ON attractions(location_id);

-- Wyszukiwanie po nazwie (case-insensitive)
CREATE INDEX idx_attractions_name_lower ON attractions(LOWER(name));

-- Sortowanie po ratingu (najczęstsze)
CREATE INDEX idx_attractions_rating_desc ON attractions(rating DESC NULLS LAST);

-- Filtrowanie po właścicielu
CREATE INDEX idx_attractions_created_by_user ON attractions(created_by_user_id) WHERE created_by_user_id IS NOT NULL;

-- Kompozytowy indeks dla typowych zapytań
CREATE INDEX idx_attractions_location_rating ON attractions(location_id, rating DESC NULLS LAST);
```

### 8.2. Optymalizacja zapytań

- Użycie `AsNoTracking()` dla operacji odczytu
- Projekcja bezpośrednio do DTO zamiast mapowania całych entity
- Eager loading tylko wymaganych relacji (`Include(Location)` tylko dla GET by ID)
- Paginacja po stronie bazy danych (Skip/Take)

### 8.3. Caching (opcjonalnie)

Dla często odpytywanych atrakcji systemowych:
- Response caching dla GET endpoints
- Distributed cache dla popularnych lokalizacji

---

## 9. Etapy wdrożenia

### Faza 1: Przygotowanie infrastruktury

#### 1.1. Dodanie kolumny image_url do bazy danych
```
Plik: Nowa migracja EF Core
- Dodanie kolumny image_url VARCHAR(500) NULL do tabeli attractions
```

#### 1.2. Utworzenie/aktualizacja Domain Entity
```
Plik: src/TripPlanner.Domain/Entities/Attraction.cs
- Pełna definicja entity zgodnie z sekcją 3.1
```

#### 1.3. Konfiguracja DbContext
```
Plik: src/TripPlanner.Infrastructure/Persistence/ApplicationDbContext.cs
- Dodanie DbSet<Attraction> Attractions
- Konfiguracja w OnModelCreating (constraints, indexes)
```

#### 1.4. Dodanie Custom Exceptions
```
Pliki: src/TripPlanner.Application/Common/Exceptions/
- ForbiddenAccessException.cs
- ConflictException.cs
- Aktualizacja ExceptionHandlingMiddleware
```

### Faza 2: Implementacja warstwy Application (Queries)

#### 2.1. Utworzenie DTOs
```
Pliki: src/TripPlanner.Application/Attractions/DTOs/
- AttractionListItemDto.cs
- AttractionDto.cs
- LocationSummaryDto.cs (jeśli nie istnieje w Common)
```

#### 2.2. GetAttractionsQuery
```
Pliki: src/TripPlanner.Application/Attractions/Queries/GetAttractions/
- GetAttractionsQuery.cs
- GetAttractionsQueryValidator.cs
- GetAttractionsQueryHandler.cs
```

#### 2.3. GetAttractionByIdQuery
```
Pliki: src/TripPlanner.Application/Attractions/Queries/GetAttractionById/
- GetAttractionByIdQuery.cs
- GetAttractionByIdQueryHandler.cs
```

### Faza 3: Implementacja warstwy Application (Commands)

#### 3.1. CreateAttractionCommand
```
Pliki: src/TripPlanner.Application/Attractions/Commands/CreateAttraction/
- CreateAttractionCommand.cs
- CreateAttractionCommandValidator.cs
- CreateAttractionCommandHandler.cs
```

#### 3.2. UpdateAttractionCommand
```
Pliki: src/TripPlanner.Application/Attractions/Commands/UpdateAttraction/
- UpdateAttractionCommand.cs
- UpdateAttractionCommandValidator.cs
- UpdateAttractionCommandHandler.cs
```

#### 3.3. DeleteAttractionCommand
```
Pliki: src/TripPlanner.Application/Attractions/Commands/DeleteAttraction/
- DeleteAttractionCommand.cs
- DeleteAttractionCommandHandler.cs
```

### Faza 4: Implementacja warstwy WebApi

#### 4.1. Utworzenie AttractionEndpoints
```
Plik: src/TripPlanner.WebApi/Endpoints/AttractionEndpoints.cs
- MapAttractionEndpoints() extension method
- GET /api/attractions
- GET /api/attractions/{id}
- POST /api/attractions [Authorize]
- PUT /api/attractions/{id} [Authorize]
- DELETE /api/attractions/{id} [Authorize]
```

#### 4.2. Rejestracja endpointów
```
Plik: src/TripPlanner.WebApi/Program.cs
- app.MapAttractionEndpoints()
```

### Faza 5: Testy jednostkowe

#### 5.1. Testy walidatorów
```
Pliki: tests/TripPlanner.Application.UnitTests/Attractions/
- GetAttractionsQueryValidatorTests.cs
- CreateAttractionCommandValidatorTests.cs
- UpdateAttractionCommandValidatorTests.cs
```

#### 5.2. Testy handlerów (opcjonalnie z mockami)
```
Pliki: tests/TripPlanner.Application.UnitTests/Attractions/
- GetAttractionsQueryHandlerTests.cs
- CreateAttractionCommandHandlerTests.cs
```

### Faza 6: Testy integracyjne

#### 6.1. Przygotowanie danych testowych
```
Plik: tests/TripPlanner.WebApi.IntegrationTests/CustomWebApplicationFactory.cs
- Dodanie seed data dla Attractions w SeedTestData()
```

#### 6.2. Testy endpointów
```
Plik: tests/TripPlanner.WebApi.IntegrationTests/Endpoints/AttractionEndpointsTests.cs
- Testy GET /api/attractions (paginacja, filtry, sortowanie)
- Testy GET /api/attractions/{id}
- Testy POST /api/attractions (z auth)
- Testy PUT /api/attractions/{id} (ownership)
- Testy DELETE /api/attractions/{id} (ownership, conflict)
```

### Faza 7: Dokumentacja i finalizacja

#### 7.1. Weryfikacja OpenAPI
- Sprawdzenie generowanej dokumentacji Swagger
- Dodanie przykładów responses

#### 7.2. Code Review
- Przegląd zgodności z architekturą
- Sprawdzenie pokrycia testami

---

## 10. Struktura plików

```
src/
├── TripPlanner.Domain/
│   └── Entities/
│       └── Attraction.cs
│
├── TripPlanner.Application/
│   ├── Common/
│   │   └── Exceptions/
│   │       ├── ForbiddenAccessException.cs
│   │       └── ConflictException.cs
│   │
│   └── Attractions/
│       ├── DTOs/
│       │   ├── AttractionListItemDto.cs
│       │   ├── AttractionDto.cs
│       │   └── LocationSummaryDto.cs
│       │
│       ├── Queries/
│       │   ├── GetAttractions/
│       │   │   ├── GetAttractionsQuery.cs
│       │   │   ├── GetAttractionsQueryValidator.cs
│       │   │   └── GetAttractionsQueryHandler.cs
│       │   │
│       │   └── GetAttractionById/
│       │       ├── GetAttractionByIdQuery.cs
│       │       └── GetAttractionByIdQueryHandler.cs
│       │
│       └── Commands/
│           ├── CreateAttraction/
│           │   ├── CreateAttractionCommand.cs
│           │   ├── CreateAttractionCommandValidator.cs
│           │   └── CreateAttractionCommandHandler.cs
│           │
│           ├── UpdateAttraction/
│           │   ├── UpdateAttractionCommand.cs
│           │   ├── UpdateAttractionCommandValidator.cs
│           │   └── UpdateAttractionCommandHandler.cs
│           │
│           └── DeleteAttraction/
│               ├── DeleteAttractionCommand.cs
│               └── DeleteAttractionCommandHandler.cs
│
├── TripPlanner.Infrastructure/
│   └── Persistence/
│       ├── ApplicationDbContext.cs (aktualizacja)
│       └── Configurations/
│           └── AttractionConfiguration.cs
│
└── TripPlanner.WebApi/
    └── Endpoints/
        └── AttractionEndpoints.cs

tests/
├── TripPlanner.Application.UnitTests/
│   └── Attractions/
│       ├── Queries/
│       │   └── GetAttractionsQueryValidatorTests.cs
│       └── Commands/
│           ├── CreateAttractionCommandValidatorTests.cs
│           └── UpdateAttractionCommandValidatorTests.cs
│
└── TripPlanner.WebApi.IntegrationTests/
    └── Endpoints/
        └── AttractionEndpointsTests.cs
```

---

## 11. Uwagi implementacyjne

### Mapowanie pól API ↔ DB

| API Field | DB Column | Uwagi |
|-----------|-----------|-------|
| isVerified | is_user_generated | `isVerified = !is_user_generated` |
| estimatedDuration | estimated_duration_minutes | Nazwa różni się |
| rating | rating | NULL dla user-generated |
| reviewCount | review_count | NULL/0 dla user-generated |

### Wymagana migracja DB

Przed implementacją należy dodać kolumnę `image_url`:

```sql
ALTER TABLE attractions ADD COLUMN image_url VARCHAR(500);
```

### ICurrentUserService

Upewnić się, że interfejs `ICurrentUserService` istnieje i jest zarejestrowany:

```csharp
public interface ICurrentUserService
{
    Guid? UserId { get; }
    bool IsAuthenticated { get; }
}
```
