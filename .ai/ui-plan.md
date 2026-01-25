# Architektura UI dla TripPlanner

## 1. Przegląd struktury UI

TripPlanner to aplikacja webowa SPA (Single Page Application) do planowania jednodniowych wycieczek turystycznych. Aplikacja jest zbudowana w oparciu o React 19 z TypeScript, wykorzystując bibliotekę komponentów shadcn/ui oraz TanStack Query do zarządzania stanem serwera.

### Główne założenia architektury:

- **Platforma docelowa:** Desktop i tablet (brak optymalizacji mobilnej)
- **Język interfejsu:** Polski
- **Typ aplikacji:** SPA (Vite + React) - niezależna od backendu .NET
- **Styl nawigacji:** Top navbar ze sticky header + breadcrumbs
- **Zarządzanie stanem:** TanStack Query (server state) + React Context (auth state)
- **Responsywność:** Breakpoints dla desktop (1200px+) i tablet (768px-1199px)

### Ograniczenia MVP:

- Wycieczki tylko jednodniowe
- Brak integracji z mapami
- Brak trybu offline
- Brak udostępniania wycieczek
- Brak wyświetlania: rating, reviewCount, estimatedDuration
- Brak sortowania list
- Brak globalnego wyszukiwania

---

## 2. Lista widoków

### 2.1. Strona główna (Landing/Dashboard)

| Atrybut | Wartość |
|---------|---------|
| **Ścieżka** | `/` |
| **Główny cel** | Punkt wejścia do aplikacji - różny widok dla zalogowanych i niezalogowanych |
| **Dostęp** | Publiczny |

**Kluczowe informacje do wyświetlenia:**

*Dla niezalogowanych:*
- Opis aplikacji i jej głównych funkcji
- Zachęta do rejestracji/logowania

*Dla zalogowanych:*
- Powitanie użytkownika
- Liczba posiadanych wycieczek
- Lista ostatnich wycieczek (max 3)
- Szybkie akcje (utwórz wycieczkę, przeglądaj atrakcje)

**Kluczowe komponenty:**
- `HeroSection` - sekcja powitalna z CTA
- `TripPreviewList` - lista ostatnich wycieczek (dla zalogowanych)
- `QuickActions` - przyciski szybkich akcji

**Względy UX/A11y/Security:**
- Focus na głównym CTA przy wejściu
- Różne widoki na podstawie stanu autentykacji
- Semantic landmarks (`<main>`, `<section>`)

---

### 2.2. Logowanie

| Atrybut | Wartość |
|---------|---------|
| **Ścieżka** | `/login` |
| **Główny cel** | Uwierzytelnienie istniejącego użytkownika |
| **Dostęp** | Publiczny (redirect do `/` jeśli zalogowany) |

**Kluczowe informacje do wyświetlenia:**
- Formularz logowania (email, hasło)
- Checkbox "Zapamiętaj mnie"
- Link do rejestracji

**Kluczowe komponenty:**
- `LoginForm` - formularz z walidacją
- `PasswordInput` - pole hasła z toggle visibility
- `RememberMeCheckbox` - opcja zapamiętania sesji

**Względy UX/A11y/Security:**
- Auto-focus na polu email
- Nie ujawniać czy email istnieje w systemie (generyczny komunikat błędu)
- Hasło domyślnie zamaskowane
- Labels powiązane z polami (htmlFor)
- Aria-describedby dla komunikatów błędów

**Mapowanie API:**
- `POST /api/auth/login` → JWT token

---

### 2.3. Rejestracja

| Atrybut | Wartość |
|---------|---------|
| **Ścieżka** | `/register` |
| **Główny cel** | Utworzenie nowego konta użytkownika |
| **Dostęp** | Publiczny (redirect do `/` jeśli zalogowany) |

**Kluczowe informacje do wyświetlenia:**
- Formularz rejestracji (email, hasło, potwierdzenie hasła)
- Wskaźnik siły hasła
- Wymagania dotyczące hasła
- Link do logowania

