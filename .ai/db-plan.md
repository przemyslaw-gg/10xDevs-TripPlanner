# Schemat bazy danych TripPlanner MVP

## 1. Tabele

### 1.1. `users`

Użytkownicy systemu z autoryzacją OAuth 2.0 (Resource Owner Password Credentials).

```sql
CREATE TABLE users (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    email VARCHAR(255) NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    display_name VARCHAR(100),
    email_verified BOOLEAN NOT NULL DEFAULT FALSE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),

    CONSTRAINT uq_users_email UNIQUE (email)
);
```

| Kolumna | Typ | Ograniczenia | Opis |
|---------|-----|--------------|------|
| `id` | UUID | PK, DEFAULT gen_random_uuid() | Identyfikator użytkownika |
| `email` | VARCHAR(255) | NOT NULL, UNIQUE | Adres email (login) |
| `password_hash` | VARCHAR(255) | NOT NULL | Hash hasła (BCrypt/Argon2) |
| `display_name` | VARCHAR(100) | — | Wyświetlana nazwa użytkownika |
| `email_verified` | BOOLEAN | NOT NULL, DEFAULT FALSE | Czy email został zweryfikowany |
| `created_at` | TIMESTAMPTZ | NOT NULL, DEFAULT NOW() | Data utworzenia |
| `updated_at` | TIMESTAMPTZ | NOT NULL, DEFAULT NOW() | Data ostatniej modyfikacji |

---

### 1.2. `refresh_tokens`

Tokeny odświeżania dla OAuth 2.0 (access tokeny są JWT, stateless).

```sql
CREATE TABLE refresh_tokens (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    token_hash VARCHAR(255) NOT NULL,
    expires_at TIMESTAMPTZ NOT NULL,
    revoked_at TIMESTAMPTZ,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),

    CONSTRAINT uq_refresh_tokens_hash UNIQUE (token_hash)
);
```

| Kolumna | Typ | Ograniczenia | Opis |
|---------|-----|--------------|------|
| `id` | UUID | PK, DEFAULT gen_random_uuid() | Identyfikator tokenu |
| `user_id` | UUID | FK → users, NOT NULL, ON DELETE CASCADE | Właściciel tokenu |
| `token_hash` | VARCHAR(255) | NOT NULL, UNIQUE | Hash tokenu (SHA256) |
| `expires_at` | TIMESTAMPTZ | NOT NULL | Data wygaśnięcia |
| `revoked_at` | TIMESTAMPTZ | — | Data unieważnienia (NULL = aktywny) |
| `created_at` | TIMESTAMPTZ | NOT NULL, DEFAULT NOW() | Data utworzenia |

**Logika biznesowa:**
- Access token: JWT, krótki czas życia (15-30 min), nie przechowywany w bazie
- Refresh token: długi czas życia (7-30 dni), hash przechowywany w bazie
- Przy wylogowaniu: ustawienie `revoked_at`
- Walidacja: sprawdzenie `expires_at` i `revoked_at IS NULL`

---

### 1.3. `locations`

Lokalizacje turystyczne (miasta, regiony).

```sql
CREATE TABLE locations (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(100) NOT NULL,
    country VARCHAR(100) NOT NULL,
    timezone VARCHAR(50),
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
```

| Kolumna | Typ | Ograniczenia | Opis |
|---------|-----|--------------|------|
| `id` | UUID | PK, DEFAULT gen_random_uuid() | Identyfikator lokalizacji |
| `name` | VARCHAR(100) | NOT NULL | Nazwa miasta/regionu |
| `country` | VARCHAR(100) | NOT NULL | Kraj |
| `timezone` | VARCHAR(50) | — | Strefa czasowa (np. 'Europe/Athens') |
| `created_at` | TIMESTAMPTZ | NOT NULL, DEFAULT NOW() | Data utworzenia |
| `updated_at` | TIMESTAMPTZ | NOT NULL, DEFAULT NOW() | Data ostatniej modyfikacji |

---

### 1.4. `attractions`

Atrakcje turystyczne przypisane do lokalizacji.

```sql
CREATE TABLE attractions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    location_id UUID NOT NULL REFERENCES locations(id) ON DELETE RESTRICT,
    name VARCHAR(200) NOT NULL,
    description TEXT,
    latitude DECIMAL(10, 7) NOT NULL,
    longitude DECIMAL(10, 7) NOT NULL,
    rating DECIMAL(2, 1),
    review_count INTEGER,
    estimated_duration INTEGER,
    image_url VARCHAR(500),
    created_by_user_id UUID REFERENCES users(id) ON DELETE SET NULL,
    is_verified BOOLEAN NOT NULL DEFAULT FALSE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
```

