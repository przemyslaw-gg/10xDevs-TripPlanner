# Schemat bazy danych TripPlanner MVP

## Przegląd

Dokument zawiera specyfikację schematu bazy danych PostgreSQL dla aplikacji TripPlanner MVP, wykorzystującej Supabase jako backend.

---

## Tabele

### 1. `profiles` (rozszerzenie Supabase Auth)

Rozszerzenie danych użytkownika z Supabase Auth.

| Kolumna | Typ | Ograniczenia | Opis |
|---------|-----|--------------|------|
| `id` | UUID | PK, FK → auth.users | Identyfikator użytkownika |
| `display_name` | VARCHAR(100) | | Wyświetlana nazwa użytkownika |
| `created_at` | TIMESTAMPTZ | NOT NULL, DEFAULT NOW() | Data utworzenia |
| `updated_at` | TIMESTAMPTZ | NOT NULL, DEFAULT NOW() | Data ostatniej modyfikacji |

**Trigger:** Automatyczne tworzenie profilu przy rejestracji nowego użytkownika w auth.users.

---

### 2. `locations` (miasta/regiony)

Lokalizacje turystyczne (miasta, regiony).

| Kolumna | Typ | Ograniczenia | Opis |
|---------|-----|--------------|------|
| `id` | UUID | PK, DEFAULT gen_random_uuid() | Identyfikator lokalizacji |
| `name` | VARCHAR(100) | NOT NULL | Nazwa miasta/regionu |
| `country` | VARCHAR(100) | NOT NULL | Kraj |
| `timezone` | VARCHAR(50) | | Strefa czasowa (np. 'Europe/Athens') |
| `created_at` | TIMESTAMPTZ | NOT NULL, DEFAULT NOW() | Data utworzenia |
| `updated_at` | TIMESTAMPTZ | NOT NULL, DEFAULT NOW() | Data ostatniej modyfikacji |

**Indeksy:**
- `idx_locations_name_country` na `(name, country)`

---

### 3. `attractions` (atrakcje turystyczne)

Atrakcje turystyczne przypisane do lokalizacji.

| Kolumna | Typ | Ograniczenia | Opis |
|---------|-----|--------------|------|
| `id` | UUID | PK, DEFAULT gen_random_uuid() | Identyfikator atrakcji |
| `location_id` | UUID | FK → locations, NOT NULL | Lokalizacja atrakcji |
| `name` | VARCHAR(200) | NOT NULL | Nazwa atrakcji |
| `description` | TEXT | | Opis atrakcji |
| `latitude` | DECIMAL(10,7) | NOT NULL | Szerokość geograficzna |
| `longitude` | DECIMAL(10,7) | NOT NULL | Długość geograficzna |
| `rating` | DECIMAL(2,1) | | Ocena (dane statyczne, np. 4.5) |
| `review_count` | INTEGER | | Liczba opinii (dane statyczne) |
| `estimated_duration` | INTEGER | | Szacowany czas zwiedzania w minutach |
| `image_url` | VARCHAR(500) | | URL do zdjęcia atrakcji |
| `created_by_user_id` | UUID | FK → profiles, nullable | Autor (NULL = atrakcja systemowa) |
| `is_verified` | BOOLEAN | NOT NULL, DEFAULT FALSE | Czy atrakcja zweryfikowana |
| `created_at` | TIMESTAMPTZ | NOT NULL, DEFAULT NOW() | Data utworzenia |
| `updated_at` | TIMESTAMPTZ | NOT NULL, DEFAULT NOW() | Data ostatniej modyfikacji |

**Indeksy:**
- `idx_attractions_location_rating` na `(location_id, rating DESC)`
- `idx_attractions_name` na `(name)`

**Logika biznesowa:**
- Atrakcje systemowe: `created_by_user_id = NULL`, `is_verified = TRUE`
- Atrakcje użytkowników: `created_by_user_id` ustawione, `is_verified = FALSE`

---