**Kluczowe komponenty:**
- `RegisterForm` - formularz z walidacją
- `PasswordInput` - pole hasła z toggle visibility
- `PasswordStrengthIndicator` - wizualny wskaźnik siły hasła
- `PasswordRequirements` - lista wymagań (min 8 znaków, wielka litera, cyfra)

**Względy UX/A11y/Security:**
- Real-time walidacja siły hasła
- Aria-live dla aktualizacji wskaźnika siły
- Walidacja on blur + on submit
- Wymagania hasła: min 8 znaków, 1 wielka litera, 1 cyfra

**Mapowanie API:**
- `POST /api/auth/register` → utworzenie konta + auto-login

---

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

---

### 2.7. Przeglądanie atrakcji

| Atrybut | Wartość |
|---------|---------|
| **Ścieżka** | `/attractions` lub `/attractions?tripId={id}` |
| **Główny cel** | Odkrywanie atrakcji turystycznych / Dodawanie atrakcji do wycieczki |
| **Dostęp** | Publiczny (tryb przeglądania) / Wymagane logowanie (tryb dodawania) |

**Tryby działania:**

1. **Tryb przeglądania** (`/attractions`):
   - Standardowe przeglądanie atrakcji
   - Karty klikalne jako linki do szczegółów (planowane)
   - Przycisk "Dodaj własną atrakcję" (dla zalogowanych)

2. **Tryb dodawania do wycieczki** (`/attractions?tripId={id}`):
   - Nagłówek z nazwą wycieczki i linkiem powrotu
   - Licznik atrakcji w wycieczce (X/20)
   - Karty z przyciskiem "Dodaj do wycieczki"
   - Po kliknięciu atrakcja dodawana bezpośrednio do wycieczki
   - Blokada dodawania po osiągnięciu limitu 20 atrakcji

**Kluczowe informacje do wyświetlenia:**
- Lista kart atrakcji
- Filtr lokalizacji (dropdown)
- Paginacja (10 atrakcji na stronę)
- Przycisk "Dodaj własną atrakcję" (tylko tryb przeglądania + zalogowany)
- Empty state dla braku wyników
- *Tryb dodawania:* Nazwa wycieczki, licznik atrakcji, link powrotu

**Kluczowe komponenty:**
- `AttractionList` - grid kart atrakcji
- `AttractionCard` - karta atrakcji z opcjonalnym przyciskiem "Dodaj"
- `LocationFilter` - dropdown filtra lokalizacji
- `Pagination` - nawigacja stron (Poprzednia/Następna + numery)
- `CreateAttractionButton` - przycisk tworzenia (warunkowo)
- `SkeletonAttractionCard` - placeholder podczas ładowania
- `EmptyState` - komunikat dla braku wyników

**Względy UX/A11y/Security:**
- Skeleton loaders podczas ładowania
- Filtrowanie po stronie serwera
- W trybie przeglądania: karty jako linki z alt text dla obrazków
- W trybie dodawania: karty z przyciskiem akcji, feedback "Dodano do wycieczki"
- Badge "Własna" dla atrakcji użytkownika
- Publiczny dostęp do przeglądania
- Tylko właściciel wycieczki może dodawać atrakcje

**Mapowanie API:**
- `GET /api/attractions?locationId=...&page=...&pageSize=10` → lista atrakcji
- `GET /api/locations` → lista lokalizacji dla filtra
- `GET /api/trips/{tripId}` → dane wycieczki (tryb dodawania)
- `GET /api/trips/{tripId}/attractions` → liczba atrakcji w wycieczce
- `POST /api/trips/{tripId}/attractions` → dodanie atrakcji do wycieczki

---

### 2.8. Szczegóły atrakcji (Post-MVP)

> **Uwaga:** Ten widok jest planowany do implementacji po MVP. Obecnie dodawanie atrakcji do wycieczki odbywa się bezpośrednio z listy atrakcji (sekcja 2.7).

