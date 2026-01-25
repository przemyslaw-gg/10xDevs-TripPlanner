# Plan implementacji widoku Optymalizacji Trasy

## 1. Przegląd

Widok optymalizacji trasy to rozszerzenie istniejącej strony szczegółów wycieczki (`TripDetailsPage`). Dodaje przycisk "Optymalizuj trasę", który wywołuje algorytm najbliższego sąsiada (Nearest Neighbor) w celu zoptymalizowania kolejności odwiedzania atrakcji. Po kliknięciu przycisku użytkownik wybiera atrakcję startową w modalu, a następnie system automatycznie przelicza optymalną trasę i aktualizuje kolejność atrakcji na liście.

**Główne cele:**
- Minimalizacja dystansu podróży między atrakcjami
- Automatyczne sortowanie atrakcji w optymalnej kolejności
- Wyświetlenie całkowitej odległości trasy po optymalizacji

## 2. Routing widoku

Funkcjonalność jest częścią istniejącego widoku szczegółów wycieczki:

```
/trips/:id
```

Nie wymaga nowej ścieżki routingu - jest rozszerzeniem istniejącej strony `TripDetailsPage`.

## 3. Struktura komponentów

```
TripDetailsPage (istniejący)
├── TripHeader (istniejący)
├── TripSettingsPanel (istniejący)
├── DeleteTripButton (istniejący)
├── ActionBar (rozszerzony)
│   ├── OptimizeRouteButton (NOWY)
│   └── AddAttractionButton (istniejący)
├── TripAttractionList (istniejący)
├── SelectStartingAttractionModal (NOWY)
│   └── AttractionRadioItem (NOWY)
└── OptimizationResultToast (NOWY)
```

## 4. Szczegóły komponentów

### OptimizeRouteButton

**Opis:**
Przycisk uruchamiający proces optymalizacji trasy. Wyświetla się po lewej stronie od przycisku "Dodaj atrakcje". Po kliknięciu otwiera modal wyboru atrakcji startowej.

**Główne elementy:**
- `<button>` z ikoną trasy/optymalizacji (SVG)
- Tekst "Optymalizuj trasę"
- Opcjonalny spinner podczas ładowania

**Obsługiwane interakcje:**
- `onClick` - otwiera modal wyboru atrakcji startowej

**Obsługiwana walidacja:**
- Disabled gdy `attractionsCount < 2` (potrzeba minimum 2 atrakcji do optymalizacji)
- Disabled gdy `isSaving === true`
- Disabled gdy `isOptimizing === true`

**Typy:**
- `OptimizeRouteButtonProps`

**Propsy:**
```typescript
interface OptimizeRouteButtonProps {
  onClick: () => void;
  disabled?: boolean;
  isOptimizing?: boolean;
  attractionsCount: number;
}
```

---

### SelectStartingAttractionModal

**Opis:**
Modal pozwalający użytkownikowi wybrać atrakcję, od której ma rozpocząć się zoptymalizowana trasa. Wyświetla listę wszystkich atrakcji w wycieczce z radio buttonami.

**Główne elementy:**
- `<Modal>` (istniejący komponent)
- Nagłówek z tytułem "Wybierz atrakcję startową"
- Opis wyjaśniający cel wyboru
- Lista atrakcji z `<AttractionRadioItem>`
- Przyciski "Anuluj" i "Optymalizuj"
- Spinner/loading state podczas optymalizacji

**Obsługiwane interakcje:**
- `onSelect(attractionId)` - wybór atrakcji startowej
- `onConfirm` - potwierdzenie i uruchomienie optymalizacji
- `onCancel` - zamknięcie modala bez akcji

**Obsługiwana walidacja:**
- Przycisk "Optymalizuj" disabled gdy nie wybrano żadnej atrakcji
- Przycisk "Optymalizuj" disabled podczas trwającej optymalizacji

**Typy:**
- `SelectStartingAttractionModalProps`
- `TripAttractionItemDTO` (istniejący)

**Propsy:**
```typescript
interface SelectStartingAttractionModalProps {
  isOpen: boolean;
  attractions: TripAttractionItemDTO[];
  onConfirm: (startingAttractionId: UUID) => void;
  onCancel: () => void;
  isOptimizing?: boolean;
}
```

