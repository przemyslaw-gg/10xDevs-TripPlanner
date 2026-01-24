# API Endpoint Implementation Plan: Authentication

## 1. Przegląd punktu końcowego

Moduł Authentication (`/api/auth`) odpowiada za zarządzanie tożsamością użytkowników w systemie TripPlanner. Implementuje OAuth 2.0 Resource Owner Password Credentials flow z JWT access tokens i refresh tokens.

**Endpointy:**
| Endpoint | Metoda | Autoryzacja | Opis |
|----------|--------|-------------|------|
| `/api/auth/register` | POST | Publiczny | Rejestracja nowego użytkownika |
| `/api/auth/login` | POST | Publiczny | Logowanie i wydanie tokenów |
| `/api/auth/refresh` | POST | Publiczny | Odświeżenie access token |
| `/api/auth/logout` | POST | Bearer Token | Unieważnienie refresh token |
| `/api/auth/me` | GET | Bearer Token | Dane zalogowanego użytkownika |

## 2. Szczegóły żądań

### 2.1 POST /api/auth/register
- **URL:** `/api/auth/register`
- **Autoryzacja:** Brak (publiczny)
- **Request Body:**
```json
{
  "email": "string (wymagane, max 255, format email)",
  "password": "string (wymagane, min 8, complexity)",
  "displayName": "string (opcjonalne, max 100)"
}
```

### 2.2 POST /api/auth/login
- **URL:** `/api/auth/login`
- **Autoryzacja:** Brak (publiczny)
- **Request Body:**
```json
{
  "email": "string (wymagane)",
  "password": "string (wymagane)"
}
```

### 2.3 POST /api/auth/refresh
- **URL:** `/api/auth/refresh`
- **Autoryzacja:** Brak (publiczny)
- **Request Body:**
```json
{
  "refreshToken": "string (wymagane)"
}
```

### 2.4 POST /api/auth/logout
- **URL:** `/api/auth/logout`
- **Autoryzacja:** `Authorization: Bearer {access_token}`
- **Request Body:**
```json
{
  "refreshToken": "string (wymagane)"
}
```

### 2.5 GET /api/auth/me
- **URL:** `/api/auth/me`
- **Autoryzacja:** `Authorization: Bearer {access_token}`
- **Request Body:** Brak

## 3. Wykorzystywane typy

### 3.1 DTOs

```csharp
// Odpowiedź rejestracji
public record RegisteredUserDto(
    Guid Id,
    string Email,
    string? DisplayName,
    DateTime CreatedAt
);

// Dane użytkownika w odpowiedzi logowania
public record UserInfoDto(
    Guid Id,
    string Email,
    string? DisplayName
);

// Pełna odpowiedź logowania
public record AuthResponseDto(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn,
    string TokenType,
    UserInfoDto User
);

// Odpowiedź refresh (bez user)
public record TokenResponseDto(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn,
    string TokenType
);

// Pełne dane użytkownika dla /me
public record CurrentUserDto(
    Guid Id,
    string Email,
    string? DisplayName,
    bool EmailVerified,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
```

### 3.2 Commands

```csharp
public record RegisterUserCommand(
    string Email,
    string Password,
    string? DisplayName
) : IRequest<RegisteredUserDto>;

public record LoginCommand(
    string Email,
    string Password
) : IRequest<AuthResponseDto>;

public record RefreshTokenCommand(
    string RefreshToken
) : IRequest<TokenResponseDto>;

public record LogoutCommand(
    string RefreshToken
) : IRequest<Unit>;
```

### 3.3 Queries

```csharp
public record GetCurrentUserQuery : IRequest<CurrentUserDto>;
```

### 3.4 Interfejsy serwisów

```csharp
public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    string HashRefreshToken(string token);
    bool ValidateRefreshTokenHash(string token, string hash);
}

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}
```

## 4. Szczegóły odpowiedzi

### 4.1 POST /api/auth/register - 201 Created
```json
{
  "id": "uuid",
  "email": "user@example.com",
  "displayName": "John Doe",
  "createdAt": "2026-01-22T10:00:00Z"
}
```

