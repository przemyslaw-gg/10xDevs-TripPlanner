# Dokument Planistyczny Projektu: TripPlanner

---

## 1. OPIS PROBLEMU

**Kontekst:**
Planowanie wycieczek do popularnych destynacji turystycznych jest czasochłonnym i chaotycznym procesem. Turyści muszą ręcznie przeszukiwać wiele portali internetowych, aby znaleźć atrakcje, ocenić ich wartość i zaplanować optymalną trasę zwiedzania.

**Problem biznesowy:**
Brak zintegrowanego narzędzia, które automatyzuje proces planowania wycieczek poprzez:
- Agregację informacji o atrakcjach z różnych źródeł
- Optymalizację trasy zwiedzania pod kątem minimalizacji dystansu
- Automatyczne planowanie czasowe z uwzględnieniem czasu zwiedzania
- Możliwość wielokrotnego wykorzystania i edycji planów

---

## 2. PROBLEM UŻYTKOWNIKA

**Persona:** Tomasz - lubi podróżować i zwiedzać miasta

**Scenariusz użycia:**
> "Planuję 3-dniową wycieczkę do Aten. Chcę zobaczyć najważniejsze zabytki, ale nie wiem:
> - Które atrakcje są naprawdę warte odwiedzenia?
> - W jakiej kolejności je zwiedzać, żeby nie biegać po mieście w kółko?
> - Ile czasu zajmie mi zwiedzanie każdej atrakcji?
> - Ile atrakcji realnie mogę zobaczyć w ciągu jednego dnia?
> - Czy 3 dni wystarczą na zwiedzenie tego, co chcę zobaczyć?
>
> Obecnie spędzam godziny na przeglądaniu Google Maps, TripAdvisor i blogów podróżniczych, robiąc notatki w Excelu. Później próbuję ręcznie ułożyć trasę na mapie, co jest frustrujące."

**Potrzeby użytkownika:**
1. **Odkrywanie** - znalezienie najlepiej ocenianych atrakcji w danej lokalizacji
2. **Wybór** - możliwość samodzielnego wybrania interesujących miejsc
3. **Optymalizacja** - automatyczne ułożenie trasy w logicznej kolejności
4. **Planowanie czasowe** - oszacowanie czasu i podział na dni
5. **Persistencja** - zapisanie planu i możliwość powrotu do niego
6. **Dostępność** - dostęp z dowolnego urządzenia przez przeglądarkę

---

## 3. WYMAGANIA FUNKCJONALNE (MVP)

### 3.1. Autentykacja i zarządzanie użytkownikami
- **FR-01:** System umożliwia rejestrację nowego użytkownika (email + hasło)
- **FR-02:** System umożliwia logowanie i wylogowanie użytkownika
- **FR-03:** Każdy użytkownik ma dostęp publicznych planów wycieczek oraz swoich prywatnych

### 3.2. Wyszukiwanie i wybór atrakcji
- **FR-04:** Użytkownik może wyszukać miasto/lokalizację (np. "Ateny, Grecja")
- **FR-05:** System pobiera listę atrakcji zdefiniowaną w bazie danych dla danej lokalizacji
- **FR-06:** Dla każdej atrakcji wyświetlane są: nazwa, ocena, liczba opinii, szacowany czas zwiedzania
- **FR-07:** Użytkownik może przeglądać listę atrakcji posortowaną według oceny
- **FR-08:** Użytkownik może wybrać atrakcje, które chce odwiedzić (checkbox/selection)
- **FR-09:** Użytkownik może ręcznie dodać własną atrakcję do bazy

### 3.3. Optymalizacja trasy zwiedzania
- **FR-10:** System oblicza optymalną trasę zwiedzania dla wybranych atrakcji używając algorytmu najbliższego sąsiada
- **FR-11:** Użytkownik może określić punkt startowy trasy - pierwszą atrakcję od której chce zacząć zwiedzanie
- **FR-12:** System wyświetla zaproponowaną kolejność zwiedzania na liście
- **FR-13:** Użytkownik może ręcznie zmienić kolejność atrakcji w trasie (drag & drop)

