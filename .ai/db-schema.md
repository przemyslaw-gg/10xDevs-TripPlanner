# Specyfikacja Schematu Bazy Danych - TripPlanner MVP

## Podsumowanie

Dokument zawiera finalną specyfikację schematu bazy danych PostgreSQL dla aplikacji TripPlanner MVP. Schemat został zaprojektowany z uwzględnieniem:
- Clean Architecture i CQRS
- ASP.NET Identity dla autentykacji
- Row-Level Security (RLS) dla bezpieczeństwa danych
- Skalowalności i wydajności

---

## 1. Diagram ERD (Entity Relationship Diagram)

```
┌─────────────────┐       ┌─────────────────┐       ┌─────────────────┐
│   AspNetUsers   │       │    locations    │       │   attractions   │
│   (Identity)    │       ├─────────────────┤       ├─────────────────┤
├─────────────────┤       │ id (PK, UUID)   │       │ id (PK, UUID)   │
│ Id (PK, UUID)   │       │ name            │       │ location_id (FK)│
│ Email           │       │ country         │       │ name            │
│ UserName        │       │ latitude        │       │ description     │
│ PasswordHash    │       │ longitude       │       │ latitude        │
│ ...             │       │ created_at      │       │ longitude       │
└────────┬────────┘       └────────┬────────┘       │ rating          │
         │                         │                │ review_count    │
         │                         │                │ estimated_      │
         │ 1                       │ 1              │ duration_minutes│
         │                         │                │ created_by_     │
         │                         │                │ user_id (FK)    │
         │                         │                │ is_user_        │
         │                         │                │ generated       │
         │                         │                │ created_at      │
         │                         │                │ updated_at      │
         │                         └────────────────┤                 │
         │                                          └────────┬────────┘
         │                                                   │
         │ N                                                 │ N
         ▼                                                   │
┌─────────────────┐                                          │
│     trips       │                                          │
├─────────────────┤       ┌─────────────────┐                │
│ id (PK, UUID)   │       │ trip_attractions│                │
│ user_id (FK)    │       ├─────────────────┤                │
│ name            │       │ id (PK, UUID)   │                │
│ description     │       │ trip_id (FK)    │◄───────────────┘
│ location_id (FK)│       │ attraction_id   │
│ visibility      │       │ (FK)            │
│ hours_per_day   │       │ day_number      │
│ flex_hours      │       │ order_in_day    │
│ start_date      │       │ start_time      │
│ created_at      │       │ end_time        │
│ updated_at      │       │ created_at      │
└────────┬────────┘       └─────────────────┘
         │ 1                       ▲
         │                         │ N
         └─────────────────────────┘
```

---

## 2. Definicje Tabel

### 2.1. Tabela `locations`

Przechowuje informacje o miastach/lokalizacjach turystycznych.

```sql
CREATE TABLE locations (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(255) NOT NULL,
    country VARCHAR(100) NOT NULL,
    latitude DECIMAL(10, 7) NOT NULL,
    longitude DECIMAL(10, 7) NOT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),

    CONSTRAINT chk_locations_latitude
        CHECK (latitude >= -90 AND latitude <= 90),
    CONSTRAINT chk_locations_longitude
        CHECK (longitude >= -180 AND longitude <= 180),
    CONSTRAINT uq_locations_name_country
        UNIQUE (name, country)
);

COMMENT ON TABLE locations IS 'Miasta i lokalizacje turystyczne';
COMMENT ON COLUMN locations.latitude IS 'Szerokość geograficzna (-90 do 90)';
COMMENT ON COLUMN locations.longitude IS 'Długość geograficzna (-180 do 180)';
```

---

### 2.2. Tabela `attractions`

Przechowuje atrakcje turystyczne (systemowe i dodane przez użytkowników).

