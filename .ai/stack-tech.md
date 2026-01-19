# Stack Technologiczny - TripPlanner MVP

## Przegląd

Stack technologiczny dla projektu TripPlanner - aplikacji webowej do planowania wycieczek turystycznych.

---

## Backend

### Runtime & Framework

**ASP.NET Core 10 - Minimal API**

- **Wersja:** .NET 10
- **Typ:** Minimal API
- **Zastosowanie:** Backend REST API dla aplikacji webowej
- **Dlaczego:** Nowoczesne podejście do tworzenia API z minimalną ceremonią kodu

### Architektura

**Clean Architecture + CQRS**

- **Pattern:** Clean Architecture w połączeniu z CQRS (Command Query Responsibility Segregation)
- **Biblioteka:** MediatR
- **Struktura:**
  - `TripPlanner.Web` - Warstwa prezentacji (Controllers/API)
  - `TripPlanner.Application` - Warstwa przypadków użycia (Commands, Queries, Handlers)
  - `TripPlanner.Domain` - Warstwa logiki biznesowej (Entities, Value Objects, Domain Services)
  - `TripPlanner.Infrastructure` - Warstwa infrastruktury (Persistence, External Services, Identity)

### Walidacja

**FluentValidation**

- **Zastosowanie:** Walidacja requestów i modeli biznesowych
- **Dlaczego:** Bardziej czytelna i łatwiejsza w utrzymaniu walidacja niż Data Annotations

### Baza Danych

**PostgreSQL + Entity Framework Core 10**

- **Baza danych:** PostgreSQL
- **ORM:** Entity Framework Core 10
- **Migracje:** EF Core Migrations
- **Zastosowanie:**
  - Przechowywanie danych użytkowników
  - Przechowywanie atrakcji turystycznych
  - Przechowywanie planów wycieczek

### Autentykacja & Autoryzacja

**ASP.NET Identity**

- **System:** ASP.NET Core Identity
- **Zastosowanie:**
  - Rejestracja użytkowników
  - Logowanie/wylogowanie
  - Zarządzanie hasłami
  - Token-based authentication

---

## Frontend

### Framework

**React 19**

- **Wersja:** React 19
- **Język:** TypeScript
- **Zastosowanie:** Budowa interaktywnego interfejsu użytkownika SPA (Single Page Application)

### Routing

**React Router**

- **Zastosowanie:** Nawigacja między widokami aplikacji
- **Funkcje:**
  - Routing kliencki
  - Nested routes
  - Protected routes (dla zalogowanych użytkowników)

### Stylowanie

**Tailwind CSS**

- **Framework:** Tailwind CSS
- **Podejście:** Utility-first CSS framework
- **Zastosowanie:**
  - Responsywny design
  - Szybkie prototypowanie UI
  - Spójny design system

### Build Tool

**Vite** (domyślny dla React 19)

- **Zastosowanie:** Bundling i development server
- **Zalety:** Szybkie HMR (Hot Module Replacement), nowoczesny build

---

## DevOps & CI/CD

### Continuous Integration

**GitHub Actions**

- **Platforma:** GitHub Actions
- **Triggery:**
  - Push do gałęzi `main`/`master`
  - Pull Requests
- **Pipeline:**
  1. Restore dependencies
  2. Build (Release mode)
  3. Testy jednostkowe
  4. Testy integracyjne Web API
  5. Publikacja raportów testowych

### Konteneryzacja

**Docker**

- **Zastosowanie:** Konteneryzacja aplikacji
- **Build:** Automatyczny build obrazu Docker po przejściu testów
- **Tagging:**
  - `latest` - najnowsza wersja
  - `{commit-sha}` - wersja powiązana z commitem

### Testowanie

**MSTest / xUnit**

- **Kategorie testów:**
  - `[TestCategory("Unit")]` - Testy jednostkowe (logika biznesowa, serwisy domenowe)
  - `[TestCategory("Integration")]` - Testy integracyjne (Web API endpoints)
- **Format raportów:** TRX (Test Results XML)
- **Narzędzia:** WebApplicationFactory dla testów integracyjnych API

---

## Moduły Aplikacji

### 1. Users Module

- **Odpowiedzialność:** Autentykacja i autoryzacja użytkowników
- **Technologie:**
  - ASP.NET Identity
  - JWT Tokens (opcjonalnie)

### 2. Attractions Module

- **Odpowiedzialność:** Wyszukiwanie i zarządzanie atrakcjami turystycznymi
- **Technologie:**
  - EF Core (persistence)
  - PostgreSQL (storage)

### 3. TripPlanning Module

