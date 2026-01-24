# Plan implementacji widoków Trips (Lista, Tworzenie, Szczegóły/Edycja)

## 1. Przegląd

Widoki Trips obejmują trzy powiązane ekrany do zarządzania wycieczkami użytkownika:
- **Lista wycieczek** (`/trips`) - przegląd wszystkich wycieczek użytkownika z możliwością nawigacji
- **Tworzenie wycieczki** (`/trips/new`) - formularz tworzenia nowej wycieczki
- **Szczegóły/Edycja wycieczki** (`/trips/:id`) - podgląd i edycja istniejącej wycieczki z listą atrakcji

Główne cele:
- Umożliwienie użytkownikowi przeglądania swoich zapisanych planów wycieczek
- Tworzenie nowych wycieczek z podstawowymi parametrami (nazwa, lokalizacja, godziny)
- Edycja wycieczki z autosave, zarządzanie atrakcjami (kolejność, usuwanie)
- Usunięcie wycieczki z potwierdzeniem

## 2. Routing widoku

| Ścieżka | Komponent | Dostęp |
|---------|-----------|--------|
| `/trips` | `TripsPage` | Wymagane logowanie |
| `/trips/new` | `CreateTripPage` | Wymagane logowanie |
| `/trips/:id` | `TripDetailsPage` | Wymagane logowanie (tylko właściciel) |

Wszystkie trasy wymagają owinięcia w `ProtectedRoute` (do stworzenia) sprawdzający autentykację.

## 3. Struktura komponentów

```
/trips
└── TripsPage
    ├── CreateTripButton
    ├── TripList
    │   ├── TripCard (×n)
    │   └── SkeletonTripCard (×n, podczas ładowania)
    └── EmptyState (gdy brak wycieczek)

/trips/new
└── CreateTripPage
    └── TripForm
        ├── TextInput (nazwa)
        ├── CharacterCounter
        ├── LocationSelect
        ├── TimeInput (startTime)
        ├── NumberInput (dailyHours)
        ├── NumberInput (maxExtensionHours)
        └── FormActions (Utwórz, Anuluj)

/trips/:id
└── TripDetailsPage
    ├── TripHeader
    │   ├── EditableTitle
    │   ├── LocationDisplay
    │   └── SaveStatusIndicator
    ├── TripSettingsPanel
    │   ├── TimeInput (startTime)
    │   ├── NumberInput (dailyHours)
    │   └── NumberInput (maxExtensionHours)
    ├── TripAttractionList
    │   ├── AttractionItem (×n)
    │   │   ├── AttractionInfo
    │   │   ├── ReorderButtons (góra/dół)
    │   │   └── RemoveButton
    │   └── EmptyAttractionState
    ├── AddAttractionButton
    ├── DeleteTripButton
    └── ConfirmDeleteModal
```

## 4. Szczegóły komponentów

### 4.1. TripsPage

- **Opis:** Główny kontener strony listy wycieczek. Wyświetla listę kart wycieczek użytkownika z przyciskiem tworzenia nowej.
- **Główne elementy:**
  - Nagłówek strony z tytułem "Moje wycieczki"
  - `CreateTripButton` - przycisk nawigacji do `/trips/new`
  - `TripList` lub `EmptyState` w zależności od danych
  - `Pagination` jeśli więcej niż 1 strona
- **Obsługiwane interakcje:**
  - Kliknięcie `CreateTripButton` → nawigacja do `/trips/new`
  - Kliknięcie karty wycieczki → nawigacja do `/trips/:id`
  - Zmiana strony paginacji
- **Obsługiwana walidacja:** Brak (tylko odczyt)
- **Typy:** `TripListItemDTO`, `PaginationDTO`, `UseTripsReturn`
- **Propsy:** Brak (komponent strony)

### 4.2. TripList

- **Opis:** Kontener listy kart wycieczek. Wyświetla skeleton podczas ładowania.
- **Główne elementy:**
  - Grid `TripCard` komponentów
  - `SkeletonTripCard` podczas ładowania (6 sztuk)
  - `EmptyState` gdy brak wycieczek
- **Obsługiwane interakcje:** Brak (deleguje do dzieci)
- **Obsługiwana walidacja:** Brak
- **Typy:** `TripListItemDTO[]`
- **Propsy:**
  ```typescript
  interface TripListProps {
    trips: TripListItemDTO[];
    isLoading: boolean;
    onClearFilters?: () => void;
  }
  ```