---

### AttractionRadioItem

**Opis:**
Pojedynczy element listy atrakcji w modalu wyboru. Zawiera radio button, nazwę atrakcji i opcjonalnie miniaturę zdjęcia.

**Główne elementy:**
- `<label>` jako wrapper
- `<input type="radio">`
- Miniatura zdjęcia atrakcji (opcjonalna)
- Nazwa atrakcji
- Czas trwania (opcjonalny)

**Obsługiwane interakcje:**
- `onChange` - zmiana wyboru radio button

**Obsługiwana walidacja:**
- Brak specyficznej walidacji

**Typy:**
- `AttractionRadioItemProps`

**Propsy:**
```typescript
interface AttractionRadioItemProps {
  attraction: TripAttractionItemDTO;
  isSelected: boolean;
  onSelect: () => void;
  disabled?: boolean;
}
```

---

### OptimizationResultToast

**Opis:**
Tymczasowe powiadomienie (toast) wyświetlające wynik optymalizacji - całkowitą odległość trasy w metrach/kilometrach.

**Główne elementy:**
- Container z animacją wejścia/wyjścia
- Ikona sukcesu (checkmark)
- Tekst z informacją o dystansie
- Przycisk zamknięcia

**Obsługiwane interakcje:**
- `onClose` - ręczne zamknięcie toasta
- Auto-hide po 5 sekundach

**Obsługiwana walidacja:**
- Brak

**Typy:**
- `OptimizationResultToastProps`

**Propsy:**
```typescript
interface OptimizationResultToastProps {
  isVisible: boolean;
  totalDistance: number; // w metrach
  onClose: () => void;
}
```

## 5. Typy

### Nowe typy (do dodania do `@types.ts`)

```typescript
/**
 * Props dla OptimizeRouteButton
 */
export interface OptimizeRouteButtonProps {
  onClick: () => void;
  disabled?: boolean;
  isOptimizing?: boolean;
  attractionsCount: number;
}

/**
 * Props dla SelectStartingAttractionModal
 */
export interface SelectStartingAttractionModalProps {
  isOpen: boolean;
  attractions: TripAttractionItemDTO[];
  onConfirm: (startingAttractionId: UUID) => void;
  onCancel: () => void;
  isOptimizing?: boolean;
}

/**
 * Props dla AttractionRadioItem
 */
export interface AttractionRadioItemProps {
  attraction: TripAttractionItemDTO;
  isSelected: boolean;
  onSelect: () => void;
  disabled?: boolean;
}

/**
 * Props dla OptimizationResultToast
 */
export interface OptimizationResultToastProps {
  isVisible: boolean;
  totalDistance: number;
  onClose: () => void;
}
```

### Istniejące typy (już zdefiniowane)

```typescript
// Command - request body
interface OptimizeRouteCommand {
  startingAttractionId: UUID;
}

// Response DTO
interface OptimizeRouteResponseDTO {
  tripId: UUID;
  optimizedOrder: OptimizedRouteItemDTO[];
  totalDistance: number;
  optimizedAt: ISODateTime;
}

// Pojedynczy element w odpowiedzi
interface OptimizedRouteItemDTO {
  attractionId: UUID;
  attractionName: string;
  dayNumber: number;
  orderIndex: number;
}
```

## 6. Zarządzanie stanem

### Rozszerzenie hooka `useTripDetails`

Hook `useTripDetails` powinien zostać rozszerzony o funkcjonalność optymalizacji trasy:

```typescript
interface UseTripDetailsReturn {
  // ...istniejące pola...

  // Nowe pola dla optymalizacji
  isOptimizing: boolean;
  lastOptimizationDistance: number | null;
  optimizeRoute: (startingAttractionId: UUID) => Promise<OptimizeRouteResponseDTO>;
}
```

### Stan lokalny w `TripDetailsPage`

```typescript
// Modal state
const [showOptimizeModal, setShowOptimizeModal] = useState(false);

// Toast state
const [optimizationResult, setOptimizationResult] = useState<{
  isVisible: boolean;
  totalDistance: number;
} | null>(null);
```

### Przepływ stanu

