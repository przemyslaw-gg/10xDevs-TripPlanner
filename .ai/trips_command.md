Jako starszy programista frontendu Twoim zadaniem jest stworzenie szczegółowego planu wdrożenia nowego widoku w aplikacji internetowej. Plan ten powinien być kompleksowy i wystarczająco jasny dla innego programisty frontendowego, aby mógł poprawnie i wydajnie wdrożyć widok.

Najpierw przejrzyj następujące informacje:

1. Product Requirements Document (PRD):
<prd>
{{./.ai/prd.md}}
</prd>

2. Opis widoku:
<view_description>
### 2.4. Lista wycieczek

| Atrybut | Wartość |
|---------|---------|
| **Ścieżka** | `/trips` |
| **Główny cel** | Przegląd i zarządzanie wycieczkami użytkownika |
| **Dostęp** | Wymagane logowanie |

**Kluczowe informacje do wyświetlenia:**
- Lista kart wycieczek użytkownika
- Nazwa, data, liczba atrakcji dla każdej wycieczki
- Przycisk tworzenia nowej wycieczki
- Empty state dla braku wycieczek

**Kluczowe komponenty:**
- `TripList` - kontener listy
- `TripCard` - karta pojedynczej wycieczki
- `CreateTripButton` - przycisk tworzenia
- `EmptyState` - komunikat dla braku wycieczek
- `SkeletonTripCard` - placeholder podczas ładowania

**Względy UX/A11y/Security:**
- Skeleton loaders podczas ładowania
- Karty jako klikalne linki (keyboard accessible)
- Focus visible dla nawigacji klawiaturą
- Tylko własne wycieczki (zabezpieczone przez API)

**Mapowanie API:**
- `GET /api/trips?onlyMine=true` → lista wycieczek

---

### 2.5. Tworzenie wycieczki

| Atrybut | Wartość |
|---------|---------|
| **Ścieżka** | `/trips/new` |
| **Główny cel** | Utworzenie nowej wycieczki |
| **Dostęp** | Wymagane logowanie |

**Kluczowe informacje do wyświetlenia:**
- Pole nazwy wycieczki (1-100 znaków)
- Licznik znaków (widoczny od 80 znaków)
- Pole wyboru daty
- Przyciski: Utwórz, Anuluj

**Kluczowe komponenty:**
- `TripForm` - formularz tworzenia
- `CharacterCounter` - licznik znaków
- `DatePicker` - wybór daty
- `FormActions` - przyciski akcji

**Względy UX/A11y/Security:**
- Walidacja min. 1 znak dla nazwy
- Data dowolna (przeszła i przyszła dozwolona)
- Focus na polu nazwy przy wejściu
- Redirect do `/trips/:id` po utworzeniu

**Mapowanie API:**
- `POST /api/trips` → utworzenie wycieczki

---

### 2.6. Szczegóły/Edycja wycieczki

| Atrybut | Wartość |
|---------|---------|
| **Ścieżka** | `/trips/:id` |
| **Główny cel** | Przeglądanie i edycja wycieczki |
| **Dostęp** | Wymagane logowanie (tylko właściciel) |

**Kluczowe informacje do wyświetlenia:**
- Nazwa wycieczki (edytowalna inline)
- Data wycieczki (edytowalna)
- Lista atrakcji z możliwością zmiany kolejności
- Licznik atrakcji (X/20)
- Status zapisywania ("Zapisywanie..." / "Zapisano")
- Przycisk dodawania atrakcji
- Przycisk usunięcia wycieczki

**Kluczowe komponenty:**
- `TripHeader` - edytowalna nazwa i data
- `SaveStatusIndicator` - wskaźnik autosave
- `TripAttractionList` - lista atrakcji z drag & drop
- `AttractionItem` - element listy z przyciskami up/down/remove
- `AddAttractionButton` - link do przeglądania atrakcji
- `DeleteTripButton` - przycisk usunięcia (danger)
- `ConfirmDeleteModal` - modal potwierdzenia usunięcia