### 4.3. TripCard

- **Opis:** Karta pojedynczej wycieczki z podstawowymi informacjami. Cały element jest klikalnym linkiem.
- **Główne elementy:**
  - Nazwa wycieczki (h3)
  - Lokalizacja (jeśli przypisana)
  - Liczba atrakcji z ikoną
  - Liczba dni z ikoną
  - Badge "Publiczna" jeśli `isPublic === true`
  - Data utworzenia/modyfikacji
- **Obsługiwane interakcje:**
  - Kliknięcie karty → nawigacja do `/trips/:id`
  - Focus visible dla nawigacji klawiaturą
- **Obsługiwana walidacja:** Brak
- **Typy:** `TripListItemDTO`
- **Propsy:**
  ```typescript
  interface TripCardProps {
    trip: TripListItemDTO;
  }
  ```

### 4.4. SkeletonTripCard

- **Opis:** Placeholder karty wycieczki wyświetlany podczas ładowania.
- **Główne elementy:**
  - Animowane bloki pulse dla nazwy, lokalizacji, statystyk
- **Obsługiwane interakcje:** Brak
- **Obsługiwana walidacja:** Brak
- **Typy:** Brak
- **Propsy:** Brak

### 4.5. CreateTripButton

- **Opis:** Przycisk nawigacji do formularza tworzenia wycieczki.
- **Główne elementy:**
  - Przycisk z ikoną "+" i tekstem "Nowa wycieczka"
- **Obsługiwane interakcje:**
  - Kliknięcie → nawigacja do `/trips/new`
- **Obsługiwana walidacja:** Brak
- **Typy:** Brak
- **Propsy:** Brak

### 4.6. CreateTripPage

- **Opis:** Strona formularza tworzenia nowej wycieczki.
- **Główne elementy:**
  - Nagłówek "Utwórz nową wycieczkę"
  - `TripForm` z odpowiednimi propsami
- **Obsługiwane interakcje:**
  - Submit formularza → POST API → redirect do `/trips/:id`
  - Anuluj → nawigacja do `/trips`
- **Obsługiwana walidacja:** Delegowana do `TripForm`
- **Typy:** `CreateTripCommand`, `TripDTO`
- **Propsy:** Brak (komponent strony)

### 4.7. TripForm

- **Opis:** Formularz tworzenia/edycji wycieczki z walidacją React Hook Form + Zod.
- **Główne elementy:**
  - Input nazwy z `CharacterCounter` (widoczny od 80 znaków)
  - Select lokalizacji (opcjonalny)
  - Input godziny rozpoczęcia (startTime, format HH:mm)
  - Input liczby godzin dziennie (dailyHours, 1-24)
  - Input maksymalnego wydłużenia dnia (maxExtensionHours, 0-8)
  - Przyciski akcji (Utwórz/Zapisz, Anuluj)
  - Alert błędu API
- **Obsługiwane interakcje:**
  - Zmiana pól formularza
  - Submit formularza
  - Kliknięcie Anuluj
- **Obsługiwana walidacja:**
  - `name`: wymagane, 1-100 znaków
  - `locationId`: opcjonalne, poprawny UUID
  - `startTime`: wymagane, format HH:mm
  - `dailyHours`: wymagane, liczba 1-24
  - `maxExtensionHours`: wymagane, liczba 0-8
- **Typy:** `TripFormData`, `CreateTripCommand`, `LocationListItemDTO[]`
- **Propsy:**
  ```typescript
  interface TripFormProps {
    initialData?: Partial<TripFormData>;
    locations: LocationListItemDTO[];
    isLoadingLocations: boolean;
    onSubmit: (data: TripFormData) => Promise<void>;
    onCancel: () => void;
    submitLabel?: string;
    isSubmitting?: boolean;
  }
  ```

### 4.8. CharacterCounter

- **Opis:** Licznik znaków dla pola tekstowego, widoczny po przekroczeniu progu.
- **Główne elementy:**
  - Tekst "{current}/{max}" z odpowiednim kolorem
- **Obsługiwane interakcje:** Brak (tylko wyświetlanie)
- **Obsługiwana walidacja:** Brak
- **Typy:** Brak
- **Propsy:**
  ```typescript
  interface CharacterCounterProps {
    current: number;
    max: number;
    warningThreshold?: number; // domyślnie 80
  }
  ```

