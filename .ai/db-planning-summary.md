# Podsumowanie planowania bazy danych TripPlanner MVP

## Decyzje

1. **Autentykacja użytkowników** — tabela `users` będzie zarządzana przez Supabase Auth (`auth.users`), nie ASP.NET Identity
2. **Tabela locations** — bez współrzędnych geograficznych (latitude/longitude), tylko `name`, `country`, `timezone`
3. **Audyt zmian** — tylko `created_at` i `updated_at`, bez kolumny `version` do optymistycznego lockowania
4. **Oceny atrakcji** — `rating` i `review_count` jako dane statyczne (importowane/wprowadzane ręcznie), bez systemu oceniania przez użytkowników
5. **Kategoryzacja atrakcji** — brak kategorii w MVP (bez kolumny `category`)
6. **Zdjęcia atrakcji** — `image_url` jako VARCHAR z URL do zewnętrznego zasobu w internecie
7. **Usuwanie profilu użytkownika** — CASCADE: usunięcie konta usuwa wszystkie plany użytkownika
8. **Usuwanie atrakcji użytkownika** — RESTRICT na FK + logika w aplikacji: aplikacja sprawdza przed usunięciem czy atrakcja jest używana w cudzych planach
9. **Walidacja współrzędnych** — brak CHECK constraints w bazie, walidacja na poziomie aplikacji
10. **Limity** — brak limitów na liczbę atrakcji/planów w MVP

---

## Dopasowane rekomendacje

1. **Tabela `profiles`** jako rozszerzenie Supabase Auth z `id` (FK → auth.users), `display_name`, `created_at`, `updated_at` + trigger automatycznego tworzenia profilu
2. **Współrzędne atrakcji** jako dwie kolumny `DECIMAL(10,7)` dla latitude i longitude — wystarczające dla algorytmu Haversine
3. **Tabela pośrednia `trip_attractions`** z kolumnami: `trip_id`, `attraction_id`, `day_number`, `order_index`, `planned_start_time` (TIME) — pełna obsługa harmonogramu
4. **Model hybrydowy atrakcji** — `created_by_user_id` (nullable) i `is_verified` (boolean) do rozróżnienia atrakcji systemowych od użytkowników
5. **RLS dla trips** — `is_public` (boolean) z politykami: SELECT dla publicznych lub własnych, INSERT/UPDATE/DELETE tylko dla właściciela
6. **Hard delete z CASCADE** — usunięcie `trip` kaskadowo usuwa `trip_attractions`
7. **Punkt startowy trasy** — określany przez `order_index = 1` w `trip_attractions`, bez osobnej kolumny
8. **Parametry planowania per-trip** — `daily_hours`, `max_extension_hours`, `start_time` w tabeli `trips`
9. **Czas zwiedzania** — `estimated_duration` jako INTEGER (minuty)
10. **Bez cache dystansów** — Haversine obliczany w locie, wystarczający dla MVP

---

## Podsumowanie planowania bazy danych

### Główne wymagania schematu bazy danych

Aplikacja TripPlanner MVP wymaga schematu obsługującego:
- Autentykację przez Supabase Auth z rozszerzeniem danych użytkownika
- Zarządzanie lokalizacjami turystycznymi (miasta/regiony)
- Katalog atrakcji (systemowych i dodanych przez użytkowników)
- CRUD planów wycieczek z podziałem na dni i harmonogramem czasowym
- Publikowanie planów (publiczne/prywatne)
- Row Level Security na poziomie PostgreSQL

### Kluczowe encje i relacje

```
auth.users (Supabase)
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

**5 tabel:**
- `profiles` — dane użytkownika (rozszerzenie auth.users)
- `locations` — miasta/regiony (name, country, timezone)
- `attractions` — atrakcje turystyczne (współrzędne, rating, czas zwiedzania, image_url)
- `trips` — plany wycieczek (nazwa, właściciel, parametry czasowe, is_public)
- `trip_attractions` — harmonogram (day_number, order_index, planned_start_time)

### Bezpieczeństwo

- **Supabase Auth** — zarządzanie użytkownikami i sesjami
- **Row Level Security (RLS)** — natywne polityki PostgreSQL:
  - `profiles`: tylko własny profil
  - `locations`: publiczny odczyt
  - `attractions`: publiczny odczyt, zapis dla zalogowanych, edycja/usuwanie tylko własnych
  - `trips`: odczyt własnych + publicznych, zapis tylko własnych
  - `trip_attractions`: dostęp przez relację z trips (owner_id = auth.uid())

### Skalowalność

- **Indeksy:** na `(location_id, rating DESC)`, `(owner_id, created_at DESC)`, `(trip_id, day_number, order_index)`
- **Partial index:** na `is_public` WHERE `is_public = TRUE`
- **Brak cache dystansów** — algorytm Haversine wystarczający dla dziesiątek atrakcji
- **Brak soft delete** — uproszczenie MVP, hard delete z CASCADE

### Triggery

- Automatyczne tworzenie `profiles` przy rejestracji użytkownika
- Automatyczna aktualizacja `updated_at` przy modyfikacji rekordów

