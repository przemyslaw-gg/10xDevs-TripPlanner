# Plan implementacji widoku Przeglądanie atrakcji

## 1. Przegląd

Widok "Przeglądanie atrakcji" służy do odkrywania atrakcji turystycznych dostępnych w systemie TripPlanner. Użytkownicy mogą przeglądać listę atrakcji w formie kart, filtrować je według lokalizacji, nawigować między stronami wyników oraz - jeśli są zalogowani - dodawać własne atrakcje. Widok jest publiczny i nie wymaga autentykacji do przeglądania.

## 2. Routing widoku

- **Ścieżka:** `/attractions`
- **Dostęp:** Publiczny (bez wymagania autentykacji)
- **Parametry URL (query string):**
  - `locationId` (opcjonalny) - UUID lokalizacji do filtrowania
  - `page` (opcjonalny) - numer strony (domyślnie: 1)

## 3. Struktura komponentów

```
AttractionsPage
├── LocationFilter
├── CreateAttractionButton (warunkowo - tylko dla zalogowanych)
├── AttractionList
│   ├── AttractionCard (dla każdej atrakcji)
│   │   └── Badge "Własna" (warunkowo)
│   ├── SkeletonAttractionCard (podczas ładowania)
│   └── EmptyState (gdy brak wyników)
└── Pagination
```

## 4. Szczegóły komponentów

### AttractionsPage

- **Opis:** Główny komponent strony, zarządza stanem widoku, pobiera dane z API i koordynuje interakcje między komponentami potomnymi.
- **Główne elementy:**
  - Nagłówek strony z tytułem
  - Sekcja filtrów (`LocationFilter`)
  - Przycisk dodawania atrakcji (`CreateAttractionButton`)
  - Lista atrakcji (`AttractionList`)
  - Komponent paginacji (`Pagination`)
- **Obsługiwane interakcje:**
  - Zmiana filtra lokalizacji
  - Zmiana strony paginacji
  - Inicjalizacja pobierania danych przy montowaniu i zmianie parametrów
- **Obsługiwana walidacja:**
  - Walidacja parametru `page` (musi być liczbą >= 1)
  - Walidacja parametru `locationId` (musi być prawidłowym UUID lub puste)
- **Typy:**
  - `AttractionsQueryParams`
  - `AttractionsResponseDTO`
  - `LocationListItemDTO[]`
- **Propsy:** Brak (komponent strony)

### LocationFilter

- **Opis:** Dropdown pozwalający użytkownikowi wybrać lokalizację do filtrowania atrakcji. Zawiera opcję "Wszystkie lokalizacje".
- **Główne elementy:**
  - `<label>` z opisem pola
  - `<select>` z opcjami lokalizacji
  - Opcja domyślna "Wszystkie lokalizacje"
- **Obsługiwane interakcje:**
  - `onChange` - zmiana wybranej lokalizacji, wywołuje callback z nowym `locationId`
- **Obsługiwana walidacja:** Brak (dowolna wartość jest akceptowalna)
- **Typy:**
  - `LocationListItemDTO`
- **Propsy:**
  ```typescript
  interface LocationFilterProps {
    locations: LocationListItemDTO[];
    selectedLocationId: UUID | null;
    onLocationChange: (locationId: UUID | null) => void;
    isLoading?: boolean;
  }
  ```

### CreateAttractionButton

- **Opis:** Przycisk nawigujący do formularza tworzenia nowej atrakcji. Wyświetlany tylko dla zalogowanych użytkowników.
- **Główne elementy:**
  - `<button>` lub `<Link>` z tekstem "Dodaj własną atrakcję"
  - Ikona plus (opcjonalnie)
- **Obsługiwane interakcje:**
  - `onClick` - nawigacja do `/attractions/new` lub otwarcie modalu tworzenia
- **Obsługiwana walidacja:** Brak
- **Typy:** Brak specyficznych
- **Propsy:**
  ```typescript
  interface CreateAttractionButtonProps {
    onClick?: () => void;
  }
  ```