**Względy UX/A11y/Security:**
- Autosave z 2s debounce
- Keyboard-accessible reordering (przyciski góra/dół jako fallback)
- Aria-live dla statusu zapisywania
- Confirmation dialog przed usunięciem
- Max 20 atrakcji - disabled "Dodaj" przy limicie
- Tylko właściciel może edytować (403 przy braku uprawnień)

**Mapowanie API:**
- `GET /api/trips/:id` → dane wycieczki
- `PUT /api/trips/:id` → aktualizacja (autosave)
- `DELETE /api/trips/:id` → usunięcie
- `GET /api/trips/:id/attractions` → lista atrakcji
- `POST /api/trips/:id/attractions/reorder` → zmiana kolejności
- `DELETE /api/trips/:id/attractions/:attractionId` → usunięcie atrakcji
</view_description>

3. User Stories:
<user_stories>
### 3.5. Zapisywanie i edycja planów
- **FR-20:** Użytkownik może zapisać stworzony plan wycieczki z nazwą (np. "Ateny 2026")
- **FR-21:** System przechowuje wszystkie plany użytkownika
- **FR-22:** Użytkownik może przeglądać listę swoich zapisanych planów
- **FR-23:** Użytkownik może edytować istniejący plan (dodawać/usuwać atrakcje, zmieniać kolejność)
- **FR-24:** Użytkownik może usunąć plan wycieczki
- **FR-24:** Użytkownik może opublikować plan wycieczki aby był dostępny dla innych osób
</user_stories>

4. Endpoint Description:
<endpoint_description>
### 2.5. Trips

#### GET /api/trips

List user's trips and public trips from other users.

**Headers:**
- `Authorization: Bearer {token}`

**Query Parameters:**
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| locationId | UUID | No | Filter by location |
| onlyMine | bool | No | Show only user's own trips (default: false) |
| onlyPublic | bool | No | Show only public trips (default: false) |
| search | string | No | Search by trip name |
| page | int | No | Page number (default: 1) |
| pageSize | int | No | Items per page (default: 20, max: 100) |

**Response (200 OK):**
```json
{
  "items": [
    {
      "id": "uuid",
      "ownerId": "uuid",
      "name": "Athens 2026",
      "locationId": "uuid",
      "location": {
        "id": "uuid",
        "name": "Athens",
        "country": "Greece"
      },
      "isPublic": false,
      "dailyHours": 8,
      "maxExtensionHours": 2,
      "startTime": "09:00:00",
      "attractionCount": 12,
      "totalDays": 3,
      "isOwner": true,
      "createdAt": "2026-01-22T10:00:00Z",
      "updatedAt": "2026-01-22T10:00:00Z"
    }
  ],
  "pagination": {
    "page": 1,
    "pageSize": 20,
    "totalItems": 5,
    "totalPages": 1
  }
}
```

**Error Codes:**
| Code | Description |
|------|-------------|
| 401 | Not authenticated |

---

#### GET /api/trips/{id}

Get a specific trip with full details.

**Headers:**
- `Authorization: Bearer {token}`

**Path Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| id | UUID | Trip identifier |

**Response (200 OK):**
```json
{
  "id": "uuid",
  "ownerId": "uuid",
  "name": "Athens 2026",
  "locationId": "uuid",
  "location": {
    "id": "uuid",
    "name": "Athens",
    "country": "Greece",
    "timezone": "Europe/Athens"
  },
  "isPublic": false,
  "dailyHours": 8,
  "maxExtensionHours": 2,
  "startTime": "09:00:00",
  "isOwner": true,
  "createdAt": "2026-01-22T10:00:00Z",
  "updatedAt": "2026-01-22T10:00:00Z"
}
```

**Error Codes:**
| Code | Description |
|------|-------------|
| 401 | Not authenticated |
| 403 | Trip is private and not owned by user |
| 404 | Trip not found |

---

#### POST /api/trips

Create a new trip.

**Headers:**
- `Authorization: Bearer {token}`

**Request Body:**
```json
{
  "name": "Athens 2026",
  "locationId": "uuid",
  "dailyHours": 8,
  "maxExtensionHours": 2,
  "startTime": "09:00"
}
```

