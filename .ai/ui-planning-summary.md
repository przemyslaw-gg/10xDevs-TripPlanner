# TripPlanner - UI Architecture Planning Summary

## Decisions

Decyzje podjęte przez użytkownika podczas sesji planowania architektury UI:

1. **Wycieczki tylko jednodniowe** - brak wsparcia dla wycieczek wielodniowych (dayNumber/timeline)
2. **Brak integracji z mapami** - żadne mapy nie będą wyświetlane
3. **Brak wsparcia offline** - aplikacja wymaga połączenia z internetem
4. **Tylko desktop/tablet** - brak optymalizacji dla urządzeń mobilnych w MVP
5. **Brak globalnego wyszukiwania** - nie implementować wyszukiwarki globalnej
6. **Brak onboardingu** - użytkownik nie przechodzi procesu wprowadzającego
7. **Nie wyświetlać estimatedDuration** - czas trwania atrakcji ukryty
8. **Nie wyświetlać ratingu ani reviewCount** - oceny i liczba recenzji ukryte
9. **Brak obsługi stref czasowych** - założenie jednej strefy czasowej
10. **Filtrowanie po stronie serwera** - nie client-side
11. **Brak sortowania list** - listy wyświetlane w domyślnej kolejności
12. **Brak notatek do atrakcji** - użytkownik nie może dodawać notatek do atrakcji w wycieczce
13. **Brak duplikowania wycieczek** - nie implementować przycisku "Duplikuj"
14. **Brak eksportu do PDF** - nie implementować funkcji eksportu
15. **Brak przycisku "Kopiuj link"** - udostępnianie bez dedykowanego przycisku kopiowania
16. **Brak limitu prób logowania** - nie ograniczać nieudanych prób
17. **Brak resetowania hasła** - nie implementować flow odzyskiwania hasła
18. **Brak weryfikacji email** - rejestracja bez potwierdzenia adresu email
19. **Brak regulaminu/polityki prywatności** - nie wymagane dla MVP
20. **Brak integracji analytics** - żadne narzędzia analityczne

---

## Matched Recommendations

Rekomendacje zaakceptowane i dopasowane do odpowiedzi użytkownika:

1. **React + TypeScript + shadcn/ui** - nowoczesny stack frontend z gotowymi komponentami
2. **TanStack Query (React Query)** - zarządzanie stanem serwera z cache 5 minut i optimistic updates
3. **Feature-based folder structure** - organizacja kodu według funkcjonalności (auth, trips, attractions, locations)
4. **Top horizontal navbar ze sticky header** - główna nawigacja przyklejona podczas scrollowania
5. **Breadcrumbs** - ścieżka nawigacji: Home > Sekcja > Detal
6. **Karty zamiast tabel** - listy atrakcji i wycieczek jako karty
7. **Klasyczna paginacja** - przyciski Poprzednia/Następna zamiast infinite scroll
8. **Skeleton loaders dla list, spinner dla akcji** - rozróżnienie stanów ładowania
9. **Toast notifications (prawy dolny róg)** - success/error/info z auto-dismiss
10. **Confirmation dialogs dla destrukcyjnych akcji** - modalne potwierdzenia usunięć
11. **Autosave z 2s debounce** - automatyczne zapisywanie ze wskaźnikiem statusu
12. **Drag & drop dla kolejności atrakcji** - lub przyciski góra/dół
13. **Max 20 atrakcji na wycieczkę** - realistyczny limit
14. **Max 100 znaków nazwy wycieczki** - z licznikiem od 80 znaków
15. **Walidacja on blur + on submit** - inline errors pod polami
16. **Hasło min 8 znaków, 1 wielka, 1 cyfra** - ze wskaźnikiem siły
17. **Remember me checkbox** - dłuższy czas sesji
18. **Silent refresh token** - automatyczne odświeżanie przed wygaśnięciem
19. **Redirect after login** - zachowanie docelowego URL
20. **Lucide Icons + Inter font** - spójna ikonografia i typografia
21. **Primary blue color scheme** - domyślna paleta shadcn/ui
22. **Subtelne animacje 150-200ms** - hover, modals, toasts
23. **WCAG AA accessibility** - semantic HTML, ARIA, focus visible, contrast
24. **Dedykowane empty states z CTA** - przyjazne komunikaty zachęcające do akcji
25. **Strony błędów 404/500** - z linkami powrotnymi