| Kolumna | Typ | Ograniczenia | Opis |
|---------|-----|--------------|------|
| `id` | UUID | PK, DEFAULT gen_random_uuid() | Identyfikator atrakcji |
| `location_id` | UUID | FK → locations, NOT NULL, ON DELETE RESTRICT | Lokalizacja atrakcji |
| `name` | VARCHAR(200) | NOT NULL | Nazwa atrakcji |
| `description` | TEXT | — | Opis atrakcji |
| `latitude` | DECIMAL(10,7) | NOT NULL | Szerokość geograficzna |
| `longitude` | DECIMAL(10,7) | NOT NULL | Długość geograficzna |
| `rating` | DECIMAL(2,1) | — | Ocena (dane statyczne, np. 4.5) |
| `review_count` | INTEGER | — | Liczba opinii (dane statyczne) |
| `estimated_duration` | INTEGER | — | Szacowany czas zwiedzania w minutach |
| `image_url` | VARCHAR(500) | — | URL do zdjęcia atrakcji |
| `created_by_user_id` | UUID | FK → users, ON DELETE SET NULL | Autor (NULL = atrakcja systemowa) |
| `is_verified` | BOOLEAN | NOT NULL, DEFAULT FALSE | Czy atrakcja zweryfikowana |
| `created_at` | TIMESTAMPTZ | NOT NULL, DEFAULT NOW() | Data utworzenia |
| `updated_at` | TIMESTAMPTZ | NOT NULL, DEFAULT NOW() | Data ostatniej modyfikacji |

**Logika biznesowa:**
- Atrakcje systemowe: `created_by_user_id = NULL`, `is_verified = TRUE`
- Atrakcje użytkowników: `created_by_user_id` ustawione, `is_verified = FALSE`

---

### 1.5. `trips`

Plany wycieczek tworzonych przez użytkowników.

```sql
CREATE TABLE trips (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    owner_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    name VARCHAR(100) NOT NULL,
    location_id UUID REFERENCES locations(id) ON DELETE RESTRICT,
    is_public BOOLEAN NOT NULL DEFAULT FALSE,
    daily_hours INTEGER NOT NULL DEFAULT 8,
    max_extension_hours INTEGER NOT NULL DEFAULT 2,
    start_time TIME NOT NULL DEFAULT '09:00',
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
```

| Kolumna | Typ | Ograniczenia | Opis |
|---------|-----|--------------|------|
| `id` | UUID | PK, DEFAULT gen_random_uuid() | Identyfikator planu |
| `owner_id` | UUID | FK → users, NOT NULL, ON DELETE CASCADE | Właściciel planu |
| `name` | VARCHAR(100) | NOT NULL | Nazwa planu (np. "Ateny 2026") |
| `location_id` | UUID | FK → locations, ON DELETE RESTRICT | Główna lokalizacja wycieczki |
| `is_public` | BOOLEAN | NOT NULL, DEFAULT FALSE | Czy plan jest publiczny |
| `daily_hours` | INTEGER | NOT NULL, DEFAULT 8 | Godziny zwiedzania dziennie |
| `max_extension_hours` | INTEGER | NOT NULL, DEFAULT 2 | Maks. wydłużenie dnia |
| `start_time` | TIME | NOT NULL, DEFAULT '09:00' | Godzina rozpoczęcia zwiedzania |
| `created_at` | TIMESTAMPTZ | NOT NULL, DEFAULT NOW() | Data utworzenia |
| `updated_at` | TIMESTAMPTZ | NOT NULL, DEFAULT NOW() | Data ostatniej modyfikacji |

---

### 1.6. `trip_attractions`

Tabela pośrednia łącząca plany z atrakcjami, zawierająca harmonogram.

```sql
CREATE TABLE trip_attractions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    trip_id UUID NOT NULL REFERENCES trips(id) ON DELETE CASCADE,
    attraction_id UUID NOT NULL REFERENCES attractions(id) ON DELETE RESTRICT,
    day_number INTEGER NOT NULL,
    order_index INTEGER NOT NULL,
    planned_start_time TIME,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),

    CONSTRAINT uq_trip_attraction UNIQUE (trip_id, attraction_id),
    CONSTRAINT chk_day_number_positive CHECK (day_number > 0),
    CONSTRAINT chk_order_index_positive CHECK (order_index > 0)
);
```