### 4. `trips` (plany wycieczek)

Plany wycieczek tworzonych przez użytkowników.

| Kolumna | Typ | Ograniczenia | Opis |
|---------|-----|--------------|------|
| `id` | UUID | PK, DEFAULT gen_random_uuid() | Identyfikator planu |
| `owner_id` | UUID | FK → profiles, NOT NULL | Właściciel planu |
| `name` | VARCHAR(100) | NOT NULL | Nazwa planu (np. "Ateny 2026") |
| `location_id` | UUID | FK → locations | Główna lokalizacja wycieczki |
| `is_public` | BOOLEAN | NOT NULL, DEFAULT FALSE | Czy plan jest publiczny |
| `daily_hours` | INTEGER | NOT NULL, DEFAULT 8 | Godziny zwiedzania dziennie |
| `max_extension_hours` | INTEGER | NOT NULL, DEFAULT 2 | Maks. wydłużenie dnia |
| `start_time` | TIME | NOT NULL, DEFAULT '09:00' | Godzina rozpoczęcia zwiedzania |
| `created_at` | TIMESTAMPTZ | NOT NULL, DEFAULT NOW() | Data utworzenia |
| `updated_at` | TIMESTAMPTZ | NOT NULL, DEFAULT NOW() | Data ostatniej modyfikacji |

**Indeksy:**
- `idx_trips_owner_created` na `(owner_id, created_at DESC)`
- `idx_trips_public` na `(is_public)` WHERE `is_public = TRUE` (partial index)

---

### 5. `trip_attractions` (atrakcje w planie)

Tabela pośrednia łącząca plany z atrakcjami, zawierająca harmonogram.

| Kolumna | Typ | Ograniczenia | Opis |
|---------|-----|--------------|------|
| `id` | UUID | PK, DEFAULT gen_random_uuid() | Identyfikator rekordu |
| `trip_id` | UUID | FK → trips, NOT NULL, ON DELETE CASCADE | Plan wycieczki |
| `attraction_id` | UUID | FK → attractions, NOT NULL, ON DELETE RESTRICT | Atrakcja |
| `day_number` | INTEGER | NOT NULL | Numer dnia (1, 2, 3...) |
| `order_index` | INTEGER | NOT NULL | Kolejność w ramach dnia (1, 2, 3...) |
| `planned_start_time` | TIME | | Planowana godzina rozpoczęcia |
| `created_at` | TIMESTAMPTZ | NOT NULL, DEFAULT NOW() | Data utworzenia |
| `updated_at` | TIMESTAMPTZ | NOT NULL, DEFAULT NOW() | Data ostatniej modyfikacji |

**Indeksy:**
- `idx_trip_attractions_trip_day_order` na `(trip_id, day_number, order_index)`

**Ograniczenia:**
- `UNIQUE (trip_id, attraction_id)` — każda atrakcja może być tylko raz w planie

---

## Row Level Security (RLS)

### profiles

```sql
-- Użytkownik widzi i edytuje tylko swój profil
ALTER TABLE profiles ENABLE ROW LEVEL SECURITY;

CREATE POLICY "Users can view own profile"
  ON profiles FOR SELECT
  USING (id = auth.uid());

CREATE POLICY "Users can update own profile"
  ON profiles FOR UPDATE
  USING (id = auth.uid());

CREATE POLICY "Users can insert own profile"
  ON profiles FOR INSERT
  WITH CHECK (id = auth.uid());
```

### locations

```sql
-- Lokalizacje są publiczne (tylko odczyt dla wszystkich)
ALTER TABLE locations ENABLE ROW LEVEL SECURITY;

CREATE POLICY "Locations are viewable by everyone"
  ON locations FOR SELECT
  USING (true);

-- INSERT/UPDATE/DELETE tylko dla administratorów (opcjonalnie przez service role)
```

### attractions