**Response (201 Created):**
```json
{
  "id": "uuid",
  "ownerId": "uuid",
  "name": "Athens 2026",
  "locationId": "uuid",
  "isPublic": false,
  "dailyHours": 8,
  "maxExtensionHours": 2,
  "startTime": "09:00:00",
  "createdAt": "2026-01-22T10:00:00Z",
  "updatedAt": "2026-01-22T10:00:00Z"
}
```

**Error Codes:**
| Code | Description |
|------|-------------|
| 400 | Validation error |
| 401 | Not authenticated |
| 404 | Location not found |

---

#### PUT /api/trips/{id}

Update an existing trip (only owner can update).

**Headers:**
- `Authorization: Bearer {token}`

**Path Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| id | UUID | Trip identifier |

**Request Body:**
```json
{
  "name": "Athens Summer 2026",
  "locationId": "uuid",
  "dailyHours": 10,
  "maxExtensionHours": 3,
  "startTime": "08:00"
}
```

**Response (200 OK):**
```json
{
  "id": "uuid",
  "ownerId": "uuid",
  "name": "Athens Summer 2026",
  "locationId": "uuid",
  "isPublic": false,
  "dailyHours": 10,
  "maxExtensionHours": 3,
  "startTime": "08:00:00",
  "createdAt": "2026-01-22T10:00:00Z",
  "updatedAt": "2026-01-22T11:00:00Z"
}
```

**Error Codes:**
| Code | Description |
|------|-------------|
| 400 | Validation error |
| 401 | Not authenticated |
| 403 | Cannot modify trip owned by another user |
| 404 | Trip or location not found |

---

#### DELETE /api/trips/{id}

Delete a trip (only owner can delete).

**Headers:**
- `Authorization: Bearer {token}`

**Path Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| id | UUID | Trip identifier |

**Response (204 No Content)**

**Error Codes:**
| Code | Description |
|------|-------------|
| 401 | Not authenticated |
| 403 | Cannot delete trip owned by another user |
| 404 | Trip not found |
</endpoint_description>

5. Endpoint Implementation:
<endpoint_implementation>
Szczegóły implementacji kontrolerów możesz wyciągnąć z projektu
</endpoint_implementation>

6. Type Definitions:
<type_definitions>

</type_definitions>

7. Tech Stack:
<tech_stack>
{{./.ai/stack-tech.md}}
</tech_stack>

Przed utworzeniem ostatecznego planu wdrożenia przeprowadź analizę i planowanie wewnątrz tagów <implementation_breakdown> w swoim bloku myślenia. Ta sekcja może być dość długa, ponieważ ważne jest, aby być dokładnym.

W swoim podziale implementacji wykonaj następujące kroki:
1. Dla każdej sekcji wejściowej (PRD, User Stories, Endpoint Description, Endpoint Implementation, Type Definitions, Tech Stack):
  - Podsumuj kluczowe punkty
 - Wymień wszelkie wymagania lub ograniczenia
 - Zwróć uwagę na wszelkie potencjalne wyzwania lub ważne kwestie
2. Wyodrębnienie i wypisanie kluczowych wymagań z PRD
3. Wypisanie wszystkich potrzebnych głównych komponentów, wraz z krótkim opisem ich opisu, potrzebnych typów, obsługiwanych zdarzeń i warunków walidacji
4. Stworzenie wysokopoziomowego diagramu drzewa komponentów
5. Zidentyfikuj wymagane DTO i niestandardowe typy ViewModel dla każdego komponentu widoku. Szczegółowo wyjaśnij te nowe typy, dzieląc ich pola i powiązane typy.
6. Zidentyfikuj potencjalne zmienne stanu i niestandardowe hooki, wyjaśniając ich cel i sposób ich użycia
7. Wymień wymagane wywołania API i odpowiadające im akcje frontendowe
8. Zmapuj każdej historii użytkownika do konkretnych szczegółów implementacji, komponentów lub funkcji
9. Wymień interakcje użytkownika i ich oczekiwane wyniki
10. Wymień warunki wymagane przez API i jak je weryfikować na poziomie komponentów
11. Zidentyfikuj potencjalne scenariusze błędów i zasugeruj, jak sobie z nimi poradzić
12. Wymień potencjalne wyzwania związane z wdrożeniem tego widoku i zasugeruj możliwe rozwiązania

