Jako starszy programista frontendu Twoim zadaniem jest stworzenie szczegółowego planu wdrożenia nowego widoku w aplikacji internetowej. Plan ten powinien być kompleksowy i wystarczająco jasny dla innego programisty frontendowego, aby mógł poprawnie i wydajnie wdrożyć widok.

Najpierw przejrzyj następujące informacje:

1. Product Requirements Document (PRD):
<prd>
{{./.ai/prd.md}}
</prd>

2. Opis widoku:
<view_description>
### 2.7. Przeglądanie atrakcji

| Atrybut | Wartość |
|---------|---------|
| **Ścieżka** | `/attractions` |
| **Główny cel** | Odkrywanie atrakcji turystycznych |
| **Dostęp** | Publiczny |

**Kluczowe informacje do wyświetlenia:**
- Lista kart atrakcji
- Filtr lokalizacji (dropdown)
- Paginacja (10 atrakcji na stronę)
- Przycisk "Dodaj własną atrakcję" (tylko dla zalogowanych)
- Empty state dla braku wyników

**Kluczowe komponenty:**
- `AttractionList` - grid kart atrakcji
- `AttractionCard` - karta atrakcji (obrazek, nazwa, opis, lokalizacja)
- `LocationFilter` - dropdown filtra lokalizacji
- `Pagination` - nawigacja stron (Poprzednia/Następna + numery)
- `CreateAttractionButton` - przycisk tworzenia (warunkowo)
- `SkeletonAttractionCard` - placeholder podczas ładowania
- `EmptyState` - komunikat dla braku wyników

**Względy UX/A11y/Security:**
- Skeleton loaders podczas ładowania
- Filtrowanie po stronie serwera
- Karty jako linki z alt text dla obrazków
- Badge "Własna" dla atrakcji użytkownika
- Publiczny dostęp do przeglądania

**Mapowanie API:**
- `GET /api/attractions?locationId=...&page=...&pageSize=10` → lista atrakcji
- `GET /api/locations` → lista lokalizacji dla filtra
</view_description>

3. User Stories:
<user_stories>
### 3.2. Wyszukiwanie i wybór atrakcji
- **FR-04:** Użytkownik może wyszukać miasto/lokalizację (np. "Ateny, Grecja")
- **FR-05:** System pobiera listę atrakcji zdefiniowaną w bazie danych dla danej lokalizacji
- **FR-06:** Dla każdej atrakcji wyświetlane są: nazwa, ocena, liczba opinii, szacowany czas zwiedzania
- **FR-09:** Użytkownik może ręcznie dodać własną atrakcję do bazy
</user_stories>

4. Endpoint Description:
<endpoint_description>
### 2.4. Attractions

#### GET /api/attractions

List attractions with filtering, sorting, and pagination.

**Query Parameters:**
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| locationId | UUID | No | Filter by location |
| search | string | No | Search by name |
| sortBy | string | No | Sort field: `rating`, `name`, `reviewCount` (default: `rating`) |
| sortOrder | string | No | `asc` or `desc` (default: `desc`) |
| isVerified | bool | No | Filter verified/unverified attractions |
| page | int | No | Page number (default: 1) |
| pageSize | int | No | Items per page (default: 20, max: 100) |

**Response (200 OK):**
```json
{
  "items": [
    {
      "id": "uuid",
      "locationId": "uuid",
      "name": "Acropolis of Athens",
      "description": "Ancient citadel located on a rocky outcrop...",
      "latitude": 37.9715323,
      "longitude": 23.7257492,
      "rating": 4.8,
      "reviewCount": 95420,
      "estimatedDuration": 180,
      "imageUrl": "https://example.com/acropolis.jpg",
      "isVerified": true,
      "createdByUserId": null
    }
  ],
  "pagination": {
    "page": 1,
    "pageSize": 20,
    "totalItems": 45,
    "totalPages": 3
  }
}
```

---

#### GET /api/attractions/{id}