### 4.9. TripDetailsPage

- **Opis:** Strona szczegółów i edycji wycieczki z autosave.
- **Główne elementy:**
  - `TripHeader` z edytowalną nazwą i statusem zapisu
  - `TripSettingsPanel` z parametrami wycieczki
  - `TripAttractionList` z listą atrakcji
  - `AddAttractionButton` (disabled przy 20 atrakcjach)
  - `DeleteTripButton`
  - `ConfirmDeleteModal`
- **Obsługiwane interakcje:**
  - Edycja nazwy/parametrów → autosave z 2s debounce
  - Zmiana kolejności atrakcji (przyciski góra/dół)
  - Usunięcie atrakcji
  - Usunięcie wycieczki
- **Obsługiwana walidacja:**
  - Jak w `TripForm`
  - Max 20 atrakcji
- **Typy:** `TripDTO`, `TripAttractionsResponseDTO`, `UseTripDetailsReturn`
- **Propsy:** Brak (komponent strony, pobiera `id` z URL)

### 4.10. TripHeader

- **Opis:** Nagłówek strony szczegółów z edytowalną nazwą i statusem zapisu.
- **Główne elementy:**
  - `EditableTitle` - klikalna nazwa do edycji inline
  - Lokalizacja (tylko do odczytu)
  - `SaveStatusIndicator`
- **Obsługiwane interakcje:**
  - Kliknięcie nazwy → tryb edycji
  - Enter/blur → zapisz zmiany
  - Escape → anuluj edycję
- **Obsługiwana walidacja:**
  - `name`: wymagane, 1-100 znaków
- **Typy:** `TripDTO`
- **Propsy:**
  ```typescript
  interface TripHeaderProps {
    trip: TripDTO;
    onNameChange: (name: string) => void;
    saveStatus: SaveStatus;
  }
  ```

### 4.11. EditableTitle

- **Opis:** Pole tekstowe z trybem podglądu i edycji inline.
- **Główne elementy:**
  - Tryb podglądu: tekst z ikoną edycji
  - Tryb edycji: input z CharacterCounter
- **Obsługiwane interakcje:**
  - Kliknięcie → tryb edycji
  - Enter → zapisz i wyjdź
  - Escape → anuluj
  - Blur → zapisz i wyjdź
- **Obsługiwana walidacja:**
  - Minimalna długość 1 znak
  - Maksymalna długość 100 znaków
- **Typy:** Brak
- **Propsy:**
  ```typescript
  interface EditableTitleProps {
    value: string;
    onChange: (value: string) => void;
    maxLength?: number;
    placeholder?: string;
  }
  ```

### 4.12. SaveStatusIndicator

- **Opis:** Wskaźnik statusu autosave z aria-live.
- **Główne elementy:**
  - Ikona + tekst statusu ("Zapisywanie...", "Zapisano", "Błąd zapisu")
- **Obsługiwane interakcje:** Brak
- **Obsługiwana walidacja:** Brak
- **Typy:** `SaveStatus`
- **Propsy:**
  ```typescript
  interface SaveStatusIndicatorProps {
    status: SaveStatus;
  }
  ```

### 4.13. TripSettingsPanel

- **Opis:** Panel z edytowalnymi parametrami wycieczki.
- **Główne elementy:**
  - Select lokalizacji
  - Input godziny rozpoczęcia
  - Input godzin dziennie
  - Input maksymalnego wydłużenia
- **Obsługiwane interakcje:**
  - Zmiana wartości → wywołanie onChange z debounce
- **Obsługiwana walidacja:**
  - Jak w `TripForm`
- **Typy:** `TripDTO`, `LocationListItemDTO[]`
- **Propsy:**
  ```typescript
  interface TripSettingsPanelProps {
    trip: TripDTO;
    locations: LocationListItemDTO[];
    onChange: (updates: Partial<UpdateTripCommand>) => void;
    disabled?: boolean;
  }
  ```

### 4.14. TripAttractionList

- **Opis:** Lista atrakcji przypisanych do wycieczki z możliwością zmiany kolejności.
- **Główne elementy:**
  - Nagłówek z licznikiem atrakcji (X/20)
  - Lista `AttractionItem` komponentów
  - `EmptyAttractionState` gdy brak atrakcji