| Kolumna | Typ | Ograniczenia | Opis |
|---------|-----|--------------|------|
| `id` | UUID | PK, DEFAULT gen_random_uuid() | Identyfikator rekordu |
| `trip_id` | UUID | FK → trips, NOT NULL, ON DELETE CASCADE | Plan wycieczki |
| `attraction_id` | UUID | FK → attractions, NOT NULL, ON DELETE RESTRICT | Atrakcja |
| `day_number` | INTEGER | NOT NULL, CHECK > 0 | Numer dnia (1, 2, 3...) |
| `order_index` | INTEGER | NOT NULL, CHECK > 0 | Kolejność w ramach dnia (1, 2, 3...) |
| `planned_start_time` | TIME | — | Planowana godzina rozpoczęcia |
| `created_at` | TIMESTAMPTZ | NOT NULL, DEFAULT NOW() | Data utworzenia |
| `updated_at` | TIMESTAMPTZ | NOT NULL, DEFAULT NOW() | Data ostatniej modyfikacji |

**Ograniczenia:**
- `UNIQUE (trip_id, attraction_id)` — każda atrakcja może być tylko raz w planie
- `CHECK (day_number > 0)` — numer dnia musi być dodatni
- `CHECK (order_index > 0)` — kolejność musi być dodatnia

---

## 2. Relacje między tabelami

```
users
  │
  ├──1:N──> refresh_tokens
  │
  ├──1:N──> trips ──N:M──> attractions
  │            │              (przez trip_attractions)
  │            │
  │            └──N:1──> locations
  │
  └──1:N──> attractions (created_by_user_id, nullable)
                │
                └──N:1──> locations
```

| Relacja | Typ | Opis |
|---------|-----|------|
| `users` → `refresh_tokens` | 1:N | Użytkownik może mieć wiele tokenów (historycznych) |
| `users` → `trips` | 1:N | Użytkownik może mieć wiele planów |
| `users` → `attractions` | 1:N | Użytkownik może utworzyć wiele atrakcji |
| `locations` → `attractions` | 1:N | Lokalizacja może mieć wiele atrakcji |
| `locations` → `trips` | 1:N | Lokalizacja może być celem wielu planów |
| `trips` ↔ `attractions` | N:M | Plan zawiera wiele atrakcji, atrakcja może być w wielu planach |

**Zasady usuwania (ON DELETE):**

| Źródło | Cel | Akcja | Uzasadnienie |
|--------|-----|-------|--------------|
| `users` | `refresh_tokens` | CASCADE | Usunięcie konta usuwa tokeny |
| `users` | `trips` | CASCADE | Usunięcie konta usuwa plany użytkownika |
| `users` | `attractions` | SET NULL | Usunięcie konta nie usuwa atrakcji |
| `locations` | `attractions` | RESTRICT | Nie można usunąć lokalizacji z atrakcjami |
| `locations` | `trips` | RESTRICT | Nie można usunąć lokalizacji z planami |
| `trips` | `trip_attractions` | CASCADE | Usunięcie planu usuwa powiązania |
| `attractions` | `trip_attractions` | RESTRICT | Aplikacja sprawdza przed usunięciem |

---

## 3. Indeksy

```sql
-- users
CREATE INDEX idx_users_email ON users(email);

-- refresh_tokens
CREATE INDEX idx_refresh_tokens_user ON refresh_tokens(user_id);
CREATE INDEX idx_refresh_tokens_expires ON refresh_tokens(expires_at)
    WHERE revoked_at IS NULL;

-- locations
CREATE INDEX idx_locations_name_country ON locations(name, country);

-- attractions
CREATE INDEX idx_attractions_location_rating ON attractions(location_id, rating DESC);
CREATE INDEX idx_attractions_name ON attractions(name);
CREATE INDEX idx_attractions_created_by ON attractions(created_by_user_id)
    WHERE created_by_user_id IS NOT NULL;

-- trips
CREATE INDEX idx_trips_owner_created ON trips(owner_id, created_at DESC);
CREATE INDEX idx_trips_location ON trips(location_id) WHERE location_id IS NOT NULL;
CREATE INDEX idx_trips_public ON trips(created_at DESC) WHERE is_public = TRUE;

-- trip_attractions
CREATE INDEX idx_trip_attractions_trip_day_order ON trip_attractions(trip_id, day_number, order_index);
CREATE INDEX idx_trip_attractions_attraction ON trip_attractions(attraction_id);
```