```sql
CREATE TABLE attractions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    location_id UUID NOT NULL,
    name VARCHAR(255) NOT NULL,
    description TEXT,
    latitude DECIMAL(10, 7) NOT NULL,
    longitude DECIMAL(10, 7) NOT NULL,
    rating DECIMAL(2, 1) DEFAULT 0,
    review_count INTEGER NOT NULL DEFAULT 0,
    estimated_duration_minutes INTEGER NOT NULL DEFAULT 60,
    created_by_user_id UUID,
    is_user_generated BOOLEAN NOT NULL DEFAULT FALSE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ,

    CONSTRAINT fk_attractions_location
        FOREIGN KEY (location_id)
        REFERENCES locations(id)
        ON DELETE RESTRICT,
    CONSTRAINT fk_attractions_user
        FOREIGN KEY (created_by_user_id)
        REFERENCES "AspNetUsers"("Id")
        ON DELETE SET NULL,
    CONSTRAINT chk_attractions_latitude
        CHECK (latitude >= -90 AND latitude <= 90),
    CONSTRAINT chk_attractions_longitude
        CHECK (longitude >= -180 AND longitude <= 180),
    CONSTRAINT chk_attractions_rating
        CHECK (rating >= 0 AND rating <= 5),
    CONSTRAINT chk_attractions_review_count
        CHECK (review_count >= 0),
    CONSTRAINT chk_attractions_duration
        CHECK (estimated_duration_minutes > 0)
);

COMMENT ON TABLE attractions IS 'Atrakcje turystyczne';
COMMENT ON COLUMN attractions.rating IS 'Ocena atrakcji w skali 0-5';
COMMENT ON COLUMN attractions.estimated_duration_minutes IS 'Szacowany czas zwiedzania w minutach';
COMMENT ON COLUMN attractions.is_user_generated IS 'TRUE jeśli atrakcja dodana przez użytkownika';
```

---

### 2.3. Tabela `trips`

Przechowuje plany wycieczek użytkowników.

```sql
CREATE TYPE trip_visibility AS ENUM ('private', 'public');

CREATE TABLE trips (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL,
    location_id UUID NOT NULL,
    name VARCHAR(255) NOT NULL,
    description TEXT,
    visibility trip_visibility NOT NULL DEFAULT 'private',
    hours_per_day INTEGER NOT NULL DEFAULT 8,
    flex_hours INTEGER NOT NULL DEFAULT 2,
    start_date DATE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ,

    CONSTRAINT fk_trips_user
        FOREIGN KEY (user_id)
        REFERENCES "AspNetUsers"("Id")
        ON DELETE CASCADE,
    CONSTRAINT fk_trips_location
        FOREIGN KEY (location_id)
        REFERENCES locations(id)
        ON DELETE RESTRICT,
    CONSTRAINT uq_trips_user_name
        UNIQUE (user_id, name),
    CONSTRAINT chk_trips_hours_per_day
        CHECK (hours_per_day > 0 AND hours_per_day <= 24),
    CONSTRAINT chk_trips_flex_hours
        CHECK (flex_hours >= 0 AND flex_hours <= 8)
);

COMMENT ON TABLE trips IS 'Plany wycieczek użytkowników';
COMMENT ON COLUMN trips.visibility IS 'Widoczność planu: private (tylko właściciel) lub public (wszyscy)';
COMMENT ON COLUMN trips.hours_per_day IS 'Maksymalna liczba godzin zwiedzania dziennie';
COMMENT ON COLUMN trips.flex_hours IS 'O ile godzin można wydłużyć dzień jeśli braknie czasu';
```

---

### 2.4. Tabela `trip_attractions`

Tabela pośrednia łącząca plany z atrakcjami (wraz z harmonogramem).

```sql
CREATE TABLE trip_attractions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    trip_id UUID NOT NULL,
    attraction_id UUID NOT NULL,
    day_number INTEGER NOT NULL DEFAULT 1,
    order_in_day INTEGER NOT NULL DEFAULT 1,
    start_time TIME,
    end_time TIME,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),

    CONSTRAINT fk_trip_attractions_trip
        FOREIGN KEY (trip_id)
        REFERENCES trips(id)
        ON DELETE CASCADE,
    CONSTRAINT fk_trip_attractions_attraction
        FOREIGN KEY (attraction_id)
        REFERENCES attractions(id)
        ON DELETE RESTRICT,
    CONSTRAINT uq_trip_attractions_trip_attraction
        UNIQUE (trip_id, attraction_id),
    CONSTRAINT uq_trip_attractions_order
        UNIQUE (trip_id, day_number, order_in_day),
    CONSTRAINT chk_trip_attractions_day
        CHECK (day_number > 0),
    CONSTRAINT chk_trip_attractions_order
        CHECK (order_in_day > 0),
    CONSTRAINT chk_trip_attractions_time
        CHECK (start_time IS NULL OR end_time IS NULL OR start_time < end_time)
);

COMMENT ON TABLE trip_attractions IS 'Atrakcje przypisane do planów wycieczek z harmonogramem';
COMMENT ON COLUMN trip_attractions.day_number IS 'Numer dnia wycieczki (1, 2, 3...)';
COMMENT ON COLUMN trip_attractions.order_in_day IS 'Kolejność zwiedzania w danym dniu';
```