### 4.2 POST /api/auth/login - 200 OK
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "dGhpcyBpcyBhIHJlZnJlc2ggdG9rZW4...",
  "expiresIn": 1800,
  "tokenType": "Bearer",
  "user": {
    "id": "uuid",
    "email": "user@example.com",
    "displayName": "John Doe"
  }
}
```

### 4.3 POST /api/auth/refresh - 200 OK
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "bmV3IHJlZnJlc2ggdG9rZW4...",
  "expiresIn": 1800,
  "tokenType": "Bearer"
}
```

### 4.4 POST /api/auth/logout - 204 No Content

### 4.5 GET /api/auth/me - 200 OK
```json
{
  "id": "uuid",
  "email": "user@example.com",
  "displayName": "John Doe",
  "emailVerified": false,
  "createdAt": "2026-01-22T10:00:00Z",
  "updatedAt": "2026-01-22T10:00:00Z"
}
```

## 5. Przepływ danych

### 5.1 Rejestracja

```
┌─────────┐      ┌──────────────┐      ┌──────────────┐      ┌──────────┐
│ Request │─────>│ Validation   │─────>│ Handler      │─────>│ Database │
│         │      │ (FluentVal)  │      │              │      │ (users)  │
└─────────┘      └──────────────┘      └──────────────┘      └──────────┘
                                              │
                                              ▼
                                       ┌──────────────┐
                                       │ PasswordHash │
                                       │ (BCrypt)     │
                                       └──────────────┘
```

1. Walidacja danych wejściowych (email format, password complexity)
2. Sprawdzenie czy email nie istnieje (409 Conflict)
3. Hashowanie hasła BCrypt
4. Utworzenie użytkownika w bazie
5. Zwrócenie RegisteredUserDto

### 5.2 Logowanie

```
┌─────────┐      ┌──────────────┐      ┌──────────────┐
│ Request │─────>│ Handler      │─────>│ Database     │
│         │      │              │      │ (users)      │
└─────────┘      └──────────────┘      └──────────────┘
                        │                     │
                        ▼                     │
                 ┌──────────────┐             │
                 │ PasswordHash │<────────────┘
                 │ Verify       │
                 └──────────────┘
                        │
                        ▼
                 ┌──────────────┐      ┌──────────────┐
                 │ TokenService │─────>│ Database     │
                 │ Generate     │      │ (refresh_    │
                 └──────────────┘      │  tokens)     │
                                       └──────────────┘
```

1. Pobranie użytkownika po email (401 jeśli nie istnieje)
2. Weryfikacja hasła BCrypt (401 jeśli niepoprawne)
3. Generowanie JWT access token
4. Generowanie refresh token + zapis hash w bazie
5. Zwrócenie AuthResponseDto

### 5.3 Refresh Token

```
┌─────────┐      ┌──────────────┐      ┌──────────────┐
│ Request │─────>│ Handler      │─────>│ Database     │
│         │      │              │      │ (refresh_    │
└─────────┘      └──────────────┘      │  tokens)     │
                        │              └──────────────┘
                        │                     │
                        ▼                     │
                 ┌──────────────┐             │
                 │ Validate &   │<────────────┘
                 │ Revoke Old   │
                 └──────────────┘
                        │
                        ▼
                 ┌──────────────┐
                 │ Generate New │
                 │ Token Pair   │
                 └──────────────┘
```

1. Hashowanie otrzymanego refresh token
2. Wyszukanie w bazie po hash (401 jeśli nie znaleziono)
3. Sprawdzenie expires_at i revoked_at (401 jeśli nieważny)
4. Unieważnienie starego tokenu (revoked_at = now)
5. Wygenerowanie nowej pary tokenów
6. Zapis nowego refresh token hash
7. Zwrócenie TokenResponseDto

### 5.4 Logout

1. Walidacja access token (middleware)
2. Hashowanie refresh token z body
3. Unieważnienie tokenu w bazie (revoked_at = now)
4. Zwrócenie 204 No Content

### 5.5 Get Current User