| Tabela | Indeks | Kolumny | Typ | Uzasadnienie |
|--------|--------|---------|-----|--------------|
| `users` | `idx_users_email` | (email) | B-tree | Logowanie po email |
| `refresh_tokens` | `idx_refresh_tokens_user` | (user_id) | B-tree | Tokeny użytkownika |
| `refresh_tokens` | `idx_refresh_tokens_expires` | (expires_at) | Partial | Czyszczenie wygasłych tokenów |
| `locations` | `idx_locations_name_country` | (name, country) | B-tree | Wyszukiwanie lokalizacji |
| `attractions` | `idx_attractions_location_rating` | (location_id, rating DESC) | B-tree | Lista atrakcji dla lokalizacji sortowana po ocenie |
| `attractions` | `idx_attractions_name` | (name) | B-tree | Wyszukiwanie atrakcji po nazwie |
| `attractions` | `idx_attractions_created_by` | (created_by_user_id) | Partial | Atrakcje użytkownika |
| `trips` | `idx_trips_owner_created` | (owner_id, created_at DESC) | B-tree | Lista planów użytkownika |
| `trips` | `idx_trips_location` | (location_id) | Partial | Plany dla lokalizacji |
| `trips` | `idx_trips_public` | (created_at DESC) | Partial | Publiczne plany |
| `trip_attractions` | `idx_trip_attractions_trip_day_order` | (trip_id, day_number, order_index) | B-tree | Pobieranie harmonogramu |
| `trip_attractions` | `idx_trip_attractions_attraction` | (attraction_id) | B-tree | Sprawdzanie użycia atrakcji |

---

## 4. Autoryzacja na poziomie aplikacji

Ponieważ autoryzacja odbywa się w .NET (OAuth 2.0), nie używamy Row Level Security (RLS) w PostgreSQL. Cała logika autoryzacji jest implementowana w warstwie aplikacji.

### 4.1. Przepływ OAuth 2.0

```
┌─────────┐      ┌─────────────┐      ┌──────────────┐
│  Client │──1──>│  /api/auth  │──2──>│   Database   │
│ (React) │      │   /login    │      │   (users)    │
└─────────┘      └─────────────┘      └──────────────┘
     │                  │
     │<───3─── Access Token (JWT) + Refresh Token
     │
     │           ┌─────────────┐
     │───4──────>│  /api/...   │  (Authorization: Bearer {access_token})
     │           │  (protected)│
     │           └─────────────┘
```

1. Klient wysyła email + hasło do `/api/auth/login`
2. Serwer weryfikuje hasło (BCrypt) i tworzy tokeny
3. Zwraca: access token (JWT, 15-30 min) + refresh token (7-30 dni)
4. Klient używa access token do autoryzowanych żądań

### 4.2. Endpointy autoryzacji

| Endpoint | Metoda | Opis |
|----------|--------|------|
| `/api/auth/register` | POST | Rejestracja nowego użytkownika |
| `/api/auth/login` | POST | Logowanie (email + hasło → tokeny) |
| `/api/auth/refresh` | POST | Odświeżenie access token |
| `/api/auth/logout` | POST | Wylogowanie (unieważnienie refresh token) |
| `/api/auth/me` | GET | Dane zalogowanego użytkownika |

### 4.3. Struktura JWT (Access Token)

```json
{
  "sub": "user-uuid",
  "email": "user@example.com",
  "name": "Display Name",
  "iat": 1706000000,
  "exp": 1706001800
}
```

### 4.4. Reguły autoryzacji w aplikacji

| Zasób | Reguła |
|-------|--------|
| `users` | Użytkownik widzi/edytuje tylko siebie |
| `locations` | Publiczny odczyt, brak modyfikacji (dane systemowe) |
| `attractions` | Publiczny odczyt, tworzenie dla zalogowanych, edycja/usuwanie tylko własnych |
| `trips` | Odczyt własnych + publicznych, tworzenie/edycja/usuwanie tylko własnych |
| `trip_attractions` | Zgodnie z uprawnieniami do trip |

---

## 5. Funkcje i triggery

### 5.1. Automatyczna aktualizacja `updated_at`