---

## 3. Indeksy

```sql
-- Locations
CREATE INDEX idx_locations_country ON locations(country);
CREATE INDEX idx_locations_name ON locations(name);

-- Attractions
CREATE INDEX idx_attractions_location ON attractions(location_id);
CREATE INDEX idx_attractions_rating ON attractions(rating DESC);
CREATE INDEX idx_attractions_user ON attractions(created_by_user_id)
    WHERE created_by_user_id IS NOT NULL;
CREATE INDEX idx_attractions_user_generated ON attractions(is_user_generated)
    WHERE is_user_generated = TRUE;

-- Trips
CREATE INDEX idx_trips_user ON trips(user_id);
CREATE INDEX idx_trips_location ON trips(location_id);
CREATE INDEX idx_trips_visibility ON trips(visibility)
    WHERE visibility = 'public';
CREATE INDEX idx_trips_created_at ON trips(created_at DESC);

-- Trip Attractions
CREATE INDEX idx_trip_attractions_trip ON trip_attractions(trip_id);
CREATE INDEX idx_trip_attractions_attraction ON trip_attractions(attraction_id);
CREATE INDEX idx_trip_attractions_day_order ON trip_attractions(trip_id, day_number, order_in_day);
```

---

## 4. Row-Level Security (RLS)

### 4.1. Włączenie RLS

```sql
ALTER TABLE trips ENABLE ROW LEVEL SECURITY;
ALTER TABLE trip_attractions ENABLE ROW LEVEL SECURITY;
ALTER TABLE attractions ENABLE ROW LEVEL SECURITY;
```

### 4.2. Polityki dla tabeli `trips`

```sql
-- Użytkownik widzi swoje plany oraz publiczne plany innych
CREATE POLICY trips_select_policy ON trips
    FOR SELECT
    USING (
        user_id = current_setting('app.current_user_id')::UUID
        OR visibility = 'public'
    );

-- Użytkownik może tworzyć tylko swoje plany
CREATE POLICY trips_insert_policy ON trips
    FOR INSERT
    WITH CHECK (user_id = current_setting('app.current_user_id')::UUID);

-- Użytkownik może aktualizować tylko swoje plany
CREATE POLICY trips_update_policy ON trips
    FOR UPDATE
    USING (user_id = current_setting('app.current_user_id')::UUID)
    WITH CHECK (user_id = current_setting('app.current_user_id')::UUID);

-- Użytkownik może usuwać tylko swoje plany
CREATE POLICY trips_delete_policy ON trips
    FOR DELETE
    USING (user_id = current_setting('app.current_user_id')::UUID);
```

### 4.3. Polityki dla tabeli `trip_attractions`

```sql
-- Użytkownik widzi atrakcje swoich planów i planów publicznych
CREATE POLICY trip_attractions_select_policy ON trip_attractions
    FOR SELECT
    USING (
        EXISTS (
            SELECT 1 FROM trips
            WHERE trips.id = trip_attractions.trip_id
            AND (
                trips.user_id = current_setting('app.current_user_id')::UUID
                OR trips.visibility = 'public'
            )
        )
    );

-- Użytkownik może modyfikować atrakcje tylko w swoich planach
CREATE POLICY trip_attractions_insert_policy ON trip_attractions
    FOR INSERT
    WITH CHECK (
        EXISTS (
            SELECT 1 FROM trips
            WHERE trips.id = trip_attractions.trip_id
            AND trips.user_id = current_setting('app.current_user_id')::UUID
        )
    );

CREATE POLICY trip_attractions_update_policy ON trip_attractions
    FOR UPDATE
    USING (
        EXISTS (
            SELECT 1 FROM trips
            WHERE trips.id = trip_attractions.trip_id
            AND trips.user_id = current_setting('app.current_user_id')::UUID
        )
    );

CREATE POLICY trip_attractions_delete_policy ON trip_attractions
    FOR DELETE
    USING (
        EXISTS (
            SELECT 1 FROM trips
            WHERE trips.id = trip_attractions.trip_id
            AND trips.user_id = current_setting('app.current_user_id')::UUID
        )
    );
```