1. Użytkownik klika "Optymalizuj trasę"
2. `showOptimizeModal = true`
3. Użytkownik wybiera atrakcję startową
4. Wywołanie `optimizeRoute(startingAttractionId)`
5. `isOptimizing = true` (spinner w modalu)
6. Po sukces:
   - `showOptimizeModal = false`
   - `attractions` zaktualizowane z nową kolejnością
   - `optimizationResult = { isVisible: true, totalDistance: response.totalDistance }`
7. Toast auto-hide po 5 sekundach

## 7. Integracja API

### Funkcja API

Dodać do `api/trips.ts`:

```typescript
/**
 * Optymalizuje trasę wycieczki używając algorytmu najbliższego sąsiada
 * POST /api/trips/{tripId}/optimize-route
 */
export async function optimizeRoute(
  tripId: UUID,
  command: OptimizeRouteCommand
): Promise<OptimizeRouteResponseDTO> {
  const response = await apiClient.post<OptimizeRouteResponseDTO>(
    `/api/trips/${tripId}/optimize-route`,
    command
  );
  return response.data;
}
```

### Typy żądania i odpowiedzi

**Request:**
```typescript
// POST /api/trips/{tripId}/optimize-route
// Headers: Authorization: Bearer {token}
// Body:
{
  "startingAttractionId": "uuid"
}
```

**Response (200 OK):**
```typescript
{
  "tripId": "uuid",
  "optimizedOrder": [
    {
      "attractionId": "uuid",
      "attractionName": "Acropolis of Athens",
      "dayNumber": 1,
      "orderIndex": 1
    }
  ],
  "totalDistance": 12500.5,
  "optimizedAt": "2026-01-22T11:00:00Z"
}
```

## 8. Interakcje użytkownika

### Scenariusz główny

1. **Użytkownik widzi przycisk "Optymalizuj trasę"**
   - Przycisk jest widoczny po lewej stronie od "Dodaj atrakcje"
   - Przycisk jest aktywny gdy są minimum 2 atrakcje

2. **Użytkownik klika przycisk**
   - Otwiera się modal z listą atrakcji
   - Domyślnie żadna atrakcja nie jest wybrana

3. **Użytkownik wybiera atrakcję startową**
   - Klika na radio button przy wybranej atrakcji
   - Przycisk "Optymalizuj" staje się aktywny

4. **Użytkownik potwierdza wybór**
   - Klika "Optymalizuj"
   - Modal pokazuje spinner
   - API jest wywoływane

5. **Sukces optymalizacji**
   - Modal się zamyka
   - Lista atrakcji jest posortowana w nowej kolejności
   - Pojawia się toast z informacją o dystansie

6. **Użytkownik zamyka toast**
   - Toast znika po 5 sekundach automatycznie
   - Lub użytkownik klika X

### Scenariusze alternatywne

**Anulowanie:**
- Użytkownik klika "Anuluj" w modalu → modal się zamyka, bez zmian

**Błąd API:**
- Toast błędu z komunikatem
- Modal się zamyka
- Atrakcje pozostają w poprzedniej kolejności

## 9. Warunki i walidacja

### Warunki wyświetlania przycisku

| Warunek | Wpływ na UI |
|---------|-------------|
| `attractions.length < 2` | Przycisk disabled z tooltip "Dodaj minimum 2 atrakcje" |
| `isSaving === true` | Przycisk disabled |
| `isOptimizing === true` | Przycisk disabled ze spinnerem |

### Walidacja w modalu

| Warunek | Wpływ na UI |
|---------|-------------|
| Nie wybrano atrakcji | Przycisk "Optymalizuj" disabled |
| Trwa optymalizacja | Oba przyciski disabled, spinner |

### Walidacja po stronie API

| Kod błędu | Warunek | Obsługa |
|-----------|---------|---------|
| 400 | `startingAttractionId` nie jest w tripie | Toast z błędem |
| 401 | Brak autoryzacji | Redirect do logowania |
| 403 | Nie jesteś właścicielem | Toast z błędem |
| 404 | Trip nie istnieje | Redirect do listy wycieczek |
| 422 | Brak atrakcji do optymalizacji | Toast z błędem |

## 10. Obsługa błędów

### Błędy sieciowe