| Atrybut | Wartość |
|---------|---------|
| **Ścieżka** | `/attractions/:id` |
| **Główny cel** | Wyświetlenie pełnych informacji o atrakcji |
| **Dostęp** | Publiczny |
| **Status** | Planowane (Post-MVP) |

**Kluczowe informacje do wyświetlenia:**
- Obrazek atrakcji (lub placeholder)
- Nazwa atrakcji
- Pełny opis
- Lokalizacja (miasto, kraj)
- Współrzędne geograficzne
- Badge "Własna" / "Zweryfikowana"
- Przycisk "Dodaj do wycieczki" (dla zalogowanych)
- Przyciski Edytuj/Usuń (tylko dla właściciela)

**Kluczowe komponenty:**
- `AttractionDetail` - główny kontener
- `AttractionImage` - duży obrazek z placeholder
- `AttractionInfo` - informacje tekstowe
- `LocationBadge` - badge lokalizacji
- `OwnershipBadge` - badge własna/zweryfikowana
- `AddToTripButton` - przycisk dodawania do wycieczki
- `AddToTripModal` - modal wyboru wycieczki
- `EditAttractionButton` - przycisk edycji (warunkowo)
- `DeleteAttractionButton` - przycisk usunięcia (warunkowo)

**Względy UX/A11y/Security:**
- Focus na głównej akcji (Dodaj do wycieczki)
- Semantic headings dla struktury
- Alt text dla obrazka
- Modal z listą wycieczek użytkownika
- Edycja/usuwanie tylko dla właściciela

**Mapowanie API:**
- `GET /api/attractions/:id` → szczegóły atrakcji
- `GET /api/trips?onlyMine=true` → lista wycieczek dla modalu
- `POST /api/trips/:tripId/attractions` → dodanie do wycieczki
- `DELETE /api/attractions/:id` → usunięcie (tylko właściciel)

---

### 2.9. Tworzenie atrakcji

| Atrybut | Wartość |
|---------|---------|
| **Ścieżka** | `/attractions/new` |
| **Główny cel** | Utworzenie własnej atrakcji |
| **Dostęp** | Wymagane logowanie |

**Kluczowe informacje do wyświetlenia:**
- Pole nazwy atrakcji
- Pole opisu (textarea)
- Wybór lokalizacji (dropdown)
- Pola współrzędnych (szerokość, długość geograficzna)
- Pole URL obrazka
- Podgląd obrazka (jeśli URL podany)

**Kluczowe komponenty:**
- `AttractionForm` - formularz tworzenia
- `LocationSelect` - dropdown lokalizacji
- `CoordinatesInput` - pola szerokości i długości
- `ImageUrlInput` - pole URL z podglądem
- `ImagePreview` - podgląd obrazka
- `FormActions` - przyciski Utwórz/Anuluj

**Względy UX/A11y/Security:**
- Walidacja formatu URL dla obrazka
- Walidacja zakresu współrzędnych (-90 do 90, -180 do 180)
- Podgląd obrazka przy prawidłowym URL
- Atrakcja oznaczana jako "własna" (isVerified=false)

**Mapowanie API:**
- `GET /api/locations` → lista lokalizacji dla dropdown
- `POST /api/attractions` → utworzenie atrakcji

---

### 2.10. Edycja atrakcji

| Atrybut | Wartość |
|---------|---------|
| **Ścieżka** | `/attractions/:id/edit` |
| **Główny cel** | Edycja własnej atrakcji |
| **Dostęp** | Wymagane logowanie (tylko właściciel) |

**Kluczowe informacje do wyświetlenia:**
- Formularz z wypełnionymi danymi atrakcji
- Te same pola co w tworzeniu
- Przyciski Zapisz/Anuluj

**Kluczowe komponenty:**
- `AttractionForm` - formularz edycji (reużycie z tworzenia)
- Pozostałe komponenty jak w tworzeniu

