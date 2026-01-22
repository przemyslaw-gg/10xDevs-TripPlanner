# Schemat bazy danych TripPlanner MVP

## 1. Tabele

### 1.1. `profiles`

Rozszerzenie danych użytkownika z Supabase Auth.

```sql
CREATE TABLE profiles (
    id UUID PRIMARY KEY REFERENCES auth.users(id) ON DELETE CASCADE,
    display_name VARCHAR(100),
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
```

| Kolumna | Typ | Ograniczenia | Opis |
|---------|-----|--------------|------|
| `id` | UUID | PK, FK → auth.users ON DELETE CASCADE | Identyfikator użytkownika |
| `display_name` | VARCHAR(100) | — | Wyświetlana nazwa użytkownika |
| `created_at` | TIMESTAMPTZ | NOT NULL, DEFAULT NOW() | Data utworzenia |
| `updated_at` | TIMESTAMPTZ | NOT NULL, DEFAULT NOW() | Data ostatniej modyfikacji |

---

### 1.2. `locations`

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

### 1.3. `attractions`

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
    created_by_user_id UUID REFERENCES profiles(id) ON DELETE SET NULL,
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
| `created_by_user_id` | UUID | FK → profiles, ON DELETE SET NULL | Autor (NULL = atrakcja systemowa) |
| `is_verified` | BOOLEAN | NOT NULL, DEFAULT FALSE | Czy atrakcja zweryfikowana |
| `created_at` | TIMESTAMPTZ | NOT NULL, DEFAULT NOW() | Data utworzenia |
| `updated_at` | TIMESTAMPTZ | NOT NULL, DEFAULT NOW() | Data ostatniej modyfikacji |

**Logika biznesowa:**
- Atrakcje systemowe: `created_by_user_id = NULL`, `is_verified = TRUE`
- Atrakcje użytkowników: `created_by_user_id` ustawione, `is_verified = FALSE`

---

### 1.4. `trips`

Plany wycieczek tworzonych przez użytkowników.

```sql
CREATE TABLE trips (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    owner_id UUID NOT NULL REFERENCES profiles(id) ON DELETE CASCADE,
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
| `owner_id` | UUID | FK → profiles, NOT NULL, ON DELETE CASCADE | Właściciel planu |
| `name` | VARCHAR(100) | NOT NULL | Nazwa planu (np. "Ateny 2026") |
| `location_id` | UUID | FK → locations, ON DELETE RESTRICT | Główna lokalizacja wycieczki |
| `is_public` | BOOLEAN | NOT NULL, DEFAULT FALSE | Czy plan jest publiczny |
| `daily_hours` | INTEGER | NOT NULL, DEFAULT 8 | Godziny zwiedzania dziennie |
| `max_extension_hours` | INTEGER | NOT NULL, DEFAULT 2 | Maks. wydłużenie dnia |
| `start_time` | TIME | NOT NULL, DEFAULT '09:00' | Godzina rozpoczęcia zwiedzania |
| `created_at` | TIMESTAMPTZ | NOT NULL, DEFAULT NOW() | Data utworzenia |
| `updated_at` | TIMESTAMPTZ | NOT NULL, DEFAULT NOW() | Data ostatniej modyfikacji |

---

### 1.5. `trip_attractions`

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
auth.users (Supabase Auth)
    │
    └──1:1──> profiles
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
| `auth.users` → `profiles` | 1:1 | Każdy użytkownik ma dokładnie jeden profil |
| `profiles` → `trips` | 1:N | Użytkownik może mieć wiele planów |
| `profiles` → `attractions` | 1:N | Użytkownik może utworzyć wiele atrakcji |
| `locations` → `attractions` | 1:N | Lokalizacja może mieć wiele atrakcji |
| `locations` → `trips` | 1:N | Lokalizacja może być celem wielu planów |
| `trips` ↔ `attractions` | N:M | Plan zawiera wiele atrakcji, atrakcja może być w wielu planach |

**Zasady usuwania (ON DELETE):**

| Źródło | Cel | Akcja | Uzasadnienie |
|--------|-----|-------|--------------|
| `auth.users` | `profiles` | CASCADE | Usunięcie konta usuwa profil |
| `profiles` | `trips` | CASCADE | Usunięcie profilu usuwa plany użytkownika |
| `profiles` | `attractions` | SET NULL | Usunięcie profilu nie usuwa atrakcji |
| `locations` | `attractions` | RESTRICT | Nie można usunąć lokalizacji z atrakcjami |
| `locations` | `trips` | RESTRICT | Nie można usunąć lokalizacji z planami |
| `trips` | `trip_attractions` | CASCADE | Usunięcie planu usuwa powiązania |
| `attractions` | `trip_attractions` | RESTRICT | Aplikacja sprawdza przed usunięciem |

---

## 3. Indeksy

```sql
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