Po przeprowadzeniu analizy dostarcz plan wdrożenia w formacie Markdown z następującymi sekcjami:

1. Przegląd: Krótkie podsumowanie widoku i jego celu.
2. Routing widoku: Określenie ścieżki, na której widok powinien być dostępny.
3. Struktura komponentów: Zarys głównych komponentów i ich hierarchii.
4. Szczegóły komponentu: Dla każdego komponentu należy opisać:
 - Opis komponentu, jego przeznaczenie i z czego się składa
 - Główne elementy HTML i komponenty dzieci, które budują komponent
 - Obsługiwane zdarzenia
 - Warunki walidacji (szczegółowe warunki, zgodnie z API)
 - Typy (DTO i ViewModel) wymagane przez komponent
 - Propsy, które komponent przyjmuje od rodzica (interfejs komponentu)
5. Typy: Szczegółowy opis typów wymaganych do implementacji widoku, w tym dokładny podział wszelkich nowych typów lub modeli widoku według pól i typów.
6. Zarządzanie stanem: Szczegółowy opis sposobu zarządzania stanem w widoku, określenie, czy wymagany jest customowy hook.
7. Integracja API: Wyjaśnienie sposobu integracji z dostarczonym punktem końcowym. Precyzyjnie wskazuje typy żądania i odpowiedzi.
8. Interakcje użytkownika: Szczegółowy opis interakcji użytkownika i sposobu ich obsługi.
9. Warunki i walidacja: Opisz jakie warunki są weryfikowane przez interfejs, których komponentów dotyczą i jak wpływają one na stan interfejsu
10. Obsługa błędów: Opis sposobu obsługi potencjalnych błędów lub przypadków brzegowych.
11. Kroki implementacji: Przewodnik krok po kroku dotyczący implementacji widoku.

Upewnij się, że Twój plan jest zgodny z PRD, historyjkami użytkownika i uwzględnia dostarczony stack technologiczny.

Ostateczne wyniki powinny być w języku polskim i zapisane w pliku o nazwie .ai/{view-name}-view-implementation-plan.md. Nie uwzględniaj żadnej analizy i planowania w końcowym wyniku.

Oto przykład tego, jak powinien wyglądać plik wyjściowy (treść jest do zastąpienia):

```markdown
# Plan implementacji widoku [Nazwa widoku]

## 1. Przegląd
[Krótki opis widoku i jego celu]

## 2. Routing widoku
[Ścieżka, na której widok powinien być dostępny]

## 3. Struktura komponentów
[Zarys głównych komponentów i ich hierarchii]

## 4. Szczegóły komponentów
### [Nazwa komponentu 1]
- Opis komponentu [opis]
- Główne elementy: [opis]
- Obsługiwane interakcje: [lista]
- Obsługiwana walidacja: [lista, szczegółowa]
- Typy: [lista]
- Propsy: [lista]

### [Nazwa komponentu 2]
[...]

## 5. Typy
[Szczegółowy opis wymaganych typów]

## 6. Zarządzanie stanem
[Opis zarządzania stanem w widoku]

## 7. Integracja API
[Wyjaśnienie integracji z dostarczonym endpointem, wskazanie typów żądania i odpowiedzi]

## 8. Interakcje użytkownika
[Szczegółowy opis interakcji użytkownika]

## 9. Warunki i walidacja
[Szczegółowy opis warunków i ich walidacji]

## 10. Obsługa błędów
[Opis obsługi potencjalnych błędów]

## 11. Kroki implementacji
1. [Krok 1]
2. [Krok 2]
3. [...]
```

Rozpocznij analizę i planowanie już teraz. Twój ostateczny wynik powinien składać się wyłącznie z planu wdrożenia w języku polskim w formacie markdown, który zapiszesz w pliku .ai/{view-name}-view-implementation-plan.md i nie powinien powielać ani powtarzać żadnej pracy wykonanej w podziale implementacji.