```typescript
try {
  const result = await optimizeRoute(tripId, { startingAttractionId });
  // sukces
} catch (error) {
  if (error.response?.status === 401) {
    // Przekieruj do logowania
    navigate('/login');
  } else if (error.response?.status === 403) {
    setError('Nie masz uprawnień do edycji tej wycieczki');
  } else if (error.response?.status === 404) {
    setError('Wycieczka nie została znaleziona');
    navigate('/trips');
  } else if (error.response?.status === 422) {
    setError('Dodaj atrakcje do wycieczki przed optymalizacją');
  } else {
    setError('Wystąpił błąd podczas optymalizacji trasy');
  }
}
```

### Wyświetlanie błędów

- Błędy walidacji: Toast z czerwonym tłem
- Błędy autoryzacji: Przekierowanie + toast informacyjny
- Błędy serwera: Toast z komunikatem ogólnym + możliwość ponowienia

### Fallback UI

- Podczas ładowania: Spinner w przycisku i modalu
- Po błędzie: Poprzedni stan atrakcji zachowany (optimistic update revert)

## 11. Kroki implementacji

### Krok 1: Dodanie funkcji API

1. Otworzyć `frontend/src/api/trips.ts`
2. Dodać import typów `OptimizeRouteCommand`, `OptimizeRouteResponseDTO`
3. Dodać funkcję `optimizeRoute(tripId, command)`

### Krok 2: Dodanie nowych typów props

1. Otworzyć `frontend/src/@types.ts`
2. Dodać interfejsy:
   - `OptimizeRouteButtonProps`
   - `SelectStartingAttractionModalProps`
   - `AttractionRadioItemProps`
   - `OptimizationResultToastProps`

### Krok 3: Utworzenie komponentu AttractionRadioItem

1. Utworzyć `frontend/src/components/trips/AttractionRadioItem.tsx`
2. Zaimplementować komponent z radio button i informacjami o atrakcji
3. Zastosować style Tailwind CSS

### Krok 4: Utworzenie komponentu SelectStartingAttractionModal

1. Utworzyć `frontend/src/components/trips/SelectStartingAttractionModal.tsx`
2. Użyć istniejącego komponentu `Modal`
3. Zaimplementować listę z `AttractionRadioItem`
4. Dodać obsługę stanu wyboru i przycisków akcji

### Krok 5: Utworzenie komponentu OptimizeRouteButton

1. Utworzyć `frontend/src/components/trips/OptimizeRouteButton.tsx`
2. Zaimplementować przycisk z ikoną i tekstem
3. Dodać obsługę stanów disabled i loading

### Krok 6: Utworzenie komponentu OptimizationResultToast

1. Utworzyć `frontend/src/components/trips/OptimizationResultToast.tsx`
2. Zaimplementować toast z animacją i auto-hide
3. Dodać formatowanie dystansu (m/km)

### Krok 7: Rozszerzenie hooka useTripDetails

1. Otworzyć `frontend/src/hooks/useTripDetails.ts`
2. Dodać stan `isOptimizing`
3. Dodać funkcję `optimizeRoute` z wywołaniem API
4. Zaimplementować aktualizację kolejności atrakcji po sukcesie

### Krok 8: Integracja w TripDetailsPage

1. Otworzyć `frontend/src/pages/TripDetailsPage.tsx`
2. Zaimportować nowe komponenty
3. Dodać stan dla modalu i toasta
4. Dodać `OptimizeRouteButton` obok `AddAttractionButton`
5. Dodać `SelectStartingAttractionModal`
6. Dodać `OptimizationResultToast`
7. Połączyć wszystko z hookiem i obsługą zdarzeń

### Krok 9: Testy manualne

1. Sprawdzić czy przycisk jest disabled przy < 2 atrakcjach
2. Sprawdzić wybór atrakcji startowej w modalu
3. Zweryfikować poprawność kolejności po optymalizacji
4. Przetestować obsługę błędów
5. Sprawdzić responsywność na urządzeniach mobilnych

### Krok 10: Finalizacja

1. Code review
2. Sprawdzenie dostępności (a11y)
3. Weryfikacja typów TypeScript
4. Czyszczenie kodu i formatowanie