### 3.4. Planowanie czasowe wycieczki
- **FR-15:** System oszacowuje całkowity czas potrzebny na zwiedzanie wszystkich wybranych atrakcji
- **FR-16:** Parametr systemowy określa ilość godzin zwiedzania dziennie (np. 8 godzin/dzień)
- **FR-17:** System automatycznie dzieli atrakcje na dni na podstawie czasu zwiedzania
- **FR-18:** System wyświetla szczegółowy harmonogram dzienny z godziną rozpoczęcia każdej atrakcji
- **FR-16:** Parametr systemowy określa o ile godzin można wydłużyć dzień podróży, jeśli braknie czasu

### 3.5. Zapisywanie i edycja planów
- **FR-20:** Użytkownik może zapisać stworzony plan wycieczki z nazwą (np. "Ateny 2026")
- **FR-21:** System przechowuje wszystkie plany użytkownika
- **FR-22:** Użytkownik może przeglądać listę swoich zapisanych planów
- **FR-23:** Użytkownik może edytować istniejący plan (dodawać/usuwać atrakcje, zmieniać kolejność)
- **FR-24:** Użytkownik może usunąć plan wycieczki
- **FR-24:** Użytkownik może opublikować plan wycieczki aby był dostępny dla innych osób

---

## 4. GRANICE PROJEKTU

### ✅ CO WCHODZI W ZAKRES MVP:

**Funkcjonalności:**
- Rejestracja i logowanie użytkowników (ASP.NET Identity)
- Wyszukiwanie atrakcji w danym mieście
- Lokalne przechowywanie danych o atrakcjach w bazie
- Wybór atrakcji do planu
- Prosty algorytm optymalizacji trasy (najbliższy sąsiad lub haverside)
- Planowanie czasowe z podziałem na dni
- CRUD operacje na planach wycieczek
- Podstawowy interfejs webowy (responsywny)

**Technologie:**
- .Net 10 Web API (minimal api)
- Clean Architecture + CQRS (MediatR)
- FluentValidation
- Entity Framework Core 10
- ASP.NET Identity
- Frontend (React19 + TypeScript), React Router, Tailwind CSS
- PostgreSQL

**Dane:**
- Podstawowe informacje o atrakcjach (nazwa, lokalizacja, ocena, czas)
- Plany użytkowników
- Dane użytkowników

---

### ❌ CO NIE WCHODZI W ZAKRES MVP:

**Funkcjonalności wykluczone z MVP:**
- Udostępnianie planów innym użytkownikom (social features)
- Integracja z kalendarzem Google
- Integracja z mapami (google, leaflet)
- Sprawdzanie pogody
- Rekomendacje AI/ML oparte na preferencjach
- Rezerwacje hoteli/biletów
- Budżetowanie wycieczki
- Integracja z systemami płatności
- Współdzielenie planów w czasie rzeczywistym (collaborative editing)
- Aplikacja mobilna (iOS/Android)
- Powiadomienia push
- Obsługa wielu języków (i18n) - tylko angielski
- Godziny otwarcia atrakcji w algorytmie
- Zaawansowane algorytmy optymalizacji (symulowane wyżarzanie, genetyczny)
- Import/export tras z zewnętrznych źródeł

**Funkcjonalności planowane na przyszłość (Post-MVP):**
- Integracja z mapami
- Export planu do PDF
- Obsługa wielu języków
- Godziny otwarcia w planowaniu

**Ograniczenia techniczne w MVP:**
- Brak cache'owania zapytań do API (można dodać później)
- Podstawowa obsługa błędów
- Brak zaawansowanych metryk i monitoringu
- Brak testów obciążeniowych
- Deployment ręczny (automatyczne testy w CI/CD, bez automatycznego wdrożenia)

---

## 5. ARCHITEKTURA TECHNICZNA (wysokopoziomowo)

**Clean Architecture + CQRS:**

```
TripPlanner.Web (Presentation)
├── Controllers/API
└── Frontend (Blazor/React)

TripPlanner.Application (Use Cases)
├── Commands (CreateTrip, UpdateTrip, OptimizeRoute)
├── Queries (GetTrips, GetAttractions, GetOptimalRoute)
└── Interfaces

TripPlanner.Domain (Business Logic)
├── Entities (Trip, Attraction, Route, User)
├── Value Objects (Location, TimeSlot, Rating)
└── Domain Services (RouteOptimizer)

TripPlanner.Infrastructure (External Concerns)
├── Persistence (EF Core, Repositories)
├── ExternalServices (GooglePlacesService)
└── Identity (Authentication)
```