Get a specific attraction by ID.

**Path Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| id | UUID | Attraction identifier |

**Response (200 OK):**
```json
{
  "id": "uuid",
  "locationId": "uuid",
  "location": {
    "id": "uuid",
    "name": "Athens",
    "country": "Greece"
  },
  "name": "Acropolis of Athens",
  "description": "Ancient citadel located on a rocky outcrop...",
  "latitude": 37.9715323,
  "longitude": 23.7257492,
  "rating": 4.8,
  "reviewCount": 95420,
  "estimatedDuration": 180,
  "imageUrl": "https://example.com/acropolis.jpg",
  "isVerified": true,
  "createdByUserId": null,
  "createdAt": "2026-01-01T00:00:00Z",
  "updatedAt": "2026-01-01T00:00:00Z"
}
```

**Error Codes:**
| Code | Description |
|------|-------------|
| 404 | Attraction not found |

---

#### POST /api/attractions

Create a custom attraction (user-generated).

**Headers:**
- `Authorization: Bearer {token}`

**Request Body:**
```json
{
  "locationId": "uuid",
  "name": "Hidden Coffee Shop",
  "description": "A great local coffee shop...",
  "latitude": 37.9750,
  "longitude": 23.7350,
  "estimatedDuration": 45,
  "imageUrl": "https://example.com/coffee.jpg"
}
```

**Response (201 Created):**
```json
{
  "id": "uuid",
  "locationId": "uuid",
  "name": "Hidden Coffee Shop",
  "description": "A great local coffee shop...",
  "latitude": 37.9750,
  "longitude": 23.7350,
  "rating": null,
  "reviewCount": null,
  "estimatedDuration": 45,
  "imageUrl": "https://example.com/coffee.jpg",
  "isVerified": false,
  "createdByUserId": "uuid",
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

#### PUT /api/attractions/{id}

Update a custom attraction (only owner can update).

**Headers:**
- `Authorization: Bearer {token}`

**Path Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| id | UUID | Attraction identifier |

**Request Body:**
```json
{
  "name": "Amazing Coffee Shop",
  "description": "Updated description...",
  "latitude": 37.9751,
  "longitude": 23.7351,
  "estimatedDuration": 60,
  "imageUrl": "https://example.com/coffee-updated.jpg"
}
```

**Response (200 OK):**
```json
{
  "id": "uuid",
  "locationId": "uuid",
  "name": "Amazing Coffee Shop",
  "description": "Updated description...",
  "latitude": 37.9751,
  "longitude": 23.7351,
  "rating": null,
  "reviewCount": null,
  "estimatedDuration": 60,
  "imageUrl": "https://example.com/coffee-updated.jpg",
  "isVerified": false,
  "createdByUserId": "uuid",
  "createdAt": "2026-01-22T10:00:00Z",
  "updatedAt": "2026-01-22T11:00:00Z"
}
```

**Error Codes:**
| Code | Description |
|------|-------------|
| 400 | Validation error |
| 401 | Not authenticated |
| 403 | Cannot modify attraction created by another user |
| 404 | Attraction not found |

---

#### DELETE /api/attractions/{id}

Delete a custom attraction (only owner can delete).

**Headers:**
- `Authorization: Bearer {token}`

**Path Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| id | UUID | Attraction identifier |

**Response (204 No Content)**

**Error Codes:**
| Code | Description |
|------|-------------|
| 401 | Not authenticated |
| 403 | Cannot delete attraction created by another user |
| 404 | Attraction not found |
| 409 | Attraction is used in other users' trips |

---

</endpoint_description>

5. Endpoint Implementation:
<endpoint_implementation>

</endpoint_implementation>

6. Type Definitions:
<type_definitions>
{{./frontend\src/@types.ts}} <- zamień na referencję do pliku z definicjami DTOsów (np. @types.ts)
</type_definitions>

7. Tech Stack:
<tech_stack>
{{./.ai/stach-tech.md}}
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