- **Obsługiwane interakcje:**
  - Zmiana kolejności (przyciski góra/dół)
  - Usunięcie atrakcji
- **Obsługiwana walidacja:**
  - Max 20 atrakcji
- **Typy:** `TripAttractionsResponseDTO`, `TripAttractionItemDTO`
- **Propsy:**
  ```typescript
  interface TripAttractionListProps {
    attractions: TripAttractionItemDTO[];
    totalCount: number;
    maxCount: number;
    onMoveUp: (attractionId: UUID) => void;
    onMoveDown: (attractionId: UUID) => void;
    onRemove: (attractionId: UUID) => void;
    isLoading?: boolean;
    disabled?: boolean;
  }
  ```

### 4.15. AttractionItem

- **Opis:** Element listy atrakcji z przyciskami akcji.
- **Główne elementy:**
  - Zdjęcie miniatury (opcjonalne)
  - Nazwa atrakcji
  - Czas trwania
  - Numer dnia
  - Przyciski góra/dół/usuń
- **Obsługiwane interakcje:**
  - Kliknięcie góra → onMoveUp
  - Kliknięcie dół → onMoveDown
  - Kliknięcie usuń → onRemove
- **Obsługiwana walidacja:** Brak
- **Typy:** `TripAttractionItemDTO`
- **Propsy:**
  ```typescript
  interface AttractionItemProps {
    attraction: TripAttractionItemDTO;
    isFirst: boolean;
    isLast: boolean;
    onMoveUp: () => void;
    onMoveDown: () => void;
    onRemove: () => void;
    disabled?: boolean;
  }
  ```

### 4.16. AddAttractionButton

- **Opis:** Przycisk/link do dodawania atrakcji (nawiguje do `/attractions`).
- **Główne elementy:**
  - Przycisk z ikoną "+" i tekstem "Dodaj atrakcje"
  - Disabled state z tooltip gdy limit 20
- **Obsługiwane interakcje:**
  - Kliknięcie → nawigacja do `/attractions?tripId={id}`
- **Obsługiwana walidacja:**
  - Disabled gdy liczba atrakcji >= 20
- **Typy:** Brak
- **Propsy:**
  ```typescript
  interface AddAttractionButtonProps {
    tripId: UUID;
    disabled?: boolean;
    currentCount: number;
    maxCount: number;
  }
  ```

### 4.17. DeleteTripButton

- **Opis:** Przycisk usunięcia wycieczki (styl danger).
- **Główne elementy:**
  - Przycisk z ikoną kosza i tekstem "Usuń wycieczkę"
- **Obsługiwane interakcje:**
  - Kliknięcie → otwarcie `ConfirmDeleteModal`
- **Obsługiwana walidacja:** Brak
- **Typy:** Brak
- **Propsy:**
  ```typescript
  interface DeleteTripButtonProps {
    onClick: () => void;
    disabled?: boolean;
  }
  ```

### 4.18. ConfirmDeleteModal

- **Opis:** Modal potwierdzenia usunięcia wycieczki.
- **Główne elementy:**
  - Tytuł "Usuń wycieczkę"
  - Komunikat ostrzegawczy
  - Przycisk "Anuluj"
  - Przycisk "Usuń" (danger)
- **Obsługiwane interakcje:**
  - Kliknięcie "Anuluj" → zamknięcie
  - Kliknięcie "Usuń" → onConfirm
  - Escape → zamknięcie
  - Kliknięcie overlay → zamknięcie
- **Obsługiwana walidacja:** Brak
- **Typy:** Brak
- **Propsy:**
  ```typescript
  interface ConfirmDeleteModalProps {
    isOpen: boolean;
    tripName: string;
    onConfirm: () => void;
    onCancel: () => void;
    isDeleting?: boolean;
  }
  ```

### 4.19. ProtectedRoute

- **Opis:** Wrapper dla tras wymagających autentykacji.
- **Główne elementy:**
  - Loading spinner podczas sprawdzania autentykacji
  - Redirect do `/login` gdy niezalogowany
  - Render children gdy zalogowany
- **Obsługiwane interakcje:** Brak
- **Obsługiwana walidacja:**
  - Sprawdzenie `isAuthenticated` z AuthContext
- **Typy:** `AuthContextValue`
- **Propsy:**
  ```typescript
  interface ProtectedRouteProps {
    children: ReactNode;
  }
  ```