## 4. Polityki PostgreSQL (Row Level Security)

### 4.1. Włączenie RLS

```sql
ALTER TABLE profiles ENABLE ROW LEVEL SECURITY;
ALTER TABLE locations ENABLE ROW LEVEL SECURITY;
ALTER TABLE attractions ENABLE ROW LEVEL SECURITY;
ALTER TABLE trips ENABLE ROW LEVEL SECURITY;
ALTER TABLE trip_attractions ENABLE ROW LEVEL SECURITY;
```

### 4.2. Polityki dla `profiles`

```sql
-- Użytkownik widzi tylko swój profil
CREATE POLICY "profiles_select_own"
    ON profiles FOR SELECT
    USING (id = auth.uid());

-- Użytkownik może aktualizować tylko swój profil
CREATE POLICY "profiles_update_own"
    ON profiles FOR UPDATE
    USING (id = auth.uid());

-- Użytkownik może wstawić tylko swój profil (dla triggera)
CREATE POLICY "profiles_insert_own"
    ON profiles FOR INSERT
    WITH CHECK (id = auth.uid());
```

### 4.3. Polityki dla `locations`

```sql
-- Lokalizacje są publiczne do odczytu
CREATE POLICY "locations_select_all"
    ON locations FOR SELECT
    USING (true);

-- Tylko administratorzy mogą modyfikować (przez service role)
```

### 4.4. Polityki dla `attractions`

```sql
-- Wszystkie atrakcje są widoczne publicznie
CREATE POLICY "attractions_select_all"
    ON attractions FOR SELECT
    USING (true);

-- Zalogowani użytkownicy mogą dodawać atrakcje
CREATE POLICY "attractions_insert_authenticated"
    ON attractions FOR INSERT
    WITH CHECK (auth.uid() IS NOT NULL);

-- Użytkownicy mogą edytować tylko swoje atrakcje
CREATE POLICY "attractions_update_own"
    ON attractions FOR UPDATE
    USING (created_by_user_id = auth.uid());

-- Użytkownicy mogą usuwać tylko swoje atrakcje
-- (aplikacja dodatkowo sprawdza czy atrakcja nie jest używana w cudzych planach)
CREATE POLICY "attractions_delete_own"
    ON attractions FOR DELETE
    USING (created_by_user_id = auth.uid());
```

### 4.5. Polityki dla `trips`

```sql
-- Użytkownik widzi swoje plany oraz publiczne plany innych
CREATE POLICY "trips_select_own_or_public"
    ON trips FOR SELECT
    USING (is_public = TRUE OR owner_id = auth.uid());

-- Użytkownik może tworzyć plany tylko dla siebie
CREATE POLICY "trips_insert_own"
    ON trips FOR INSERT
    WITH CHECK (owner_id = auth.uid());

-- Użytkownik może edytować tylko swoje plany
CREATE POLICY "trips_update_own"
    ON trips FOR UPDATE
    USING (owner_id = auth.uid());

-- Użytkownik może usuwać tylko swoje plany
CREATE POLICY "trips_delete_own"
    ON trips FOR DELETE
    USING (owner_id = auth.uid());
```

### 4.6. Polityki dla `trip_attractions`