### AttractionList

- **Opis:** Kontener wyświetlający siatkę (grid) kart atrakcji. Zarządza stanami: ładowanie, dane, brak wyników.
- **Główne elementy:**
  - `<div>` z CSS Grid dla kart atrakcji
  - Mapowanie tablicy atrakcji na komponenty `AttractionCard`
  - `SkeletonAttractionCard` podczas ładowania
  - `EmptyState` gdy brak wyników
- **Obsługiwane interakcje:** Brak bezpośrednich (delegowane do kart)
- **Obsługiwana walidacja:** Brak
- **Typy:**
  - `AttractionListItemDTO`
- **Propsy:**
  ```typescript
  interface AttractionListProps {
    attractions: AttractionListItemDTO[];
    isLoading: boolean;
    currentUserId?: UUID | null;
  }
  ```

### AttractionCard

- **Opis:** Karta prezentująca pojedynczą atrakcję z obrazkiem, nazwą, opisem, oceną i informacjami dodatkowymi.
- **Główne elementy:**
  - `<article>` jako kontener karty (semantyczny HTML)
  - `<img>` z obrazkiem atrakcji (z atrybutem `alt`)
  - `<h3>` z nazwą atrakcji
  - `<p>` z opisem (skróconym)
  - Sekcja z oceną (gwiazdki/liczba) i liczbą opinii
  - Sekcja z szacowanym czasem zwiedzania
  - Badge "Własna" dla atrakcji utworzonych przez bieżącego użytkownika
  - Badge "Zweryfikowana" dla zweryfikowanych atrakcji
- **Obsługiwane interakcje:**
  - Cała karta jako link do szczegółów atrakcji (`/attractions/{id}`)
  - Hover state dla interaktywności
- **Obsługiwana walidacja:** Brak
- **Typy:**
  - `AttractionListItemDTO`
- **Propsy:**
  ```typescript
  interface AttractionCardProps {
    attraction: AttractionListItemDTO;
    isOwned?: boolean;
  }
  ```

### SkeletonAttractionCard

- **Opis:** Placeholder wyświetlany podczas ładowania danych, zachowujący układ karty atrakcji.
- **Główne elementy:**
  - `<div>` z animacją pulse/shimmer
  - Placeholder dla obrazka
  - Placeholder dla tekstu (nazwa, opis)
  - Placeholder dla oceny
- **Obsługiwane interakcje:** Brak
- **Obsługiwana walidacja:** Brak
- **Typy:** Brak
- **Propsy:** Brak

### EmptyState

- **Opis:** Komunikat wyświetlany gdy nie ma atrakcji spełniających kryteria wyszukiwania.
- **Główne elementy:**
  - Ikona (np. pusta lista, lupa)
  - Nagłówek "Brak atrakcji"
  - Tekst pomocniczy (np. "Spróbuj zmienić filtr lokalizacji")
  - Opcjonalnie przycisk do wyczyszczenia filtrów
- **Obsługiwane interakcje:**
  - `onClick` na przycisku czyszczenia filtrów (opcjonalnie)
- **Obsługiwana walidacja:** Brak
- **Typy:** Brak
- **Propsy:**
  ```typescript
  interface EmptyStateProps {
    title?: string;
    message?: string;
    onClearFilters?: () => void;
  }
  ```

### Pagination

- **Opis:** Komponent nawigacji między stronami wyników z przyciskami Poprzednia/Następna oraz numerami stron.
- **Główne elementy:**
  - `<nav>` z atrybutem `aria-label="Pagination"`
  - Przycisk "Poprzednia" (disabled na pierwszej stronie)
  - Lista numerów stron (z aktualną stroną wyróżnioną)
  - Przycisk "Następna" (disabled na ostatniej stronie)
- **Obsługiwane interakcje:**
  - `onClick` na przyciskach stron - zmiana aktualnej strony
  - Obsługa klawiszy dla dostępności (Enter, Space)