```sql
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER
LANGUAGE plpgsql
AS $$
BEGIN
    NEW.updated_at = NOW();
    RETURN NEW;
END;
$$;

-- Triggery dla każdej tabeli
CREATE TRIGGER set_updated_at_users
    BEFORE UPDATE ON users
    FOR EACH ROW
    EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER set_updated_at_locations
    BEFORE UPDATE ON locations
    FOR EACH ROW
    EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER set_updated_at_attractions
    BEFORE UPDATE ON attractions
    FOR EACH ROW
    EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER set_updated_at_trips
    BEFORE UPDATE ON trips
    FOR EACH ROW
    EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER set_updated_at_trip_attractions
    BEFORE UPDATE ON trip_attractions
    FOR EACH ROW
    EXECUTE FUNCTION update_updated_at_column();
```

### 5.2. Czyszczenie wygasłych tokenów (opcjonalne)

```sql
-- Funkcja do czyszczenia wygasłych/unieważnionych tokenów
CREATE OR REPLACE FUNCTION cleanup_expired_tokens()
RETURNS INTEGER
LANGUAGE plpgsql
AS $$
DECLARE
    deleted_count INTEGER;
BEGIN
    DELETE FROM refresh_tokens
    WHERE expires_at < NOW() OR revoked_at IS NOT NULL;

    GET DIAGNOSTICS deleted_count = ROW_COUNT;
    RETURN deleted_count;
END;
$$;

-- Można wywołać okresowo przez pg_cron lub scheduled job w aplikacji
```

---

## 6. Dodatkowe uwagi

### 6.1. Decyzje projektowe

| Aspekt | Decyzja | Uzasadnienie |
|--------|---------|--------------|
| Autentykacja | .NET OAuth 2.0 | Pełna kontrola, brak zależności od Supabase |
| Hashowanie haseł | BCrypt/Argon2 | Bezpieczne, standardowe algorytmy |
| Access Token | JWT (stateless) | Brak zapytań do DB przy każdym żądaniu |
| Refresh Token | Hash w DB | Możliwość unieważnienia, bezpieczeństwo |
| Autoryzacja | Warstwa aplikacji | Logika w handlerach MediatR |
| Współrzędne | DECIMAL(10,7) | Precyzja ~1cm, wystarczająca dla nawigacji |
| Czas zwiedzania | INTEGER (minuty) | Prostota obliczeń |
| Planowany czas startu | TIME | Lokalna godzina destynacji |
| Punkt startowy | order_index = 1 | Brak redundancji |
| Soft delete | Nie | Hard delete w MVP |

### 6.2. Walidacja na poziomie aplikacji

Następujące walidacje są implementowane w aplikacji (nie w bazie):

1. **Rejestracja użytkownika:**
   - email: poprawny format, unikalność
   - hasło: min 8 znaków, wymagania złożoności

2. **Współrzędne geograficzne:**
   - latitude: -90 do 90
   - longitude: -180 do 180

3. **Usuwanie atrakcji użytkownika:**
   - Sprawdzenie czy atrakcja nie jest używana w planach innych użytkowników
   - Jeśli jest używana tylko we własnych planach → kaskadowe usunięcie z planów

4. **Parametry planowania:**
   - daily_hours: 1-24
   - max_extension_hours: 0-8
   - start_time: poprawny format TIME

### 6.3. Bezpieczeństwo

| Aspekt | Implementacja |
|--------|---------------|
| Hasła | BCrypt z cost factor 12+ lub Argon2id |
| JWT Secret | Silny klucz (256-bit), przechowywany bezpiecznie |
| Refresh Token | Losowy (256-bit), przechowywany jako SHA256 hash |
| HTTPS | Wymagane w produkcji |
| Rate limiting | Na endpointach auth (logowanie, rejestracja) |
| Token rotation | Opcjonalnie: nowy refresh token przy każdym odświeżeniu |

### 6.4. Migracja i seed data

Dane początkowe (lokalizacje, atrakcje systemowe) będą dodane w późniejszym etapie przez:
- Skrypty SQL seed
- Import z plików JSON/CSV
- Panel administracyjny

### 6.5. Zgodność z Clean Architecture

Schemat jest zaprojektowany z myślą o mapowaniu na encje domenowe:

```
Domain Entities          Database Tables
─────────────────        ───────────────
User                 →   users
RefreshToken         →   refresh_tokens
Location             →   locations
Attraction           →   attractions
Trip                 →   trips
TripAttraction       →   trip_attractions
```

Wartości domenowe (Value Objects) jak `Coordinates`, `Rating`, `Duration` są mapowane na odpowiednie kolumny w tabelach.