**Względy UX/A11y/Security:**
- Pre-fill formularza danymi atrakcji
- Tylko właściciel może edytować
- Redirect do `/attractions/:id` po zapisaniu

**Mapowanie API:**
- `GET /api/attractions/:id` → dane do edycji
- `PUT /api/attractions/:id` → aktualizacja

---

### 2.11. Ustawienia profilu

| Atrybut | Wartość |
|---------|---------|
| **Ścieżka** | `/settings` |
| **Główny cel** | Zarządzanie kontem użytkownika |
| **Dostęp** | Wymagane logowanie |

**Kluczowe informacje do wyświetlenia:**
- Sekcja danych konta (email - tylko odczyt)
- Sekcja zmiany hasła
- Sekcja usunięcia konta (danger zone)

**Kluczowe komponenty:**
- `AccountInfo` - informacje o koncie
- `ChangePasswordForm` - formularz zmiany hasła
- `DangerZone` - sekcja destrukcyjnych akcji
- `DeleteAccountButton` - przycisk usunięcia konta
- `DeleteAccountModal` - modal z potwierdzeniem hasłem

**Względy UX/A11y/Security:**
- Sekcje z wyraźnymi nagłówkami
- Danger zone wizualnie wyróżniona (czerwona ramka)
- Potwierdzenie hasłem przy usunięciu konta
- Wylogowanie i redirect do `/` po usunięciu

**Mapowanie API:**
- `GET /api/auth/me` → dane użytkownika
- `PUT /api/profiles/me` → aktualizacja profilu
- `DELETE /api/profiles/me` → usunięcie konta

---

### 2.12. Strona 404

| Atrybut | Wartość |
|---------|---------|
| **Ścieżka** | `*` (dowolna nieznana ścieżka) |
| **Główny cel** | Obsługa nieistniejących stron |
| **Dostęp** | Publiczny |

**Kluczowe informacje do wyświetlenia:**
- Przyjazny komunikat "Strona nie została znaleziona"
- Przycisk powrotu do strony głównej
- Link do przeglądania atrakcji

**Kluczowe komponenty:**
- `NotFoundPage` - strona 404
- `ErrorIllustration` - ilustracja/ikona
- `NavigationLinks` - linki nawigacyjne

**Względy UX/A11y/Security:**
- Focus na głównym CTA
- Przyjazny ton komunikatu (nie obwiniać użytkownika)

---

### 2.13. Strona błędu serwera

| Atrybut | Wartość |
|---------|---------|
| **Ścieżka** | N/A (wyświetlana przy błędach 500) |
| **Główny cel** | Obsługa błędów serwera |
| **Dostęp** | Publiczny |

**Kluczowe informacje do wyświetlenia:**
- Komunikat "Coś poszło nie tak"
- Przycisk "Spróbuj ponownie"
- Link do strony głównej

**Kluczowe komponenty:**
- `ErrorPage` - strona błędu
- `ErrorIllustration` - ilustracja/ikona
- `RetryButton` - przycisk ponowienia
- `HomeLink` - link do strony głównej

**Względy UX/A11y/Security:**
- Nie wyświetlać technicznych szczegółów użytkownikowi
- Focus na przycisku ponowienia

---

## 3. Mapa podróży użytkownika

### 3.1. Główny przepływ: Planowanie wycieczki (nowy użytkownik)

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                           NOWY UŻYTKOWNIK                                    │
└─────────────────────────────────────────────────────────────────────────────┘
                                    │
                                    ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│  1. LANDING PAGE (/)                                                         │
│     - Widzi opis aplikacji                                                   │
│     - Klika "Zarejestruj się"                                               │
└─────────────────────────────────────────────────────────────────────────────┘
                                    │
                                    ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│  2. REJESTRACJA (/register)                                                  │
│     - Wypełnia email, hasło, potwierdzenie                                  │
│     - Widzi wskaźnik siły hasła                                             │
│     - Klika "Zarejestruj się"                                               │
│     → Automatyczne logowanie                                                 │
└─────────────────────────────────────────────────────────────────────────────┘
                                    │
                                    ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│  3. DASHBOARD (/) - zalogowany                                               │