- **Obsługiwana walidacja:**
  - Strona musi być w zakresie 1 do totalPages
- **Typy:**
  - `PaginationDTO`
- **Propsy:**
  ```typescript
  interface PaginationProps {
    currentPage: number;
    totalPages: number;
    onPageChange: (page: number) => void;
  }
  ```

## 5. Typy

### Istniejące typy z `@types.ts`

```typescript
// Główny typ elementu listy atrakcji
interface AttractionListItemDTO {
  id: UUID;
  locationId: UUID;
  name: string;
  description: string | null;
  latitude: number;
  longitude: number;
  rating: number | null;
  reviewCount: number | null;
  estimatedDuration: number | null;
  imageUrl: string | null;
  isVerified: boolean;
  createdByUserId: UUID | null;
}

// Odpowiedź paginowana
type AttractionsResponseDTO = PaginatedResponse<AttractionListItemDTO>;

// Parametry zapytania
interface AttractionsQueryParams {
  locationId?: UUID;
  search?: string;
  sortBy?: 'rating' | 'name' | 'reviewCount';
  sortOrder?: 'asc' | 'desc';
  isVerified?: boolean;
  page?: number;
  pageSize?: number;
}

// Lokalizacja do filtra
interface LocationListItemDTO {
  id: UUID;
  name: string;
  country: string;
  timezone: string | null;
}

// Metadane paginacji
interface PaginationDTO {
  page: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
  hasNextPage?: boolean;
  hasPreviousPage?: boolean;
}
```

### Nowe typy ViewModel

```typescript
// Stan widoku atrakcji
interface AttractionsViewState {
  attractions: AttractionListItemDTO[];
  locations: LocationListItemDTO[];
  pagination: PaginationDTO | null;
  selectedLocationId: UUID | null;
  currentPage: number;
  isLoadingAttractions: boolean;
  isLoadingLocations: boolean;
  error: string | null;
}

// Parametry filtrowania widoku
interface AttractionsFilterParams {
  locationId: UUID | null;
  page: number;
}
```

## 6. Zarządzanie stanem

### Custom Hook: `useAttractions`

Hook zarządzający stanem widoku atrakcji, pobieraniem danych i synchronizacją z URL.

```typescript
interface UseAttractionsReturn {
  // Dane
  attractions: AttractionListItemDTO[];
  locations: LocationListItemDTO[];
  pagination: PaginationDTO | null;

  // Stan ładowania
  isLoadingAttractions: boolean;
  isLoadingLocations: boolean;

  // Błędy
  error: string | null;

  // Filtry
  selectedLocationId: UUID | null;
  currentPage: number;

  // Akcje
  setLocationFilter: (locationId: UUID | null) => void;
  setPage: (page: number) => void;
  refreshAttractions: () => void;
}
```

**Odpowiedzialności hooka:**
1. Pobieranie listy atrakcji przy montowaniu i zmianie filtrów
2. Pobieranie listy lokalizacji dla filtra (jednorazowo)
3. Synchronizacja parametrów filtrowania z URL (query string)
4. Obsługa stanów ładowania i błędów
5. Reset strony do 1 przy zmianie filtra lokalizacji

### Custom Hook: `useAuth` (istniejący lub do stworzenia)

Hook do sprawdzania stanu autentykacji użytkownika.

```typescript
interface UseAuthReturn {
  isAuthenticated: boolean;
  currentUserId: UUID | null;
  // ... inne pola
}
```

## 7. Integracja API

### Endpoint 1: GET /api/attractions

**Cel:** Pobieranie paginowanej listy atrakcji z opcjonalnym filtrowaniem.

**Żądanie:**
```typescript
// Query parameters
{
  locationId?: UUID;      // Filtr lokalizacji
  page?: number;          // Numer strony (domyślnie: 1)
  pageSize?: number;      // Rozmiar strony (stały: 10)
  sortBy?: string;        // Domyślnie: 'rating'
  sortOrder?: string;     // Domyślnie: 'desc'
}
```