```sql
-- Użytkownik widzi trip_attractions dla dostępnych planów
CREATE POLICY "trip_attractions_select"
    ON trip_attractions FOR SELECT
    USING (
        EXISTS (
            SELECT 1 FROM trips
            WHERE trips.id = trip_attractions.trip_id
            AND (trips.is_public = TRUE OR trips.owner_id = auth.uid())
        )
    );

-- Użytkownik może dodawać atrakcje tylko do swoich planów
CREATE POLICY "trip_attractions_insert_own"
    ON trip_attractions FOR INSERT
    WITH CHECK (
        EXISTS (
            SELECT 1 FROM trips
            WHERE trips.id = trip_attractions.trip_id
            AND trips.owner_id = auth.uid()
        )
    );

-- Użytkownik może edytować atrakcje tylko w swoich planach
CREATE POLICY "trip_attractions_update_own"
    ON trip_attractions FOR UPDATE
    USING (
        EXISTS (
            SELECT 1 FROM trips
            WHERE trips.id = trip_attractions.trip_id
            AND trips.owner_id = auth.uid()
        )
    );

-- Użytkownik może usuwać atrakcje tylko ze swoich planów
CREATE POLICY "trip_attractions_delete_own"
    ON trip_attractions FOR DELETE
    USING (
        EXISTS (
            SELECT 1 FROM trips
            WHERE trips.id = trip_attractions.trip_id
            AND trips.owner_id = auth.uid()
        )
    );
```

---

## 5. Funkcje i triggery

### 5.1. Automatyczne tworzenie profilu

```sql
CREATE OR REPLACE FUNCTION public.handle_new_user()
RETURNS TRIGGER
LANGUAGE plpgsql
SECURITY DEFINER
SET search_path = public
AS $$
BEGIN
    INSERT INTO public.profiles (id, display_name, created_at, updated_at)
    VALUES (NEW.id, COALESCE(NEW.raw_user_meta_data->>'display_name', NEW.email), NOW(), NOW());
    RETURN NEW;
END;
$$;

CREATE TRIGGER on_auth_user_created
    AFTER INSERT ON auth.users
    FOR EACH ROW
    EXECUTE FUNCTION public.handle_new_user();
```

### 5.2. Automatyczna aktualizacja `updated_at`

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
CREATE TRIGGER set_updated_at_profiles
    BEFORE UPDATE ON profiles
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

---

## 6. Dodatkowe uwagi

### 6.1. Decyzje projektowe

| Aspekt | Decyzja | Uzasadnienie |
|--------|---------|--------------|
| Autentykacja | Supabase Auth | Natywne RLS, gotowe API, bezpieczeństwo |
| Współrzędne | DECIMAL(10,7) | Precyzja ~1cm, wystarczająca dla nawigacji |
| Czas zwiedzania | INTEGER (minuty) | Prostota obliczeń |
| Planowany czas startu | TIME | Lokalna godzina destynacji |
| Punkt startowy | order_index = 1 | Brak redundancji |
| Soft delete | Nie | Hard delete w MVP |
| Walidacja współrzędnych | Aplikacja | Brak CHECK constraints |
| Limity | Brak | Nie potrzebne w MVP |

### 6.2. Walidacja na poziomie aplikacji

Następujące walidacje są implementowane w aplikacji (nie w bazie):

1. **Współrzędne geograficzne:**
   - latitude: -90 do 90
   - longitude: -180 do 180

2. **Usuwanie atrakcji użytkownika:**
   - Sprawdzenie czy atrakcja nie jest używana w planach innych użytkowników
   - Jeśli jest używana tylko we własnych planach → kaskadowe usunięcie z planów

3. **Parametry planowania:**
   - daily_hours: 1-24
   - max_extension_hours: 0-8
   - start_time: poprawny format TIME

### 6.3. Migracja i seed data

Dane początkowe (lokalizacje, atrakcje systemowe) będą dodane w późniejszym etapie przez:
- Skrypty SQL seed
- Import z plików JSON/CSV
- Panel administracyjny

### 6.4. Zgodność z Clean Architecture

Schemat jest zaprojektowany z myślą o mapowaniu na encje domenowe:

```
Domain Entities          Database Tables
─────────────────        ───────────────
User (Value Object)  →   profiles
Location             →   locations
Attraction           →   attractions
Trip                 →   trips
TripAttraction       →   trip_attractions
```

Wartości domenowe (Value Objects) jak `Coordinates`, `Rating`, `Duration` są mapowane na odpowiednie kolumny w tabelach.