```sql
ALTER TABLE attractions ENABLE ROW LEVEL SECURITY;

-- Wszystkie atrakcje są widoczne publicznie
CREATE POLICY "Attractions are viewable by everyone"
  ON attractions FOR SELECT
  USING (true);

-- Zalogowani użytkownicy mogą dodawać atrakcje
CREATE POLICY "Authenticated users can insert attractions"
  ON attractions FOR INSERT
  WITH CHECK (auth.uid() IS NOT NULL);

-- Użytkownicy mogą edytować/usuwać tylko swoje atrakcje
CREATE POLICY "Users can update own attractions"
  ON attractions FOR UPDATE
  USING (created_by_user_id = auth.uid());

CREATE POLICY "Users can delete own attractions"
  ON attractions FOR DELETE
  USING (created_by_user_id = auth.uid());

-- Uwaga: Logika blokowania usunięcia atrakcji używanej w planach innych
-- użytkowników jest implementowana na poziomie aplikacji (nie w bazie)
```

### trips

```sql
ALTER TABLE trips ENABLE ROW LEVEL SECURITY;

-- Użytkownik widzi swoje plany oraz publiczne plany innych
CREATE POLICY "Users can view own and public trips"
  ON trips FOR SELECT
  USING (is_public = TRUE OR owner_id = auth.uid());

-- Użytkownik może tworzyć plany tylko dla siebie
CREATE POLICY "Users can insert own trips"
  ON trips FOR INSERT
  WITH CHECK (owner_id = auth.uid());

-- Użytkownik może edytować tylko swoje plany
CREATE POLICY "Users can update own trips"
  ON trips FOR UPDATE
  USING (owner_id = auth.uid());

-- Użytkownik może usuwać tylko swoje plany
CREATE POLICY "Users can delete own trips"
  ON trips FOR DELETE
  USING (owner_id = auth.uid());
```

### trip_attractions

```sql
ALTER TABLE trip_attractions ENABLE ROW LEVEL SECURITY;

-- Dostęp przez relację z trips
CREATE POLICY "Users can view trip_attractions for accessible trips"
  ON trip_attractions FOR SELECT
  USING (
    EXISTS (
      SELECT 1 FROM trips
      WHERE trips.id = trip_attractions.trip_id
      AND (trips.is_public = TRUE OR trips.owner_id = auth.uid())
    )
  );

CREATE POLICY "Users can insert trip_attractions for own trips"
  ON trip_attractions FOR INSERT
  WITH CHECK (
    EXISTS (
      SELECT 1 FROM trips
      WHERE trips.id = trip_attractions.trip_id
      AND trips.owner_id = auth.uid()
    )
  );

CREATE POLICY "Users can update trip_attractions for own trips"
  ON trip_attractions FOR UPDATE
  USING (
    EXISTS (
      SELECT 1 FROM trips
      WHERE trips.id = trip_attractions.trip_id
      AND trips.owner_id = auth.uid()
    )
  );

CREATE POLICY "Users can delete trip_attractions for own trips"
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

## Relacje i kaskadowe usuwanie

```
auth.users (Supabase Auth)
    │
    └──< profiles (1:1)
            │
            ├──< trips (1:N) ─── ON DELETE CASCADE ───> trip_attractions
            │
            └──< attractions (1:N, created_by_user_id, nullable)

locations (1:N) ───> attractions
locations (1:N) ───> trips
```

**Zasady usuwania:**
- `trips` → `trip_attractions`: **CASCADE** (usunięcie planu usuwa jego atrakcje)
- `locations` → `attractions`: **RESTRICT** (nie można usunąć lokalizacji z atrakcjami)
- `locations` → `trips`: **RESTRICT** (nie można usunąć lokalizacji z planami)
- `profiles` → `trips`: **CASCADE** (usunięcie użytkownika usuwa jego plany)
- `profiles` → `attractions`: **SET NULL** (usunięcie użytkownika nie usuwa atrakcji, ale usuwa powiązanie)
- `attractions` → `trip_attractions`: **RESTRICT** (aplikacja sprawdza przed usunięciem czy atrakcja jest używana w cudzych planach)

---

## Triggery

### Automatyczne tworzenie profilu

```sql
CREATE OR REPLACE FUNCTION public.handle_new_user()
RETURNS TRIGGER AS $$
BEGIN
  INSERT INTO public.profiles (id, display_name, created_at, updated_at)
  VALUES (NEW.id, NEW.email, NOW(), NOW());
  RETURN NEW;