**Odpowiedź (200 OK):**
```typescript
{
  items: AttractionListItemDTO[];
  pagination: PaginationDTO;
}
```

**Obsługa błędów:**
- 400 - Nieprawidłowe parametry zapytania
- 500 - Błąd serwera

### Endpoint 2: GET /api/locations

**Cel:** Pobieranie listy lokalizacji do wypełnienia filtra.

**Żądanie:** Brak parametrów (pobierz wszystkie)

**Odpowiedź (200 OK):**
```typescript
{
  items: LocationListItemDTO[];
  pagination: PaginationDTO;
}
```

### Implementacja wywołań API

```typescript
// api/attractions.ts
export async function fetchAttractions(
  params: AttractionsQueryParams
): Promise<AttractionsResponseDTO> {
  const queryString = new URLSearchParams();
  if (params.locationId) queryString.set('locationId', params.locationId);
  queryString.set('page', String(params.page ?? 1));
  queryString.set('pageSize', '10');
  queryString.set('sortBy', 'rating');
  queryString.set('sortOrder', 'desc');

  const response = await fetch(`/api/attractions?${queryString}`);
  if (!response.ok) throw new Error('Failed to fetch attractions');
  return response.json();
}

// api/locations.ts
export async function fetchLocations(): Promise<LocationsResponseDTO> {
  const response = await fetch('/api/locations?pageSize=100');
  if (!response.ok) throw new Error('Failed to fetch locations');
  return response.json();
}
```

## 8. Interakcje użytkownika

| Interakcja | Element | Oczekiwany rezultat |
|------------|---------|---------------------|
| Wejście na stronę | - | Załadowanie pierwszej strony atrakcji i listy lokalizacji |
| Zmiana filtra lokalizacji | LocationFilter | Reset strony do 1, załadowanie atrakcji dla wybranej lokalizacji, aktualizacja URL |
| Kliknięcie numeru strony | Pagination | Załadowanie wybranej strony atrakcji, aktualizacja URL |
| Kliknięcie "Poprzednia" | Pagination | Załadowanie poprzedniej strony (jeśli dostępna) |
| Kliknięcie "Następna" | Pagination | Załadowanie następnej strony (jeśli dostępna) |
| Kliknięcie karty atrakcji | AttractionCard | Nawigacja do szczegółów atrakcji `/attractions/{id}` |
| Kliknięcie "Dodaj własną atrakcję" | CreateAttractionButton | Nawigacja do formularza tworzenia `/attractions/new` |
| Kliknięcie "Wyczyść filtry" (w EmptyState) | EmptyState | Reset filtra lokalizacji, załadowanie wszystkich atrakcji |

## 9. Warunki i walidacja

### Walidacja parametrów URL

| Parametr | Warunek | Działanie przy nieprawidłowej wartości |
|----------|---------|----------------------------------------|
| `page` | Liczba całkowita >= 1 | Użyj wartości domyślnej (1) |
| `locationId` | Prawidłowy format UUID lub pusty | Zignoruj parametr |

### Warunki wyświetlania komponentów

| Komponent | Warunek wyświetlenia |
|-----------|----------------------|
| `CreateAttractionButton` | `useAuth().isAuthenticated === true` |
| `SkeletonAttractionCard` | `isLoadingAttractions === true` |
| `EmptyState` | `isLoadingAttractions === false && attractions.length === 0` |
| `AttractionList` | `isLoadingAttractions === false && attractions.length > 0` |
| `Pagination` | `pagination !== null && pagination.totalPages > 1` |
| Badge "Własna" na karcie | `attraction.createdByUserId === currentUserId` |
| Badge "Zweryfikowana" | `attraction.isVerified === true` |

### Warunki stanu przycisków paginacji

| Przycisk | Warunek disabled |
|----------|------------------|
| "Poprzednia" | `currentPage === 1` |
| "Następna" | `currentPage === pagination.totalPages` |

## 10. Obsługa błędów