1. Walidacja access token (middleware)
2. Pobranie UserId z ICurrentUserService
3. Pobranie użytkownika z bazy
4. Zwrócenie CurrentUserDto

## 6. Względy bezpieczeństwa

### 6.1 Hashowanie haseł

```csharp
// Użycie BCrypt z cost factor 12
public class BCryptPasswordHasher : IPasswordHasher
{
    private const int WorkFactor = 12;

    public string Hash(string password) =>
        BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);

    public bool Verify(string password, string hash) =>
        BCrypt.Net.BCrypt.Verify(password, hash);
}
```

### 6.2 JWT Configuration

```csharp
// appsettings.json
{
  "Jwt": {
    "Secret": "min-256-bit-secret-key-here",
    "Issuer": "TripPlanner",
    "Audience": "TripPlanner",
    "AccessTokenExpirationMinutes": 30,
    "RefreshTokenExpirationDays": 7
  }
}
```

### 6.3 Refresh Token Security

- Generowanie: `RandomNumberGenerator.GetBytes(32)` → Base64
- Przechowywanie: SHA256 hash w bazie
- Rotacja: Nowy token przy każdym refresh
- Unieważnianie: Soft delete przez revoked_at

### 6.4 Rate Limiting

```csharp
// Na endpointach register i login
.RequireRateLimiting("auth")

// Konfiguracja
services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("auth", opt =>
    {
        opt.PermitLimit = 5;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0;
    });
});
```

### 6.5 Timing-Safe Comparisons

- Weryfikacja hasła przez BCrypt (inherently timing-safe)
- Porównanie hash refresh token przez constant-time comparison

## 7. Obsługa błędów

| Endpoint | Kod | Warunek | Wyjątek |
|----------|-----|---------|---------|
| Register | 400 | Błąd walidacji | `ValidationException` |
| Register | 409 | Email istnieje | `ConflictException` |
| Login | 400 | Błąd walidacji | `ValidationException` |
| Login | 401 | Nieprawidłowe dane | `UnauthorizedAccessException` |
| Refresh | 400 | Brak tokenu | `ValidationException` |
| Refresh | 401 | Token nieważny | `UnauthorizedAccessException` |
| Logout | 401 | Brak autoryzacji | `UnauthorizedAccessException` |
| Me | 401 | Brak autoryzacji | `UnauthorizedAccessException` |
| Me | 404 | User nie istnieje | `NotFoundException` |

### Format błędu 401:
```json
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized",
  "status": 401,
  "detail": "Invalid credentials"
}
```