- **Odpowiedzialność:** Tworzenie i zarządzanie planami wycieczek
- **Technologie:**
  - CQRS (Commands/Queries)
  - MediatR
  - FluentValidation

### 4. RouteOptimization Module

- **Odpowiedzialność:** Algorytmy optymalizacji trasy zwiedzania
- **Algorytm:** Nearest Neighbor (najbliższy sąsiad) lub Haversine distance
- **Technologie:**
  - Domain Services
  - Value Objects (Location, Distance)

### 5. TimeManagement Module

- **Odpowiedzialność:** Planowanie czasowe i podział na dni
- **Technologie:**
  - Domain Services
  - Value Objects (TimeSlot, Duration)

---

## Zewnętrzne Zależności

### NuGet Packages (Backend)

```
MediatR - CQRS implementation
MediatR.Extensions.Microsoft.DependencyInjection
FluentValidation
FluentValidation.AspNetCore
Microsoft.EntityFrameworkCore
Microsoft.EntityFrameworkCore.Design
Npgsql.EntityFrameworkCore.PostgreSQL
Microsoft.AspNetCore.Identity.EntityFrameworkCore
```

### NPM Packages (Frontend)

```
react@19
react-dom@19
react-router-dom
typescript
tailwindcss
postcss
autoprefixer
axios (lub fetch API dla komunikacji z backendem)
```

---

## Środowiska

### Development

- **Backend:** ASP.NET Core Development Server (Kestrel)
- **Frontend:** Vite Dev Server
- **Database:** PostgreSQL (lokalny lub Docker)
- **Secrets:** User Secrets (.NET)

### Production (docelowo)

- **Hosting:** TBD (Azure, AWS, lub self-hosted)
- **Database:** PostgreSQL (managed service)
- **Secrets:** Azure Key Vault / AWS Secrets Manager
- **Deployment:** Docker containers

---

## Wersje i Kompatybilność

| Technologia | Wersja | Status |
|-------------|--------|--------|
| .NET | 10 | Najnowsza (Listopad 2025) |
| Entity Framework Core | 10 | Zgodna z .NET 10 |
| React | 19 | Najnowsza |
| TypeScript | 5.x | Stabilna |
| PostgreSQL | 15+ | Stabilna |
| Node.js | 20 LTS | Wymagana dla buildu frontendu |

---

## Decyzje Architektoniczne

### Dlaczego Clean Architecture + CQRS?

- **Separacja odpowiedzialności:** Jasny podział na warstwy
- **Testowalność:** Łatwe mockowanie zależności
- **Nauka:** Projekt edukacyjny - cel to nauka wzorców architektonicznych
- **Skalowalność:** Przygotowanie na przyszły rozwój

### Dlaczego Minimal API?

- **Prostota:** Mniej boilerplate code
- **Nowoczesność:** Najnowsze podejście w .NET
- **Performance:** Szybsze niż tradycyjne Controllers

### Dlaczego PostgreSQL zamiast SQL Server?

- **Open Source:** Darmowa licencja
- **Cross-platform:** Działa na Windows, Linux, macOS
- **Popularność:** Jedna z najpopularniejszych relacyjnych baz danych

### Dlaczego React 19 zamiast Blazor?

- **Ekosystem:** Większy wybór bibliotek i komponentów
- **Popularność:** Bardziej powszechny w przemyśle
- **Nauka:** Umiejętność ceniona na rynku pracy
- **SPA Experience:** Pełna kontrola nad UI/UX

---

## Ograniczenia MVP

### Co NIE jest w stacku MVP:

- Cache layer (Redis) - można dodać później
- Message Queue (RabbitMQ, Kafka) - nie potrzebne w MVP
- Monitoring (Application Insights, Prometheus) - podstawowa obsługa błędów na start
- CDN - nie potrzebne dla MVP
- Load Balancer - pojedyncza instancja wystarczy
- Elasticsearch - PostgreSQL full-text search wystarczy
- GraphQL - REST API wystarczy dla MVP

### Planowane rozszerzenia (Post-MVP):

- Redis cache dla queries
- SignalR dla real-time collaboration
- Azure Blob Storage / AWS S3 dla plików
- External API integrations (Google Maps, Weather API)
- Progressive Web App (PWA) capabilities

---

## Podsumowanie Stack'u

**Backend:** ASP.NET Core 10 (Minimal API) + Clean Architecture + CQRS + PostgreSQL + EF Core + ASP.NET Identity

**Frontend:** React 19 + TypeScript + React Router + Tailwind CSS

**DevOps:** GitHub Actions + Docker

**Testing:** MSTest/xUnit z kategoriami Unit i Integration

**Architektura:** 4-warstwowa Clean Architecture z separacją CQRS