### Scenariusze błędów i ich obsługa

| Scenariusz | Obsługa |
|------------|---------|
| Błąd pobierania atrakcji | Wyświetl komunikat błędu z przyciskiem "Spróbuj ponownie" |
| Błąd pobierania lokalizacji | Wyświetl filter z tekstem "Błąd ładowania lokalizacji", pozwól na przeglądanie bez filtra |
| Brak połączenia z siecią | Wyświetl komunikat "Brak połączenia z internetem" |
| Timeout żądania | Wyświetl komunikat błędu z przyciskiem "Spróbuj ponownie" |
| Nieistniejąca strona (page > totalPages) | Przekieruj na ostatnią dostępną stronę |
| Nieprawidłowy locationId | Zignoruj filtr, pobierz wszystkie atrakcje |

### Komponent błędu

```typescript
interface ErrorStateProps {
  message: string;
  onRetry?: () => void;
}
```

## 11. Kroki implementacji

1. **Utworzenie struktury plików:**
   - `src/pages/AttractionsPage.tsx`
   - `src/components/attractions/AttractionList.tsx`
   - `src/components/attractions/AttractionCard.tsx`
   - `src/components/attractions/SkeletonAttractionCard.tsx`
   - `src/components/attractions/LocationFilter.tsx`
   - `src/components/attractions/CreateAttractionButton.tsx`
   - `src/components/common/Pagination.tsx`
   - `src/components/common/EmptyState.tsx`
   - `src/hooks/useAttractions.ts`
   - `src/api/attractions.ts`
   - `src/api/locations.ts`

2. **Implementacja funkcji API:**
   - `fetchAttractions(params)` - pobieranie atrakcji z filtrowaniem i paginacją
   - `fetchLocations()` - pobieranie listy lokalizacji

3. **Implementacja custom hooka `useAttractions`:**
   - Stan: attractions, locations, pagination, loading states, error
   - Efekty: pobieranie danych przy zmianie filtrów
   - Synchronizacja z URL (useSearchParams z react-router-dom)

4. **Implementacja komponentów pomocniczych:**
   - `EmptyState` - uniwersalny komponent pustego stanu
   - `Pagination` - uniwersalny komponent paginacji
   - `SkeletonAttractionCard` - skeleton loader

5. **Implementacja komponentu `LocationFilter`:**
   - Select z opcjami lokalizacji
   - Obsługa stanu ładowania
   - Callback przy zmianie wartości

6. **Implementacja komponentu `AttractionCard`:**
   - Layout karty z obrazkiem, nazwą, opisem
   - Wyświetlanie oceny i liczby opinii
   - Wyświetlanie czasu zwiedzania
   - Badge'y (Własna, Zweryfikowana)
   - Cała karta jako link

7. **Implementacja komponentu `AttractionList`:**
   - Grid responsywny (1-3 kolumny w zależności od szerokości)
   - Mapowanie atrakcji na karty
   - Obsługa stanów: loading, empty, data

8. **Implementacja komponentu `CreateAttractionButton`:**
   - Przycisk z ikoną
   - Warunkowe wyświetlanie (tylko dla zalogowanych)

9. **Implementacja strony `AttractionsPage`:**
   - Kompozycja wszystkich komponentów
   - Użycie hooka `useAttractions`
   - Integracja z routingiem

10. **Konfiguracja routingu:**
    - Dodanie ścieżki `/attractions` w React Router
    - Konfiguracja jako publiczna strona

11. **Stylowanie z Tailwind CSS:**
    - Responsywny grid dla kart
    - Style hover dla kart
    - Style paginacji
    - Animacje skeleton loaders

12. **Testowanie:**
    - Testy jednostkowe komponentów
    - Testy integracyjne hooka
    - Testy E2E przepływu użytkownika

13. **Dostępność (A11y):**
    - Alt text dla obrazków atrakcji
    - Aria labels dla nawigacji
    - Obsługa klawiatury
    - Kontrast kolorów