## 5. Typy

### 5.1. Istniejące typy (z @types.ts)

```typescript
// Już zdefiniowane w projekcie
interface TripListItemDTO {
  id: UUID;
  ownerId: UUID;
  name: string;
  locationId: UUID | null;
  location: LocationSummaryDTO | null;
  isPublic: boolean;
  dailyHours: number;
  maxExtensionHours: number;
  startTime: TimeString;
  attractionCount: number;
  totalDays: number;
  isOwner: boolean;
  createdAt: ISODateTime;
  updatedAt: ISODateTime;
}

interface TripDTO {
  id: UUID;
  ownerId: UUID;
  name: string;
  locationId: UUID | null;
  location: (LocationSummaryDTO & { timezone: string | null }) | null;
  isPublic: boolean;
  dailyHours: number;
  maxExtensionHours: number;
  startTime: TimeString;
  isOwner: boolean;
  createdAt: ISODateTime;
  updatedAt: ISODateTime;
}

interface CreateTripCommand {
  name: string;
  locationId?: UUID | null;
  dailyHours: number;
  maxExtensionHours: number;
  startTime: TimeString;
}

interface UpdateTripCommand {
  name: string;
  locationId?: UUID | null;
  dailyHours: number;
  maxExtensionHours: number;
  startTime: TimeString;
}

interface TripAttractionItemDTO {
  id: UUID;
  attractionId: UUID;
  attraction: AttractionSummaryDTO;
  dayNumber: number;
  orderIndex: number;
  plannedStartTime: TimeString | null;
}

interface TripAttractionsResponseDTO {
  tripId: UUID;
  totalDays: number;
  totalDuration: number;
  days: TripDayDTO[];
}

interface TripsQueryParams {
  locationId?: UUID;
  onlyMine?: boolean;
  onlyPublic?: boolean;
  search?: string;
  page?: number;
  pageSize?: number;
}
```

### 5.2. Nowe typy do dodania

```typescript
// ViewModel dla formularza wycieczki
interface TripFormData {
  name: string;
  locationId: UUID | null;
  dailyHours: number;
  maxExtensionHours: number;
  startTime: string; // format HH:mm
}

// Status zapisu autosave
type SaveStatus = 'idle' | 'saving' | 'saved' | 'error';

// Return type dla useTrips hook
interface UseTripsReturn {
  trips: TripListItemDTO[];
  pagination: PaginationDTO | null;
  isLoading: boolean;
  error: string | null;
  currentPage: number;
  setPage: (page: number) => void;
  refresh: () => void;
}

// Return type dla useTripDetails hook
interface UseTripDetailsReturn {
  trip: TripDTO | null;
  attractions: TripAttractionItemDTO[];
  isLoading: boolean;
  isSaving: boolean;
  saveStatus: SaveStatus;
  error: string | null;

  // Actions
  updateTrip: (updates: Partial<UpdateTripCommand>) => void;
  moveAttractionUp: (attractionId: UUID) => void;
  moveAttractionDown: (attractionId: UUID) => void;
  removeAttraction: (attractionId: UUID) => Promise<void>;
  deleteTrip: () => Promise<void>;
  refresh: () => void;
}

// Props dla komponentów formularza
interface FormFieldProps {
  label: string;
  error?: string;
  required?: boolean;
  children: ReactNode;
}
```

## 6. Zarządzanie stanem

### 6.1. Hook `useTrips`

Zarządza stanem listy wycieczek użytkownika.

```typescript
function useTrips(): UseTripsReturn {
  // State
  const [trips, setTrips] = useState<TripListItemDTO[]>([]);
  const [pagination, setPagination] = useState<PaginationDTO | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // URL params
  const [searchParams, setSearchParams] = useSearchParams();
  const currentPage = parsePageParam(searchParams.get('page'));

  // Fetch trips on mount and page change
  useEffect(() => {
    fetchTrips({ onlyMine: true, page: currentPage });
  }, [currentPage]);

  // Actions
  const setPage = (page: number) => {...};
  const refresh = () => {...};

  return { trips, pagination, isLoading, error, currentPage, setPage, refresh };
}
```

### 6.2. Hook `useTripDetails`

Zarządza stanem szczegółów wycieczki z autosave.