**Główne moduły:**
1. **Users** - autentykacja, autoryzacja
2. **Attractions** - wyszukiwanie, przechowywanie atrakcji
3. **TripPlanning** - tworzenie planów, wybór atrakcji
4. **RouteOptimization** - algorytmy optymalizacji trasy
5. **TimeManagement** - planowanie czasowe, podział na dni

---

## 6. PRIORYTETY IMPLEMENTACJI

**Faza 1 - Fundament (2-3 tygodnie):**
1. Struktura projektu (Clean Architecture)
2. Baza danych + Entity Framework
3. Autentykacja (Identity)
4. CRUD dla planów wycieczek

**Faza 2 - Atrakcje (2 tygodnie):**
5. Integracja z Google Places API
6. Wyszukiwanie i przechowywanie atrakcji
7. Wybór atrakcji do planu

**Faza 3 - Optymalizacja (1-2 tygodnie):**
8. Algorytm optymalizacji trasy
9. Wyświetlanie na mapie
10. Ręczna edycja kolejności

**Faza 4 - Planowanie czasowe (1 tydzień):**
11. Oszacowanie czasu
12. Podział na dni
13. Harmonogram dzienny

**Faza 5 - Frontend & Polish (1-2 tygodnie):**
14. Interfejs użytkownika
15. Responsywność
16. Obsługa błędów
17. Testy

---

## 7. CI/CD - CONTINUOUS INTEGRATION & DEPLOYMENT

### Rozwiązanie: GitHub Actions + Docker

**Wybór technologii:**
- **GitHub Actions** - darmowe dla publicznych repozytoriów, 2000 minut/miesiąc dla prywatnych
- **Docker** - konteneryzacja aplikacji
- Natywna integracja z GitHub
- Pełne wsparcie dla .NET i Docker

### 7.1. Pipeline CI/CD

**Trigger:**
- Każdy push do gałęzi `main`/`master`
- Każdy Pull Request do `main`/`master`

**Kroki w pipeline:**

1. **Restore & Build**
   - Przywrócenie zależności (`dotnet restore`)
   - Build projektu w trybie Release (`dotnet build`)

2. **Testy jednostkowe**
   - Uruchomienie testów jednostkowych
   - Generowanie raportu w formacie TRX
   - Warunek: muszą przejść przed kolejnymi krokami

3. **Testy integracyjne Web API**
   - Uruchomienie testów integracyjnych
   - Testowanie endpointów API
   - Generowanie raportu w formacie TRX

4. **Publikacja wyników testów**
   - Automatyczne wyświetlanie wyników w zakładce Actions
   - Oznaczanie PR jako passed/failed

5. **Build Docker Image** (tylko dla main/master)
   - Budowanie obrazu Docker po przejściu wszystkich testów
   - Tagowanie obrazem: `latest` i `{commit-sha}`
   - Zapisanie obrazu jako artifact

### 7.4. Co się dzieje w pipeline

**Przy pushu do main/master:**
1. ✅ Restore zależności
2. ✅ Build projektu (Release)
3. ✅ Uruchomienie testów jednostkowych
4. ✅ Uruchomienie testów integracyjnych Web API
5. ✅ Raport z testów dostępny w Actions
6. ✅ Build obrazu Docker (tylko jeśli wszystkie testy przeszły)
7. ✅ Zapisanie obrazu jako artifact

**Przy Pull Requestach:**
1. ✅ Restore, Build, Testy
2. ❌ Brak budowania obrazu Docker
3. ✅ Automatyczne oznaczenie PR jako ✅ passed / ❌ failed

**W przypadku błędów:**
- Pipeline zatrzymuje się na pierwszym niepowodzeniu
- Obraz Docker NIE jest budowany jeśli testy nie przejdą
- Developer otrzymuje powiadomienie o niepowodzeniu

---

## Podsumowanie

Projekt **TripPlanner** - aplikacja do planowania wycieczek, która:

✅ Rozwiązuje realny problem użytkownika
✅ Ma jasno określony zakres MVP
✅ Wykorzystuje Clean Architecture + CQRS do nauki
✅ Jest możliwa do zrealizowania w rozsądnym czasie
✅ Ma potencjał na rozwój w przyszłości