### 4.4. Polityki dla tabeli `attractions`

```sql
-- Wszyscy widzą atrakcje systemowe + użytkownik widzi swoje atrakcje
CREATE POLICY attractions_select_policy ON attractions
    FOR SELECT
    USING (
        is_user_generated = FALSE
        OR created_by_user_id = current_setting('app.current_user_id')::UUID
    );

-- Użytkownik może dodawać tylko swoje atrakcje
CREATE POLICY attractions_insert_policy ON attractions
    FOR INSERT
    WITH CHECK (
        created_by_user_id = current_setting('app.current_user_id')::UUID
        AND is_user_generated = TRUE
    );

-- Użytkownik może edytować tylko swoje atrakcje
CREATE POLICY attractions_update_policy ON attractions
    FOR UPDATE
    USING (
        created_by_user_id = current_setting('app.current_user_id')::UUID
        AND is_user_generated = TRUE
    );

-- Użytkownik może usuwać tylko swoje atrakcje (jeśli nie są używane)
CREATE POLICY attractions_delete_policy ON attractions
    FOR DELETE
    USING (
        created_by_user_id = current_setting('app.current_user_id')::UUID
        AND is_user_generated = TRUE
    );
```

---

## 5. Funkcje Pomocnicze

### 5.1. Automatyczna aktualizacja `updated_at`

```sql
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = NOW();
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Trigger dla trips
CREATE TRIGGER trips_updated_at
    BEFORE UPDATE ON trips
    FOR EACH ROW
    EXECUTE FUNCTION update_updated_at_column();

-- Trigger dla attractions
CREATE TRIGGER attractions_updated_at
    BEFORE UPDATE ON attractions
    FOR EACH ROW
    EXECUTE FUNCTION update_updated_at_column();
```

### 5.2. Funkcja obliczania odległości (Haversine)

```sql
CREATE OR REPLACE FUNCTION haversine_distance(
    lat1 DECIMAL(10,7),
    lon1 DECIMAL(10,7),
    lat2 DECIMAL(10,7),
    lon2 DECIMAL(10,7)
)
RETURNS DECIMAL AS $$
DECLARE
    R CONSTANT DECIMAL := 6371; -- Promień Ziemi w km
    dlat DECIMAL;
    dlon DECIMAL;
    a DECIMAL;
    c DECIMAL;
BEGIN
    dlat := RADIANS(lat2 - lat1);
    dlon := RADIANS(lon2 - lon1);
    a := SIN(dlat/2) * SIN(dlat/2) +
         COS(RADIANS(lat1)) * COS(RADIANS(lat2)) *
         SIN(dlon/2) * SIN(dlon/2);
    c := 2 * ATAN2(SQRT(a), SQRT(1-a));
    RETURN R * c;
END;
$$ LANGUAGE plpgsql IMMUTABLE;

COMMENT ON FUNCTION haversine_distance IS 'Oblicza odległość w km między dwoma punktami geograficznymi';
```

---

## 6. Przykładowe Dane Seedowe