```typescript
function useTripDetails(tripId: UUID): UseTripDetailsReturn {
  // State
  const [trip, setTrip] = useState<TripDTO | null>(null);
  const [attractions, setAttractions] = useState<TripAttractionItemDTO[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [saveStatus, setSaveStatus] = useState<SaveStatus>('idle');
  const [error, setError] = useState<string | null>(null);

  // Debounced save
  const debouncedSave = useMemo(
    () => debounce((updates) => saveTrip(updates), 2000),
    [tripId]
  );

  // Load data on mount
  useEffect(() => {
    loadTripAndAttractions();
  }, [tripId]);

  // Actions
  const updateTrip = (updates) => {
    setTrip(prev => ({ ...prev, ...updates }));
    setSaveStatus('saving');
    debouncedSave(updates);
  };

  const moveAttractionUp = (attractionId) => {...};
  const moveAttractionDown = (attractionId) => {...};
  const removeAttraction = async (attractionId) => {...};
  const deleteTrip = async () => {...};

  return { trip, attractions, isLoading, saveStatus, ... };
}
```

### 6.3. Hook `useTripForm`

Zarządza stanem formularza tworzenia wycieczki (opcjonalnie, można użyć React Hook Form bezpośrednio).

## 7. Integracja API

### 7.1. Nowy plik `src/api/trips.ts`

```typescript
const API_BASE_URL = '/api';

// GET /api/trips?onlyMine=true
export async function fetchTrips(
  params: TripsQueryParams
): Promise<TripsResponseDTO> {
  const queryString = new URLSearchParams();
  if (params.onlyMine) queryString.set('onlyMine', 'true');
  if (params.search) queryString.set('search', params.search);
  queryString.set('page', String(params.page ?? 1));
  queryString.set('pageSize', String(params.pageSize ?? 20));

  const token = getStoredToken();
  const response = await fetch(`${API_BASE_URL}/trips?${queryString}`, {
    headers: {
      'Authorization': `Bearer ${token}`,
    },
  });

  if (!response.ok) {
    await handleErrorResponse(response);
  }

  return response.json();
}

// GET /api/trips/{id}
export async function fetchTrip(id: UUID): Promise<TripDTO> {...}

// POST /api/trips
export async function createTrip(data: CreateTripCommand): Promise<TripDTO> {...}

// PUT /api/trips/{id}
export async function updateTrip(id: UUID, data: UpdateTripCommand): Promise<TripDTO> {...}

// DELETE /api/trips/{id}
export async function deleteTrip(id: UUID): Promise<void> {...}

// GET /api/trips/{tripId}/attractions
export async function fetchTripAttractions(tripId: UUID): Promise<TripAttractionsResponseDTO> {...}

// DELETE /api/trips/{tripId}/attractions/{attractionId}
export async function removeTripAttraction(tripId: UUID, attractionId: UUID): Promise<void> {...}

// POST /api/trips/{tripId}/attractions/reorder
export async function reorderTripAttractions(
  tripId: UUID,
  attractions: ReorderAttractionItem[]
): Promise<ReorderAttractionsResponseDTO> {...}
```

### 7.2. Typy żądań i odpowiedzi

| Endpoint | Metoda | Request | Response |
|----------|--------|---------|----------|
| `/api/trips` | GET | `TripsQueryParams` | `TripsResponseDTO` |
| `/api/trips/{id}` | GET | - | `TripDTO` |
| `/api/trips` | POST | `CreateTripCommand` | `TripDTO` |
| `/api/trips/{id}` | PUT | `UpdateTripCommand` | `TripDTO` |
| `/api/trips/{id}` | DELETE | - | `204 No Content` |
| `/api/trips/{id}/attractions` | GET | - | `TripAttractionsResponseDTO` |
| `/api/trips/{id}/attractions/{aid}` | DELETE | - | `204 No Content` |
| `/api/trips/{id}/attractions/reorder` | POST | `ReorderTripAttractionsCommand` | `ReorderAttractionsResponseDTO` |

## 8. Interakcje użytkownika

### 8.1. Lista wycieczek (`/trips`)

| Interakcja | Oczekiwany rezultat |
|------------|---------------------|
| Wejście na stronę | Pobranie listy wycieczek użytkownika, wyświetlenie skeleton |
| Kliknięcie "Nowa wycieczka" | Nawigacja do `/trips/new` |
| Kliknięcie karty wycieczki | Nawigacja do `/trips/{id}` |
| Kliknięcie numeru strony | Zmiana strony, pobranie nowych danych |
| Brak wycieczek | Wyświetlenie EmptyState z przyciskiem tworzenia |