END;
$$ LANGUAGE plpgsql SECURITY DEFINER;

CREATE TRIGGER on_auth_user_created
  AFTER INSERT ON auth.users
  FOR EACH ROW EXECUTE FUNCTION public.handle_new_user();
```

### Automatyczna aktualizacja updated_at

```sql
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
  NEW.updated_at = NOW();
  RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Aplikuj do wszystkich tabel
CREATE TRIGGER update_profiles_updated_at
  BEFORE UPDATE ON profiles
  FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_locations_updated_at
  BEFORE UPDATE ON locations
  FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_attractions_updated_at
  BEFORE UPDATE ON attractions
  FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_trips_updated_at
  BEFORE UPDATE ON trips
  FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_trip_attractions_updated_at
  BEFORE UPDATE ON trip_attractions
  FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
```

---

## Decyzje architektoniczne

| Aspekt | Decyzja | Uzasadnienie |
|--------|---------|--------------|
| Autentykacja | Supabase Auth | Natywne RLS, gotowe API |
| Współrzędne atrakcji | DECIMAL(10,7) | Precyzja ~1cm, wystarczająca dla nawigacji |
| Współrzędne lokalizacji | Brak | Nie potrzebne w MVP |
| Czas zwiedzania | INTEGER (minuty) | Prostota obliczeń, mapowanie na TimeSpan |
| Planowany czas startu | TIME | Lokalna godzina, niezależna od strefy serwera |
| Punkt startowy trasy | order_index = 1 | Brak redundancji, spójność danych |
| Soft delete | Nie | Hard delete z CASCADE w MVP |
| Cache dystansów | Nie | Haversine wystarczająco szybki |
| Kategorie atrakcji | Nie | Uproszczenie MVP |
| Historia zmian | created_at/updated_at | Bez wersjonowania w MVP |
| Zdjęcia atrakcji | URL zewnętrzny | Brak storage w MVP |
| Usuwanie profilu | CASCADE na trips | Usunięcie konta usuwa wszystkie plany użytkownika |
| Usuwanie atrakcji użytkownika | RESTRICT + logika w aplikacji | Aplikacja sprawdza użycie w cudzych planach przed usunięciem |
| Walidacja współrzędnych | Brak CHECK constraints | Walidacja na poziomie aplikacji |
| Limity atrakcji/planów | Brak | Nie potrzebne w MVP |

---

## Przykładowe zapytania

### Pobierz atrakcje dla lokalizacji (posortowane po ocenie)

```sql
SELECT * FROM attractions
WHERE location_id = :location_id
ORDER BY rating DESC NULLS LAST;
```

### Pobierz plany użytkownika

```sql
SELECT * FROM trips
WHERE owner_id = auth.uid()
ORDER BY created_at DESC;
```

### Pobierz harmonogram planu

```sql
SELECT
  ta.day_number,
  ta.order_index,
  ta.planned_start_time,
  a.name,
  a.estimated_duration,
  a.latitude,
  a.longitude
FROM trip_attractions ta
JOIN attractions a ON a.id = ta.attraction_id
WHERE ta.trip_id = :trip_id
ORDER BY ta.day_number, ta.order_index;
```

### Pobierz publiczne plany dla lokalizacji

```sql
SELECT t.*, p.display_name as owner_name
FROM trips t
JOIN profiles p ON p.id = t.owner_id
WHERE t.location_id = :location_id
  AND t.is_public = TRUE
ORDER BY t.created_at DESC;
```