---

## UI Architecture Planning Summary

### Główne wymagania architektury UI

Aplikacja TripPlanner MVP to **desktopowa/tabletowa** aplikacja webowa do planowania jednodniowych wycieczek. Kluczowe ograniczenia to:
- Brak funkcji zaawansowanych (mapy, offline, mobile, analytics)
- Uproszczony model danych (brak ratingu, czasu trwania, notatek)
- Minimalistyczna autentykacja (bez reset hasła, weryfikacji email)

### Kluczowe widoki i ekrany

| Widok | Opis | Komponenty |
|-------|------|------------|
| **Dashboard/Home** | Strona główna po zalogowaniu | TripList, quick actions |
| **Browse Attractions** | Przeglądanie atrakcji z filtrowaniem | AttractionList, LocationFilter, Pagination |
| **Attraction Detail** | Szczegóły pojedynczej atrakcji | AttractionCard (expanded), "Add to trip" |
| **My Trips** | Lista wycieczek użytkownika | TripList, TripCard |
| **Trip Detail** | Edycja wycieczki | TripAttractionList (drag & drop), Share button |
| **Trip Form** | Tworzenie/edycja wycieczki | Name input, date picker |
| **Login/Register** | Autentykacja | LoginForm, RegisterForm |
| **Profile Settings** | Ustawienia konta | Delete account option |
| **Shared Trip View** | Widok publiczny (read-only) | TripDetail (bez edycji) |

### Przepływy użytkownika

```
┌─────────────┐     ┌─────────────┐     ┌─────────────┐
│  Register   │────▶│  Dashboard  │────▶│ Create Trip │
└─────────────┘     └─────────────┘     └─────────────┘
                           │                    │
                           ▼                    ▼
                    ┌─────────────┐     ┌─────────────┐
                    │   Browse    │────▶│ Trip Detail │
                    │ Attractions │     │ (add/order) │
                    └─────────────┘     └─────────────┘
                                               │
                                               ▼
                                        ┌─────────────┐
                                        │    Share    │
                                        │   (link)    │
                                        └─────────────┘
```

### Strategia integracji z API

**TanStack Query** jako główne narzędzie:

```typescript
// Konfiguracja cache
{
  staleTime: 5 * 60 * 1000,  // 5 minut dla list atrakcji
  refetchOnWindowFocus: true, // dla danych użytkownika
}

// Optimistic updates dla CRUD
useMutation({
  onMutate: async (newData) => {
    await queryClient.cancelQueries(['trips']);
    const previous = queryClient.getQueryData(['trips']);
    queryClient.setQueryData(['trips'], (old) => [...old, newData]);
    return { previous };
  },
  onError: (err, newData, context) => {
    queryClient.setQueryData(['trips'], context.previous);
  },
});
```

**Endpointy API do integracji:**
- `GET/POST /api/locations` - lokalizacje
- `GET/POST/PUT/DELETE /api/attractions` - atrakcje (z filtrowaniem server-side)
- `GET/POST/PUT/DELETE /api/trips` - wycieczki
- `POST/DELETE /api/trips/{id}/attractions` - atrakcje w wycieczce
- `POST /api/auth/login`, `POST /api/auth/register` - autentykacja

### Zarządzanie stanem

| Typ stanu | Rozwiązanie |
|-----------|-------------|
| Server state | TanStack Query (cache, refetch, mutations) |
| Auth state | Context + localStorage (token) |
| UI state | Local component state (useState) |
| Form state | React Hook Form lub natywne formularze |

### Responsywność

- **Breakpoints:** Desktop (1200px+), Tablet (768px-1199px)
- **Layout:** Fluid grid, karty responsywne
- **Nawigacja:** Sticky top navbar (bez hamburger menu - tylko desktop/tablet)