### 8.2. Tworzenie wycieczki (`/trips/new`)

| Interakcja | Oczekiwany rezultat |
|------------|---------------------|
| Wejście na stronę | Focus na polu nazwy |
| Wpisywanie nazwy (>80 znaków) | Wyświetlenie CharacterCounter |
| Wpisywanie nazwy (>100 znaków) | Zablokowanie dalszego wpisywania |
| Submit z pustą nazwą | Wyświetlenie błędu walidacji |
| Submit z poprawnymi danymi | POST do API, redirect do `/trips/{id}` |
| Kliknięcie "Anuluj" | Nawigacja do `/trips` |
| Błąd API | Wyświetlenie alertu błędu |

### 8.3. Szczegóły wycieczki (`/trips/:id`)

| Interakcja | Oczekiwany rezultat |
|------------|---------------------|
| Wejście na stronę | Pobranie wycieczki i atrakcji, wyświetlenie danych |
| Kliknięcie nazwy | Przejście w tryb edycji |
| Edycja nazwy + Enter/blur | Autosave z debounce 2s |
| Zmiana parametru (godziny) | Autosave z debounce 2s |
| Status "Zapisywanie..." | Wyświetlenie podczas zapisu |
| Status "Zapisano" | Wyświetlenie po zapisie (znika po 2s) |
| Kliknięcie góra/dół atrakcji | Zmiana kolejności, reorder API |
| Kliknięcie usuń atrakcji | Usunięcie z listy, DELETE API |
| Kliknięcie "Dodaj atrakcje" | Nawigacja do `/attractions?tripId={id}` |
| Kliknięcie "Dodaj atrakcje" (limit 20) | Przycisk disabled, tooltip |
| Kliknięcie "Usuń wycieczkę" | Otwarcie ConfirmDeleteModal |
| Potwierdzenie usunięcia | DELETE API, redirect do `/trips` |
| Anulowanie usunięcia | Zamknięcie modala |
| Błąd 403 | Wyświetlenie komunikatu o braku uprawnień |
| Błąd 404 | Wyświetlenie komunikatu "nie znaleziono" |

## 9. Warunki i walidacja

### 9.1. Walidacja formularza tworzenia/edycji (Zod schema)

```typescript
const tripFormSchema = z.object({
  name: z
    .string()
    .min(1, 'Nazwa wycieczki jest wymagana')
    .max(100, 'Nazwa może mieć maksymalnie 100 znaków'),
  locationId: z
    .string()
    .uuid('Nieprawidłowa lokalizacja')
    .nullable()
    .optional(),
  startTime: z
    .string()
    .regex(/^([01]?[0-9]|2[0-3]):[0-5][0-9]$/, 'Nieprawidłowy format godziny (HH:mm)'),
  dailyHours: z
    .number()
    .int('Musi być liczbą całkowitą')
    .min(1, 'Minimum 1 godzina')
    .max(24, 'Maksimum 24 godziny'),
  maxExtensionHours: z
    .number()
    .int('Musi być liczbą całkowitą')
    .min(0, 'Minimum 0 godzin')
    .max(8, 'Maksimum 8 godzin'),
});
```

### 9.2. Warunki biznesowe

| Warunek | Komponent | Wpływ na UI |
|---------|-----------|-------------|
| Użytkownik niezalogowany | ProtectedRoute | Redirect do `/login` |
| Użytkownik nie jest właścicielem | TripDetailsPage | Błąd 403, komunikat |
| Wycieczka nie istnieje | TripDetailsPage | Błąd 404, komunikat |
| Liczba atrakcji >= 20 | AddAttractionButton | Przycisk disabled |
| Atrakcja pierwsza na liście | AttractionItem | Przycisk "góra" disabled |
| Atrakcja ostatnia na liście | AttractionItem | Przycisk "dół" disabled |
| Nazwa wycieczki >= 80 znaków | TripForm/EditableTitle | Wyświetlenie CharacterCounter |
| Formularz w trakcie wysyłania | TripForm | Wszystkie pola i przyciski disabled |

## 10. Obsługa błędów