│     - Widzi puste statystyki                                                │
│     - Klika "Utwórz pierwszą wycieczkę"                                     │
└─────────────────────────────────────────────────────────────────────────────┘
                                    │
                                    ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│  4. TWORZENIE WYCIECZKI (/trips/new)                                         │
│     - Wpisuje nazwę wycieczki                                               │
│     - Wybiera datę                                                          │
│     - Klika "Utwórz"                                                        │
└─────────────────────────────────────────────────────────────────────────────┘
                                    │
                                    ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│  5. SZCZEGÓŁY WYCIECZKI (/trips/:id)                                         │
│     - Widzi pustą listę atrakcji                                            │
│     - Klika "Dodaj atrakcje"                                                │
└─────────────────────────────────────────────────────────────────────────────┘
                                    │
                                    ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│  6. DODAWANIE ATRAKCJI (/attractions?tripId=...)                             │
│     - Widzi nagłówek z nazwą wycieczki i licznikiem (X/20)                  │
│     - Filtruje po lokalizacji                                               │
│     - Przegląda listę atrakcji z przyciskami "Dodaj"                        │
│     - Klika "Dodaj do wycieczki" na wybranej atrakcji                       │
│     - Widzi potwierdzenie "Dodano do wycieczki"                             │
│     → Powtarza dla kolejnych atrakcji                                       │
│     - Klika "Wróć do wycieczki" gdy skończy                                 │
└─────────────────────────────────────────────────────────────────────────────┘
                                    │
                                    ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│  7. SZCZEGÓŁY WYCIECZKI (/trips/:id)                                         │
│     - Widzi listę dodanych atrakcji                                         │
│     - Zmienia kolejność (przyciski góra/dół)                                │
│     - Zmiany zapisują się automatycznie                                     │
│     → Wycieczka gotowa!                                                     │
└─────────────────────────────────────────────────────────────────────────────┘
```

### 3.2. Przepływ: Powracający użytkownik

```
┌─────────────────┐     ┌─────────────────┐     ┌─────────────────┐
│  /login         │────▶│  /             │────▶│  /trips         │
│  Logowanie      │     │  Dashboard      │     │  Lista wycieczek│
└─────────────────┘     └─────────────────┘     └─────────────────┘
                                                        │
                        ┌───────────────────────────────┘
                        │
                        ▼
              ┌─────────────────┐
              │  /trips/:id     │
              │  Edycja         │
              │  wycieczki      │
              └─────────────────┘