```sql
-- Lokalizacje
INSERT INTO locations (id, name, country, latitude, longitude) VALUES
    ('a1b2c3d4-e5f6-7890-abcd-ef1234567890', 'Ateny', 'Grecja', 37.9838096, 23.7275388),
    ('b2c3d4e5-f6a7-8901-bcde-f12345678901', 'Rzym', 'Włochy', 41.9027835, 12.4963655),
    ('c3d4e5f6-a7b8-9012-cdef-123456789012', 'Paryż', 'Francja', 48.8566140, 2.3522219);

-- Atrakcje w Atenach
INSERT INTO attractions (location_id, name, description, latitude, longitude, rating, review_count, estimated_duration_minutes) VALUES
    ('a1b2c3d4-e5f6-7890-abcd-ef1234567890', 'Akropol', 'Starożytna cytadela na wzgórzu', 37.9715323, 23.7267166, 4.8, 125000, 180),
    ('a1b2c3d4-e5f6-7890-abcd-ef1234567890', 'Muzeum Akropolu', 'Muzeum archeologiczne', 37.9685438, 23.7285321, 4.7, 45000, 120),
    ('a1b2c3d4-e5f6-7890-abcd-ef1234567890', 'Agora Ateńska', 'Starożytny rynek', 37.9747636, 23.7234669, 4.5, 18000, 90),
    ('a1b2c3d4-e5f6-7890-abcd-ef1234567890', 'Plaka', 'Historyczna dzielnica', 37.9725648, 23.7312584, 4.6, 32000, 120),
    ('a1b2c3d4-e5f6-7890-abcd-ef1234567890', 'Świątynia Zeusa Olimpijskiego', 'Ruiny świątyni', 37.9693397, 23.7331626, 4.4, 22000, 60);
```

---

## 7. Mapowanie EF Core (C#)

### 7.1. Konfiguracja DbContext

```csharp
public class TripPlannerDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Attraction> Attractions => Set<Attraction>();
    public DbSet<Trip> Trips => Set<Trip>();
    public DbSet<TripAttraction> TripAttractions => Set<TripAttraction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TripPlannerDbContext).Assembly);

        // Konwersja ENUM na PostgreSQL
        modelBuilder.HasPostgresEnum<TripVisibility>();
    }
}
```

### 7.2. Encje domenowe

```csharp
public class Location
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Country { get; set; } = null!;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<Attraction> Attractions { get; set; } = new List<Attraction>();
    public ICollection<Trip> Trips { get; set; } = new List<Trip>();
}

public class Attraction
{
    public Guid Id { get; set; }
    public Guid LocationId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public decimal Rating { get; set; }
    public int ReviewCount { get; set; }
    public int EstimatedDurationMinutes { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public bool IsUserGenerated { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public Location Location { get; set; } = null!;
    public ApplicationUser? CreatedByUser { get; set; }
    public ICollection<TripAttraction> TripAttractions { get; set; } = new List<TripAttraction>();
}

public enum TripVisibility
{
    Private,
    Public
}

public class Trip
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid LocationId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public TripVisibility Visibility { get; set; }
    public int HoursPerDay { get; set; }
    public int FlexHours { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public ApplicationUser User { get; set; } = null!;
    public Location Location { get; set; } = null!;
    public ICollection<TripAttraction> TripAttractions { get; set; } = new List<TripAttraction>();
}

public class TripAttraction
{
    public Guid Id { get; set; }
    public Guid TripId { get; set; }
    public Guid AttractionId { get; set; }
    public int DayNumber { get; set; }
    public int OrderInDay { get; set; }
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public DateTime CreatedAt { get; set; }

    public Trip Trip { get; set; } = null!;
    public Attraction Attraction { get; set; } = null!;
}
```

---

## 8. Decyzje Architektoniczne - Podsumowanie

| Aspekt | Decyzja | Uzasadnienie |
|--------|---------|--------------|
| Klucze główne | UUID | Bezpieczeństwo, zgodność z Identity |
| Nazewnictwo | snake_case | Standard PostgreSQL |
| Soft delete | Nie (hard delete) | Prostota dla MVP |
| Kolumny audytowe | created_at, updated_at | Śledzenie zmian |
| Lokalizacja geograficzna | DECIMAL(10,7) | Wystarczające bez PostGIS |
| Czas zwiedzania | INTEGER (minuty) | Prostota obliczeń |
| Widoczność planów | ENUM (private/public) | Czytelność i type safety |
| RLS | Włączone | Bezpieczeństwo na poziomie bazy |
| Kaskadowe usuwanie | CASCADE dla planów, RESTRICT dla atrakcji | Integralność danych |

---

## 9. Kolejne Kroki

1. **Utworzenie migracji EF Core** - wygenerowanie migracji na podstawie encji
2. **Implementacja RLS w Supabase** - jeśli używamy Supabase, RLS konfigurujemy przez dashboard
3. **Seed data** - załadowanie przykładowych lokalizacji i atrakcji
4. **Testy integracyjne** - weryfikacja constraintów i RLS

---

*Dokument wygenerowany: 2026-01-19*
*Wersja: 1.0 MVP*