### Format błędu 409:
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.8",
  "title": "Conflict",
  "status": 409,
  "detail": "Email is already registered"
}
```

## 8. Wydajność

### 8.1 Optymalizacje

- **BCrypt cost factor 12** - balans między bezpieczeństwem a wydajnością (~250ms)
- **Indeks na email** - szybkie wyszukiwanie przy login
- **Indeks na token_hash** - szybka walidacja refresh token
- **JWT stateless** - brak zapytań do bazy przy każdym request

### 8.2 Cleanup wygasłych tokenów

```sql
-- Okresowe czyszczenie (pg_cron lub background job)
DELETE FROM refresh_tokens
WHERE expires_at < NOW() OR revoked_at IS NOT NULL;
```

## 9. Kroki implementacji

### Krok 1: Serwisy (Application/Common/Services/)

1. Utwórz `IPasswordHasher.cs` i `ITokenService.cs` w Interfaces
2. Zaimplementuj `BCryptPasswordHasher.cs` w Infrastructure
3. Zaimplementuj `JwtTokenService.cs` w Infrastructure
4. Zarejestruj serwisy w DI

### Krok 2: DTOs (Application/Auth/DTOs/)

1. Utwórz `RegisteredUserDto.cs`
2. Utwórz `UserInfoDto.cs`
3. Utwórz `AuthResponseDto.cs`
4. Utwórz `TokenResponseDto.cs`
5. Utwórz `CurrentUserDto.cs`

### Krok 3: Commands (Application/Auth/Commands/)

**RegisterUser/**
1. `RegisterUserCommand.cs`
2. `RegisterUserCommandHandler.cs`
3. `RegisterUserCommandValidator.cs`

**Login/**
1. `LoginCommand.cs`
2. `LoginCommandHandler.cs`
3. `LoginCommandValidator.cs`

**RefreshToken/**
1. `RefreshTokenCommand.cs`
2. `RefreshTokenCommandHandler.cs`
3. `RefreshTokenCommandValidator.cs`

**Logout/**
1. `LogoutCommand.cs`
2. `LogoutCommandHandler.cs`
3. `LogoutCommandValidator.cs`

### Krok 4: Query (Application/Auth/Queries/)

**GetCurrentUser/**
1. `GetCurrentUserQuery.cs`
2. `GetCurrentUserQueryHandler.cs`

### Krok 5: JWT Configuration (WebApi)

1. Dodaj konfigurację JWT w `appsettings.json`
2. Skonfiguruj JWT Authentication w `ServiceCollectionExtensions.cs`
3. Dodaj middleware autoryzacji

### Krok 6: Endpoints (WebApi/Endpoints/)

1. Utwórz `AuthEndpoints.cs`
2. Zdefiniuj wszystkie 5 endpointów
3. Zarejestruj w `WebApplicationExtensions.MapEndpoints()`

### Krok 7: Rate Limiting

1. Skonfiguruj rate limiter dla endpointów auth
2. Zastosuj na register i login

### Krok 8: Testy

1. Testy jednostkowe dla handlerów
2. Testy walidatorów
3. Testy TokenService i PasswordHasher
4. Testy integracyjne endpointów

---

## 10. Struktura plików do utworzenia

```
src/TripPlanner.Application/
├── Common/
│   └── Interfaces/
│       ├── IPasswordHasher.cs
│       └── ITokenService.cs
└── Auth/
    ├── DTOs/
    │   ├── RegisteredUserDto.cs
    │   ├── UserInfoDto.cs
    │   ├── AuthResponseDto.cs
    │   ├── TokenResponseDto.cs
    │   └── CurrentUserDto.cs
    ├── Commands/
    │   ├── RegisterUser/
    │   │   ├── RegisterUserCommand.cs
    │   │   ├── RegisterUserCommandHandler.cs
    │   │   └── RegisterUserCommandValidator.cs
    │   ├── Login/
    │   │   ├── LoginCommand.cs
    │   │   ├── LoginCommandHandler.cs
    │   │   └── LoginCommandValidator.cs
    │   ├── RefreshToken/
    │   │   ├── RefreshTokenCommand.cs
    │   │   ├── RefreshTokenCommandHandler.cs
    │   │   └── RefreshTokenCommandValidator.cs
    │   └── Logout/
    │       ├── LogoutCommand.cs
    │       ├── LogoutCommandHandler.cs
    │       └── LogoutCommandValidator.cs
    └── Queries/
        └── GetCurrentUser/
            ├── GetCurrentUserQuery.cs
            └── GetCurrentUserQueryHandler.cs

src/TripPlanner.Infrastructure/
└── Services/
    ├── BCryptPasswordHasher.cs
    └── JwtTokenService.cs

src/TripPlanner.WebApi/
└── Endpoints/
    └── AuthEndpoints.cs
```

## 11. Konfiguracja JWT (przykład)

```csharp
// appsettings.json
{
  "Jwt": {
    "Secret": "your-256-bit-secret-key-minimum-32-characters",
    "Issuer": "TripPlanner",
    "Audience": "TripPlanner",
    "AccessTokenExpirationMinutes": 30,
    "RefreshTokenExpirationDays": 7
  }
}

// ServiceCollectionExtensions.cs
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidAudience = configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!))
        };
    });

services.AddAuthorization();
```

## 12. Przykład implementacji TokenService

```csharp
public class JwtTokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateAccessToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("name", user.DisplayName ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(
            int.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"]!));

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    public string HashRefreshToken(string token)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(token);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    public bool ValidateRefreshTokenHash(string token, string hash)
    {
        var computedHash = HashRefreshToken(token);
        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(computedHash),
            Encoding.UTF8.GetBytes(hash));
    }
}
```