```

### 3.3. Przepływ: Tworzenie własnej atrakcji

```
┌─────────────────┐     ┌─────────────────┐     ┌─────────────────┐
│  /attractions   │────▶│  /attractions   │────▶│  /attractions   │
│  Lista atrakcji │     │  /new           │     │  /:id           │
│  → "Dodaj       │     │  Formularz      │     │  Nowa atrakcja  │
│     własną"     │     │  tworzenia      │     │  (szczegóły)    │
└─────────────────┘     └─────────────────┘     └─────────────────┘
```

### 3.4. Przepływ: Usunięcie wycieczki

```
┌─────────────────┐     ┌─────────────────┐     ┌─────────────────┐
│  /trips/:id     │────▶│  Modal          │────▶│  /trips         │
│  Szczegóły      │     │  potwierdzenia  │     │  Lista wycieczek│
│  → "Usuń"       │     │  → "Tak, usuń"  │     │  + Toast sukces │
└─────────────────┘     └─────────────────┘     └─────────────────┘
```

### 3.5. Przepływ: Usunięcie konta

```
┌─────────────────┐     ┌─────────────────┐     ┌─────────────────┐
│  /settings      │────▶│  Modal          │────▶│  /              │
│  Ustawienia     │     │  potwierdzenia  │     │  Landing page   │
│  → "Usuń konto" │     │  (hasło)        │     │  (wylogowany)   │
└─────────────────┘     └─────────────────┘     └─────────────────┘
```

---

## 4. Układ i struktura nawigacji

### 4.1. Layout główny

```
┌─────────────────────────────────────────────────────────────────┐
│                        HEADER (sticky)                          │
│  ┌──────┐  ┌──────────────────────────────┐  ┌────────────────┐ │
│  │ Logo │  │ Moje wycieczki │ Atrakcje   │  │ User dropdown  │ │
│  └──────┘  └──────────────────────────────┘  └────────────────┘ │
├─────────────────────────────────────────────────────────────────┤
│                        BREADCRUMBS                              │
│  Home > Moje wycieczki > Ateny 2026                            │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│                                                                 │
│                         MAIN CONTENT                            │
│                                                                 │
│                                                                 │
│                                                                 │
│                                                                 │
├─────────────────────────────────────────────────────────────────┤
│                          FOOTER                                 │
│                    © 2026 TripPlanner                           │
└─────────────────────────────────────────────────────────────────┘
```

### 4.2. Nawigacja główna

**Dla niezalogowanych użytkowników:**

| Element | Ścieżka | Pozycja |
|---------|---------|---------|
| Logo | `/` | Lewa strona |
| Przeglądaj atrakcje | `/attractions` | Środek |
| Zaloguj się | `/login` | Prawa strona |
| Zarejestruj się | `/register` | Prawa strona (button primary) |

**Dla zalogowanych użytkowników:**

| Element | Ścieżka | Pozycja |
|---------|---------|---------|
| Logo | `/` | Lewa strona |
| Moje wycieczki | `/trips` | Środek |
| Przeglądaj atrakcje | `/attractions` | Środek |
| User dropdown | - | Prawa strona |
| └─ Ustawienia | `/settings` | Dropdown |
| └─ Wyloguj | (akcja) | Dropdown |

### 4.3. Breadcrumbs

| Ścieżka | Breadcrumbs |
|---------|-------------|
| `/` | (brak) |
| `/login` | Home > Logowanie |
| `/register` | Home > Rejestracja |
| `/trips` | Home > Moje wycieczki |
| `/trips/new` | Home > Moje wycieczki > Nowa wycieczka |
| `/trips/:id` | Home > Moje wycieczki > [Nazwa wycieczki] |
| `/attractions` | Home > Atrakcje |
| `/attractions/:id` | Home > Atrakcje > [Nazwa atrakcji] |
| `/attractions/new` | Home > Atrakcje > Nowa atrakcja |
| `/settings` | Home > Ustawienia |

### 4.4. Nawigacja kontekstowa

- **W szczegółach wycieczki:** przycisk "← Powrót do listy" (link do `/trips`)
- **W szczegółach atrakcji:** przycisk "← Powrót do listy" (link do `/attractions`)
- **W formularzach:** przycisk "Anuluj" (powrót do poprzedniej strony)

---

## 5. Kluczowe komponenty

### 5.1. Komponenty layoutu

| Komponent | Opis | Użycie |
|-----------|------|--------|
| `Header` | Sticky navbar z nawigacją | Wszystkie strony |
| `Footer` | Stopka z copyright | Wszystkie strony |
| `Breadcrumbs` | Ścieżka nawigacji | Wszystkie strony oprócz landing |
| `PageContainer` | Wrapper dla zawartości strony | Wszystkie strony |
| `AuthLayout` | Layout dla stron logowania/rejestracji | `/login`, `/register` |

### 5.2. Komponenty formularzy

| Komponent | Opis | Użycie |
|-----------|------|--------|
| `Input` | Pole tekstowe z label, error, helper | Wszystkie formularze |
| `PasswordInput` | Input z toggle visibility | Logowanie, rejestracja, ustawienia |
| `PasswordStrengthIndicator` | Wskaźnik siły hasła | Rejestracja |
| `Select` | Dropdown z opcjami | Filtry, formularze |
| `DatePicker` | Wybór daty | Tworzenie/edycja wycieczki |
| `Textarea` | Wieloliniowe pole tekstowe | Opis atrakcji |
| `CharacterCounter` | Licznik znaków | Nazwa wycieczki |
| `FormActions` | Przyciski Submit/Cancel | Wszystkie formularze |

### 5.3. Komponenty wyświetlania

| Komponent | Opis | Użycie |
|-----------|------|--------|
| `Card` | Bazowy komponent karty | Wycieczki, atrakcje |
| `TripCard` | Karta wycieczki | Lista wycieczek |
| `AttractionCard` | Karta atrakcji | Lista atrakcji |
| `Badge` | Etykieta (lokalizacja, własna, etc.) | Atrakcje |
| `EmptyState` | Komunikat braku danych z CTA | Listy bez elementów |
| `Skeleton` | Loading placeholder | Ładowanie list |
| `Pagination` | Nawigacja stron | Lista atrakcji |

### 5.4. Komponenty interakcji

| Komponent | Opis | Użycie |
|-----------|------|--------|
| `Button` | Przycisk (primary, secondary, danger) | Wszędzie |
| `Modal` | Dialog z overlay | Potwierdzenia, wybór wycieczki |
| `ConfirmDialog` | Modal potwierdzenia akcji | Usuwanie |
| `Toast` | Powiadomienie | Feedback po akcjach |
| `DraggableList` | Lista z drag & drop | Kolejność atrakcji w wycieczce |
| `SaveStatusIndicator` | Status autosave | Edycja wycieczki |

### 5.5. Komponenty specjalizowane

| Komponent | Opis | Użycie |
|-----------|------|--------|
| `LoginForm` | Formularz logowania | `/login` |
| `RegisterForm` | Formularz rejestracji | `/register` |
| `TripForm` | Formularz wycieczki | `/trips/new` |
| `AttractionForm` | Formularz atrakcji | `/attractions/new`, `/attractions/:id/edit` |
| `AddToTripModal` | Modal wyboru wycieczki | `/attractions/:id` |
| `LocationFilter` | Filtr lokalizacji | `/attractions` |
| `TripAttractionList` | Lista atrakcji w wycieczce | `/trips/:id` |
| `AttractionItem` | Element listy atrakcji z akcjami | `/trips/:id` |

### 5.6. Komponenty błędów

| Komponent | Opis | Użycie |
|-----------|------|--------|
| `ErrorBoundary` | Wrapper przechwytujący błędy | Cała aplikacja |
| `NotFoundPage` | Strona 404 | Nieznane ścieżki |
| `ErrorPage` | Strona błędu serwera | Błędy 500 |
| `InlineError` | Komunikat błędu przy polu | Formularze |

---

## 6. Mapowanie User Stories na UI

| User Story | Widok(i) | Komponenty | Endpoint API |
|------------|----------|------------|--------------|
| US-001: Rejestracja | `/register` | RegisterForm, PasswordStrengthIndicator | `POST /api/auth/register` |
| US-002: Logowanie | `/login` | LoginForm, PasswordInput, RememberMeCheckbox | `POST /api/auth/login` |
| US-003: Lokalizacje | `/attractions` | LocationFilter | `GET /api/locations` |
| US-004: Lista atrakcji | `/attractions` | AttractionList, AttractionCard, Pagination | `GET /api/attractions` |
| US-005: Szczegóły atrakcji | `/attractions/:id` | AttractionDetail, AddToTripButton | `GET /api/attractions/:id` |
| US-006: Tworzenie wycieczki | `/trips/new` | TripForm, DatePicker | `POST /api/trips` |
| US-007: Lista wycieczek | `/trips` | TripList, TripCard | `GET /api/trips` |
| US-008: Edycja wycieczki | `/trips/:id` | TripHeader, TripAttractionList, DraggableList | `PUT /api/trips/:id`, `POST /api/trips/:id/attractions/reorder` |
| US-009: Dodawanie do wycieczki | `/attractions/:id` | AddToTripModal | `POST /api/trips/:id/attractions` |
| US-010: Usuwanie wycieczki | `/trips/:id` | ConfirmDialog | `DELETE /api/trips/:id` |
| US-011: Własna atrakcja | `/attractions/new` | AttractionForm | `POST /api/attractions` |
| US-012: Profil | `/settings` | ChangePasswordForm, DeleteAccountButton | `PUT /api/profiles/me`, `DELETE /api/profiles/me` |

---

## 7. Obsługa błędów i stanów

### 7.1. Komunikaty błędów API

| Kod HTTP | Komunikat PL | Akcja UI |
|----------|--------------|----------|
| 400 | "Nieprawidłowe dane. Sprawdź formularz i spróbuj ponownie." | Inline errors przy polach |
| 401 | "Sesja wygasła. Zaloguj się ponownie." | Redirect do `/login` |
| 403 | "Brak uprawnień do wykonania tej operacji." | Toast error |
| 404 | "Nie znaleziono żądanego zasobu." | Strona 404 lub toast |
| 409 | "Operacja niemożliwa - zasób jest używany w innym miejscu." | Modal informacyjny |
| 500 | "Wystąpił błąd serwera. Spróbuj ponownie później." | Strona błędu |

### 7.2. Stany ładowania

| Kontekst | Komponent | Zachowanie |
|----------|-----------|------------|
| Lista wycieczek | `SkeletonTripCard` | 3-4 skeleton cards |
| Lista atrakcji | `SkeletonAttractionCard` | Grid skeleton cards |
| Szczegóły | `Skeleton` | Skeleton dla nagłówka i treści |
| Akcje (save, delete) | `Spinner` | Spinner w przycisku, disabled state |
| Autosave | `SaveStatusIndicator` | "Zapisywanie..." / "Zapisano" |

### 7.3. Stany puste

| Kontekst | Komunikat | CTA |
|----------|-----------|-----|
| Brak wycieczek | "Nie masz jeszcze wycieczek" | "Utwórz pierwszą wycieczkę" |
| Brak atrakcji w wycieczce | "Dodaj atrakcje do swojej wycieczki" | "Przeglądaj atrakcje" |
| Brak wyników filtra | "Nie znaleziono atrakcji" | "Zmień filtr" |
| Brak atrakcji w systemie | "Brak atrakcji w tej lokalizacji" | "Dodaj własną atrakcję" |

---

## 8. Wymagania dostępności (WCAG AA)

### 8.1. Struktura semantyczna

- Używanie odpowiednich elementów HTML: `<nav>`, `<main>`, `<section>`, `<article>`, `<button>`
- Hierarchia nagłówków: jeden `<h1>` na stronę, logiczna kolejność `<h2>`, `<h3>`
- Landmarks dla regionów strony

### 8.2. Nawigacja klawiaturowa

- Wszystkie interaktywne elementy dostępne z klawiatury (Tab, Enter, Space)
- Focus visible dla wszystkich focusable elements
- Skip links dla nawigacji
- Focus trap w modalach

### 8.3. Formulary

- Labels powiązane z polami (`htmlFor`)
- `aria-describedby` dla komunikatów błędów
- `aria-invalid` dla pól z błędami
- `aria-live` dla dynamicznych aktualizacji (siła hasła, status zapisu)

### 8.4. Obrazy i media

- Alt text dla wszystkich obrazów atrakcji
- Placeholder z tekstem alternatywnym
- Dekoracyjne obrazy z `aria-hidden="true"`

### 8.5. Kolory i kontrast

- Kontrast tekstu minimum 4.5:1 (WCAG AA)
- Nie poleganie wyłącznie na kolorze (ikony + kolor dla statusów)
- Wskaźniki focus widoczne niezależnie od koloru

---

*Dokument wygenerowany: 2026-01-24*