### Dostępność (a11y)

- Semantic HTML (`<nav>`, `<main>`, `<button>`, `<section>`)
- ARIA labels dla ikon bez tekstu
- Focus visible dla nawigacji klawiaturą
- Kontrast kolorów WCAG AA
- Alt text dla obrazków atrakcji

### Bezpieczeństwo

- JWT w localStorage z silent refresh
- Hasło min 8 znaków, wielka litera, cyfra
- Confirmation dialogs przed destrukcyjnymi akcjami
- RequireAuthorization() na endpointach chronionych

---

## Resolved Issues

Odpowiedzi na wcześniej nierozwiązane kwestie:

### 1. Język interfejsu
**Polski** jako domyślny i jedyny język w MVP.

### 2. Struktura routingu

| Ścieżka | Widok | Dostęp |
|---------|-------|--------|
| `/` | Home/Dashboard | Publiczny (redirect do /login jeśli niezalogowany) |
| `/login` | Logowanie | Publiczny |
| `/register` | Rejestracja | Publiczny |
| `/trips` | Lista wycieczek użytkownika | Wymagane logowanie |
| `/trips/new` | Tworzenie nowej wycieczki | Wymagane logowanie |
| `/trips/:id` | Szczegóły/edycja wycieczki | Wymagane logowanie (tylko właściciel) |
| `/attractions` | Przeglądanie atrakcji | Publiczny |
| `/attractions/:id` | Szczegóły atrakcji | Publiczny |
| `/settings` | Ustawienia profilu | Wymagane logowanie |

### 3. Udostępnianie wycieczek
**Pominięte w MVP** - wycieczki są prywatne, widoczne tylko dla zalogowanego właściciela.

### 4. Obsługa błędów API

| Kod | Komunikat PL |
|-----|--------------|
| 400 Bad Request | "Nieprawidłowe dane. Sprawdź formularz i spróbuj ponownie." |
| 401 Unauthorized | "Sesja wygasła. Zaloguj się ponownie." |
| 403 Forbidden | "Brak uprawnień do wykonania tej operacji." |
| 404 Not Found | "Nie znaleziono żądanego zasobu." |
| 409 Conflict | "Operacja niemożliwa - zasób jest używany w innym miejscu." |
| 500 Server Error | "Wystąpił błąd serwera. Spróbuj ponownie później." |

### 5. Limity danych
- **Wycieczki na użytkownika:** brak limitu
- **Atrakcje na stronę (pageSize):** 10

### 6. Strategia deploymentu frontend
**Osobna aplikacja SPA** (Vite + React) - niezależna od backendu .NET.

### 7. Testowanie UI
**Brak testów UI w MVP** - testy zostaną dodane w późniejszej fazie.

### 8. Walidacja formularzy

| Pole | Reguły |
|------|--------|
| Nazwa wycieczki | Min. 1 znak, max. 100 znaków |
| Data wycieczki | Dowolna data (przeszła i przyszła dozwolona) |
| Email | Standardowa walidacja formatu email |
| Hasło | Min. 8 znaków, 1 wielka litera, 1 cyfra |
| URL obrazka | Opcjonalny, walidacja formatu URL |

### 9. Obsługa obrazków
**External URL** - użytkownik podaje link do obrazka. Brak uploadu plików w MVP.

### 10. SEO
**Brak SEO w MVP** - czyste SPA bez server-side rendering i meta tagów.

---

## Updated Excluded Features (for MVP)

- Multi-day trips
- Map integrations
- Offline support
- Mobile optimization
- Global search
- Onboarding flow
- Estimated duration display
- Rating/review count display
- Sorting
- Notes per attraction
- Duplicate trip
- Export to PDF
- **Trip sharing (public links)**
- Copy link button
- Password reset
- Email confirmation
- Login attempt limits
- Terms of service
- Analytics
- Dark mode
- i18n (multiple languages)
- Undo/Redo
- UI tests
- SEO / SSR
- Image upload

---

*Document generated: 2026-01-24*
*Last updated: 2026-01-24*