### 10.1. Błędy sieciowe

| Błąd | Obsługa |
|------|---------|
| Brak połączenia | Toast "Brak połączenia z serwerem. Sprawdź połączenie internetowe." |
| Timeout | Toast "Przekroczono czas oczekiwania. Spróbuj ponownie." |

### 10.2. Błędy API

| Status | Obsługa |
|--------|---------|
| 400 | Wyświetlenie błędów walidacji przy polach formularza |
| 401 | Redirect do `/login`, wyczyść auth state |
| 403 | Komunikat "Nie masz uprawnień do tej wycieczki" |
| 404 | Komunikat "Wycieczka nie została znaleziona" z linkiem do `/trips` |
| 409 | Komunikat "Atrakcja jest już dodana do wycieczki" |
| 500 | Toast "Wystąpił błąd serwera. Spróbuj ponownie później." |

### 10.3. Błędy autosave

| Scenariusz | Obsługa |
|------------|---------|
| Błąd zapisu | `SaveStatusIndicator` pokazuje "Błąd zapisu", retry po 5s |
| Wielokrotne błędy | Toast z propozycją odświeżenia strony |
| Konflikt wersji | Alert "Dane zostały zmienione. Odśwież stronę." |

### 10.4. Stany brzegowe

| Stan | Obsługa |
|------|---------|
| Pusta lista wycieczek | `EmptyState` z przyciskiem tworzenia |
| Pusta lista atrakcji | `EmptyAttractionState` z przyciskiem dodawania |
| Błąd ładowania listy | `ErrorState` z przyciskiem "Spróbuj ponownie" |
| Błąd ładowania szczegółów | Pełnoekranowy komunikat błędu |

## 11. Kroki implementacji

### Krok 1: Przygotowanie infrastruktury
1. Utworzenie `src/components/common/ProtectedRoute.tsx`
2. Utworzenie `src/api/trips.ts` z funkcjami API
3. Dodanie nowych typów do `src/@types.ts` (TripFormData, SaveStatus, itp.)
4. Aktualizacja `App.tsx` z nowymi trasami

### Krok 2: Lista wycieczek
1. Utworzenie `src/hooks/useTrips.ts`
2. Utworzenie `src/components/trips/SkeletonTripCard.tsx`
3. Utworzenie `src/components/trips/TripCard.tsx`
4. Utworzenie `src/components/trips/TripList.tsx`
5. Utworzenie `src/components/trips/CreateTripButton.tsx`
6. Utworzenie `src/pages/TripsPage.tsx`

### Krok 3: Tworzenie wycieczki
1. Utworzenie `src/components/trips/CharacterCounter.tsx`
2. Utworzenie `src/components/trips/TripForm.tsx` (z React Hook Form + Zod)
3. Utworzenie `src/pages/CreateTripPage.tsx`

### Krok 4: Szczegóły wycieczki - podstawy
1. Utworzenie `src/hooks/useTripDetails.ts`
2. Utworzenie `src/components/trips/SaveStatusIndicator.tsx`
3. Utworzenie `src/components/trips/EditableTitle.tsx`
4. Utworzenie `src/components/trips/TripHeader.tsx`
5. Utworzenie `src/components/trips/TripSettingsPanel.tsx`

### Krok 5: Szczegóły wycieczki - atrakcje
1. Utworzenie `src/components/trips/AttractionItem.tsx`
2. Utworzenie `src/components/trips/TripAttractionList.tsx`
3. Utworzenie `src/components/trips/AddAttractionButton.tsx`
4. Utworzenie `src/components/trips/EmptyAttractionState.tsx`

### Krok 6: Usuwanie wycieczki
1. Utworzenie `src/components/common/Modal.tsx` (bazowy komponent)
2. Utworzenie `src/components/trips/ConfirmDeleteModal.tsx`
3. Utworzenie `src/components/trips/DeleteTripButton.tsx`

### Krok 7: Strona szczegółów
1. Utworzenie `src/pages/TripDetailsPage.tsx`
2. Integracja wszystkich komponentów
3. Implementacja autosave z debounce

### Krok 8: Finalizacja
1. Utworzenie `src/components/trips/index.ts` (barrel export)
2. Testy manualne wszystkich ścieżek
3. Weryfikacja responsywności
4. Weryfikacja dostępności (a11y)
5. Build produkcyjny
