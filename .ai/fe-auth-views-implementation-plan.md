# Plan implementacji widoków Logowania i Rejestracji

## 1. Przegląd

Widoki logowania (`/login`) i rejestracji (`/register`) stanowią podstawowy moduł autentykacji aplikacji TripPlanner. Umożliwiają użytkownikom tworzenie nowych kont oraz logowanie się do istniejących. Oba widoki są publiczne, ale przekierowują zalogowanych użytkowników na stronę główną. Implementacja kładzie nacisk na bezpieczeństwo (generyczne komunikaty błędów, maskowanie hasła) oraz dostępność (ARIA, prawidłowe etykiety).

## 2. Routing widoku

| Widok | Ścieżka | Dostęp | Przekierowanie |
|-------|---------|--------|----------------|
| Logowanie | `/login` | Publiczny | → `/` jeśli zalogowany |
| Rejestracja | `/register` | Publiczny | → `/` jeśli zalogowany |

**Konfiguracja w React Router:**
```tsx
<Route path="/login" element={<PublicRoute><LoginPage /></PublicRoute>} />
<Route path="/register" element={<PublicRoute><RegisterPage /></PublicRoute>} />
```

## 3. Struktura komponentów

```
src/
├── pages/
│   ├── LoginPage.tsx
│   └── RegisterPage.tsx
├── components/
│   └── auth/
│       ├── LoginForm.tsx
│       ├── RegisterForm.tsx
│       ├── PasswordInput.tsx
│       ├── PasswordStrengthIndicator.tsx
│       ├── PasswordRequirements.tsx
│       └── RememberMeCheckbox.tsx
├── hooks/
│   ├── useAuth.ts (rozszerzenie istniejącego)
│   ├── useLoginMutation.ts
│   ├── useRegisterMutation.ts
│   └── usePasswordStrength.ts
├── api/
│   └── auth.ts
└── components/
    └── common/
        └── PublicRoute.tsx
```

**Hierarchia komponentów:**

```
LoginPage
└── LoginForm
    ├── Input (email)
    ├── PasswordInput
    ├── RememberMeCheckbox
    └── Button (submit)

RegisterPage
└── RegisterForm
    ├── Input (email)
    ├── Input (displayName)
    ├── PasswordInput
    ├── PasswordStrengthIndicator
    ├── PasswordRequirements
    ├── PasswordInput (confirmPassword)
    └── Button (submit)
```

## 4. Szczegóły komponentów

### 4.1 LoginPage

- **Opis:** Strona kontenerowa dla formularza logowania. Zarządza layoutem i przekierowaniami.
- **Główne elementy:**
  - Nagłówek z logo/nazwą aplikacji
  - `LoginForm` - główny formularz
  - Link do strony rejestracji
- **Obsługiwane interakcje:**
  - Przekierowanie do `/` po udanym logowaniu
  - Nawigacja do `/register`
- **Obsługiwana walidacja:** Brak (delegowana do LoginForm)
- **Typy:** Brak dedykowanych
- **Propsy:** Brak

### 4.2 LoginForm

- **Opis:** Formularz logowania z walidacją i obsługą błędów API. Zawiera pola email, hasło oraz opcję "Zapamiętaj mnie".
- **Główne elementy:**
  - `<form>` z `onSubmit`
  - `<label>` + `<input type="email">` dla email
  - `PasswordInput` dla hasła
  - `RememberMeCheckbox`
  - `<button type="submit">` - przycisk logowania
  - Komunikat błędu (generyczny)
- **Obsługiwane interakcje:**
  - `onSubmit` - wysłanie formularza
  - `onChange` - aktualizacja pól
  - `onBlur` - walidacja pola
- **Obsługiwana walidacja:**
  - Email: wymagany, format email (regex: `/^[^\s@]+@[^\s@]+\.[^\s@]+$/`)
  - Hasło: wymagane, min 1 znak
- **Typy:**
  - `LoginFormData` - dane formularza
  - `LoginCommand` - request do API
  - `LoginResponseDTO` - response z API
- **Propsy:**
  - `onSuccess?: () => void` - callback po udanym logowaniu

### 4.3 RegisterPage

- **Opis:** Strona kontenerowa dla formularza rejestracji.
- **Główne elementy:**
  - Nagłówek z logo/nazwą aplikacji
  - `RegisterForm` - główny formularz
  - Link do strony logowania
- **Obsługiwane interakcje:**
  - Przekierowanie do `/` po udanej rejestracji (auto-login)
  - Nawigacja do `/login`
- **Obsługiwana walidacja:** Brak (delegowana do RegisterForm)
- **Typy:** Brak dedykowanych
- **Propsy:** Brak

### 4.4 RegisterForm

- **Opis:** Formularz rejestracji z walidacją w czasie rzeczywistym, wskaźnikiem siły hasła i listą wymagań.
- **Główne elementy:**
  - `<form>` z `onSubmit`
  - `<label>` + `<input type="email">` dla email
  - `<label>` + `<input type="text">` dla displayName
  - `PasswordInput` dla hasła
  - `PasswordStrengthIndicator`
  - `PasswordRequirements`
  - `PasswordInput` dla potwierdzenia hasła
  - `<button type="submit">` - przycisk rejestracji
  - Komunikaty błędów
- **Obsługiwane interakcje:**
  - `onSubmit` - wysłanie formularza
  - `onChange` - aktualizacja pól + real-time walidacja siły hasła
  - `onBlur` - walidacja pola
- **Obsługiwana walidacja:**
  - Email: wymagany, format email
  - DisplayName: wymagany, max 100 znaków
  - Hasło: wymagane, min 8 znaków, min 1 wielka litera, min 1 cyfra
  - Potwierdzenie hasła: wymagane, musi być identyczne z hasłem
- **Typy:**
  - `RegisterFormData` - dane formularza
  - `RegisterCommand` - request do API
  - `RegisterResponseDTO` - response z API
- **Propsy:**
  - `onSuccess?: () => void` - callback po udanej rejestracji

### 4.5 PasswordInput

- **Opis:** Reużywalne pole hasła z przyciskiem toggle do pokazywania/ukrywania wartości.
- **Główne elementy:**
  - `<div>` wrapper z `relative` positioning
  - `<label>` powiązany z inputem
  - `<input type="password|text">` - przełączany typ
  - `<button type="button">` - toggle visibility (ikona oka)
  - `<span>` - komunikat błędu z `aria-describedby`
- **Obsługiwane interakcje:**
  - `onChange` - aktualizacja wartości
  - `onBlur` - walidacja
  - `onClick` (toggle) - przełączanie widoczności
- **Obsługiwana walidacja:** Brak (delegowana do rodzica)
- **Typy:**
  - `PasswordInputProps` - propsy komponentu
- **Propsy:**
  - `id: string` - identyfikator pola
  - `name: string` - nazwa pola
  - `label: string` - etykieta
  - `value: string` - wartość
  - `onChange: (e: ChangeEvent<HTMLInputElement>) => void`
  - `onBlur?: (e: FocusEvent<HTMLInputElement>) => void`
  - `error?: string` - komunikat błędu
  - `placeholder?: string`
  - `autoComplete?: string` - np. "current-password", "new-password"
  - `autoFocus?: boolean`

### 4.6 PasswordStrengthIndicator

- **Opis:** Wizualny wskaźnik siły hasła (pasek postępu + etykieta tekstowa).
- **Główne elementy:**
  - `<div>` wrapper
  - `<div>` pasek postępu (4 segmenty)
  - `<span aria-live="polite">` - etykieta siły (Słabe/Średnie/Dobre/Silne)
- **Obsługiwane interakcje:** Brak (komponent prezentacyjny)
- **Obsługiwana walidacja:** Brak
- **Typy:**
  - `PasswordStrength` - obiekt z wynikiem analizy
  - `PasswordStrengthIndicatorProps` - propsy
- **Propsy:**
  - `strength: PasswordStrength` - obiekt z score i label

### 4.7 PasswordRequirements

- **Opis:** Lista wymagań hasła z wizualnym oznaczeniem spełnionych/niespełnionych warunków.
- **Główne elementy:**
  - `<ul>` lista wymagań
  - `<li>` dla każdego wymagania z ikoną ✓/✗
  - Kolory: zielony (spełnione), szary (niespełnione)
- **Obsługiwane interakcje:** Brak (komponent prezentacyjny)
- **Obsługiwana walidacja:** Brak
- **Typy:**
  - `PasswordRequirementsProps` - propsy
  - `PasswordStrength.requirements` - status wymagań
- **Propsy:**
  - `requirements: { minLength: boolean; hasUppercase: boolean; hasDigit: boolean }`

### 4.8 RememberMeCheckbox

- **Opis:** Checkbox "Zapamiętaj mnie" z etykietą.
- **Główne elementy:**
  - `<div>` wrapper z flex layout
  - `<input type="checkbox">`
  - `<label>` z tekstem "Zapamiętaj mnie"
- **Obsługiwane interakcje:**
  - `onChange` - toggle stanu
- **Obsługiwana walidacja:** Brak
- **Typy:**
  - `RememberMeCheckboxProps` - propsy
- **Propsy:**
  - `id: string`
  - `checked: boolean`
  - `onChange: (checked: boolean) => void`

### 4.9 PublicRoute

- **Opis:** Wrapper dla tras publicznych, przekierowuje zalogowanych użytkowników.
- **Główne elementy:**
  - Sprawdzenie stanu autentykacji
  - `Navigate` do `/` jeśli zalogowany
  - Renderowanie `children` jeśli niezalogowany
- **Obsługiwane interakcje:** Brak
- **Obsługiwana walidacja:** Brak
- **Typy:** Brak dedykowanych
- **Propsy:**
  - `children: React.ReactNode`

## 5. Typy

### 5.1 Istniejące typy (z @types.ts)

```typescript
// Request do logowania
interface LoginCommand {
  email: string;
  password: string;
}

// Response z logowania
interface LoginResponseDTO {
  accessToken: string;
  expiresAt: ISODateTime;
  user: UserDTO;
}

// Request do rejestracji
interface RegisterCommand {
  email: string;
  password: string;
  displayName: string;
}

// Response z rejestracji
interface RegisterResponseDTO {
  id: UUID;
  email: string;
  displayName: string;
  createdAt: ISODateTime;
}

// Dane użytkownika
interface UserDTO {
  id: UUID;
  email: string;
  displayName: string;
}
```

### 5.2 Nowe typy do zdefiniowania

```typescript
// === Form ViewModels ===

/**
 * Dane formularza logowania (rozszerzone o UI-only pola)
 */
interface LoginFormData {
  email: string;
  password: string;
  rememberMe: boolean;
}

/**
 * Dane formularza rejestracji (rozszerzone o confirmPassword)
 */
interface RegisterFormData {
  email: string;
  displayName: string;
  password: string;
  confirmPassword: string;
}

// === Password Strength ===

/**
 * Poziom siły hasła
 */
type PasswordStrengthLevel = 'weak' | 'fair' | 'good' | 'strong';

/**
 * Status wymagań hasła
 */
interface PasswordRequirementsStatus {
  minLength: boolean;      // >= 8 znaków
  hasUppercase: boolean;   // zawiera wielką literę
  hasDigit: boolean;       // zawiera cyfrę
}

/**
 * Pełny wynik analizy siły hasła
 */
interface PasswordStrength {
  score: number;                          // 0-4
  level: PasswordStrengthLevel;
  requirements: PasswordRequirementsStatus;
  isValid: boolean;                       // wszystkie wymagania spełnione
}

// === Component Props ===

interface PasswordInputProps {
  id: string;
  name: string;
  label: string;
  value: string;
  onChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
  onBlur?: (e: React.FocusEvent<HTMLInputElement>) => void;
  error?: string;
  placeholder?: string;
  autoComplete?: 'current-password' | 'new-password';
  autoFocus?: boolean;
  disabled?: boolean;
}

interface PasswordStrengthIndicatorProps {
  strength: PasswordStrength;
}

interface PasswordRequirementsProps {
  requirements: PasswordRequirementsStatus;
}

interface RememberMeCheckboxProps {
  id: string;
  checked: boolean;
  onChange: (checked: boolean) => void;
  disabled?: boolean;
}

// === Auth Context ===

interface AuthContextValue {
  isAuthenticated: boolean;
  user: UserDTO | null;
  isLoading: boolean;
  login: (data: LoginCommand, rememberMe?: boolean) => Promise<void>;
  register: (data: RegisterCommand) => Promise<void>;
  logout: () => Promise<void>;
}

// === API Error ===

interface AuthError {
  type: 'validation' | 'unauthorized' | 'conflict' | 'network' | 'unknown';
  message: string;
  fieldErrors?: Record<string, string>;
}
```

## 6. Zarządzanie stanem

### 6.1 AuthContext (globalny stan autentykacji)

Rozszerzenie istniejącego `useAuth` hook do pełnego kontekstu:

```typescript
// src/contexts/AuthContext.tsx
const AuthContext = createContext<AuthContextValue | null>(null);

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<UserDTO | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  // Sprawdzenie sesji przy mount
  useEffect(() => {
    checkSession();
  }, []);

  const login = async (data: LoginCommand, rememberMe = false) => {
    const response = await authApi.login(data);
    // Zapisz token
    const storage = rememberMe ? localStorage : sessionStorage;
    storage.setItem('accessToken', response.accessToken);
    storage.setItem('user', JSON.stringify(response.user));
    setUser(response.user);
  };

  const register = async (data: RegisterCommand) => {
    await authApi.register(data);
    // Auto-login po rejestracji
    await login({ email: data.email, password: data.password });
  };

  const logout = async () => {
    await authApi.logout();
    localStorage.removeItem('accessToken');
    sessionStorage.removeItem('accessToken');
    setUser(null);
  };

  return (
    <AuthContext.Provider value={{ isAuthenticated: !!user, user, isLoading, login, register, logout }}>
      {children}
    </AuthContext.Provider>
  );
}
```

### 6.2 Stan formularzy (lokalny)

Każdy formularz zarządza własnym stanem za pomocą React Hook Form (do zainstalowania):

```typescript
// LoginForm - stan lokalny
const { register, handleSubmit, formState: { errors, isSubmitting } } = useForm<LoginFormData>();

// RegisterForm - stan lokalny
const { register, handleSubmit, watch, formState: { errors, isSubmitting } } = useForm<RegisterFormData>();
const password = watch('password'); // dla real-time password strength
```

### 6.3 Custom Hooks

#### usePasswordStrength

```typescript
// src/hooks/usePasswordStrength.ts
function usePasswordStrength(password: string): PasswordStrength {
  return useMemo(() => {
    const requirements = {
      minLength: password.length >= 8,
      hasUppercase: /[A-Z]/.test(password),
      hasDigit: /\d/.test(password),
    };

    const metCount = Object.values(requirements).filter(Boolean).length;
    const score = Math.min(metCount + (password.length >= 12 ? 1 : 0), 4);

    const levels: PasswordStrengthLevel[] = ['weak', 'weak', 'fair', 'good', 'strong'];

    return {
      score,
      level: levels[score],
      requirements,
      isValid: metCount === 3,
    };
  }, [password]);
}
```

## 7. Integracja API

### 7.1 Funkcje API (src/api/auth.ts)

```typescript
const API_BASE = '/api/auth';

export const authApi = {
  /**
   * POST /api/auth/login
   * @param data LoginCommand
   * @returns LoginResponseDTO
   * @throws ApiErrorResponse (400, 401)
   */
  login: async (data: LoginCommand): Promise<LoginResponseDTO> => {
    const response = await fetch(`${API_BASE}/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      const error = await response.json();
      throw error as ApiErrorResponse;
    }

    return response.json();
  },

  /**
   * POST /api/auth/register
   * @param data RegisterCommand
   * @returns RegisterResponseDTO
   * @throws ApiErrorResponse (400, 409)
   */
  register: async (data: RegisterCommand): Promise<RegisterResponseDTO> => {
    const response = await fetch(`${API_BASE}/register`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      const error = await response.json();
      throw error as ApiErrorResponse;
    }

    return response.json();
  },

  /**
   * POST /api/auth/logout
   * @throws ApiErrorResponse (401)
   */
  logout: async (): Promise<void> => {
    const token = localStorage.getItem('accessToken') || sessionStorage.getItem('accessToken');

    await fetch(`${API_BASE}/logout`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`,
      },
    });
  },
};
```

### 7.2 Typy żądań i odpowiedzi

| Endpoint | Request Type | Response Type | Error Codes |
|----------|--------------|---------------|-------------|
| POST /api/auth/login | `LoginCommand` | `LoginResponseDTO` | 400, 401 |
| POST /api/auth/register | `RegisterCommand` | `RegisterResponseDTO` | 400, 409 |
| POST /api/auth/logout | - (token w header) | 204 No Content | 401 |

## 8. Interakcje użytkownika

### 8.1 Logowanie

| Interakcja | Akcja | Rezultat |
|------------|-------|----------|
| Wejście na `/login` (zalogowany) | - | Redirect do `/` |
| Focus na stronie | Auto-focus | Kursor w polu email |
| Wpisanie email | onChange | Aktualizacja stanu, walidacja on blur |
| Wpisanie hasła | onChange | Aktualizacja stanu |
| Klik toggle visibility | onClick | Przełączenie type password/text |
| Zaznaczenie "Zapamiętaj mnie" | onChange | Toggle checked |
| Submit z błędami walidacji | onSubmit | Wyświetlenie błędów przy polach |
| Submit poprawny | onSubmit | Spinner, request API |
| API success | - | Zapisanie tokena, redirect do `/` |
| API error 401 | - | "Nieprawidłowy email lub hasło" |
| Klik "Zarejestruj się" | onClick | Navigate do `/register` |

### 8.2 Rejestracja

| Interakcja | Akcja | Rezultat |
|------------|-------|----------|
| Wejście na `/register` (zalogowany) | - | Redirect do `/` |
| Focus na stronie | Auto-focus | Kursor w polu email |
| Wpisanie email | onChange | Aktualizacja, walidacja on blur |
| Wpisanie displayName | onChange | Aktualizacja, walidacja on blur |
| Wpisanie hasła | onChange | Real-time aktualizacja strength indicator |
| Zmiana hasła | onChange | Aktualizacja PasswordRequirements (✓/✗) |
| Wpisanie potwierdzenia | onChange | Walidacja zgodności |
| Submit z błędami | onSubmit | Wyświetlenie błędów |
| Submit poprawny | onSubmit | Spinner, request API |
| API success | - | Auto-login, redirect do `/` |
| API error 409 | - | "Ten adres email jest już zarejestrowany" |
| Klik "Zaloguj się" | onClick | Navigate do `/login` |

## 9. Warunki i walidacja

### 9.1 Walidacja formularza logowania

| Pole | Warunek | Komunikat błędu | Kiedy sprawdzane |
|------|---------|-----------------|------------------|
| email | Wymagane | "Email jest wymagany" | blur, submit |
| email | Format | "Nieprawidłowy format email" | blur, submit |
| password | Wymagane | "Hasło jest wymagane" | blur, submit |

### 9.2 Walidacja formularza rejestracji

| Pole | Warunek | Komunikat błędu | Kiedy sprawdzane |
|------|---------|-----------------|------------------|
| email | Wymagane | "Email jest wymagany" | blur, submit |
| email | Format | "Nieprawidłowy format email" | blur, submit |
| displayName | Wymagane | "Nazwa wyświetlana jest wymagana" | blur, submit |
| displayName | Max 100 znaków | "Nazwa może mieć max 100 znaków" | blur, submit |
| password | Wymagane | "Hasło jest wymagane" | blur, submit |
| password | Min 8 znaków | "Hasło musi mieć min 8 znaków" | change (indicator), submit |
| password | Wielka litera | "Hasło musi zawierać wielką literę" | change (indicator), submit |
| password | Cyfra | "Hasło musi zawierać cyfrę" | change (indicator), submit |
| confirmPassword | Wymagane | "Potwierdzenie hasła jest wymagane" | blur, submit |
| confirmPassword | Zgodność | "Hasła muszą być identyczne" | blur, submit |

### 9.3 Wpływ walidacji na UI

- **Błąd pola:** Czerwona ramka, komunikat pod polem, `aria-invalid="true"`
- **Pole poprawne:** Normalna ramka (lub zielona dla hasła w rejestracji)
- **Przycisk submit:** Disabled podczas `isSubmitting`, wyświetla spinner
- **Password strength:** Aktualizacja paska i etykiety w czasie rzeczywistym

## 10. Obsługa błędów

### 10.1 Błędy walidacji (400)

```typescript
// Przykład response
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Validation Failed",
  "status": 400,
  "errors": {
    "email": ["Invalid email format"],
    "password": ["Password is too weak"]
  }
}
```

**Obsługa:** Mapowanie `errors` na pola formularza, wyświetlenie przy odpowiednich inputach.

### 10.2 Błąd uwierzytelnienia (401) - Login

**Komunikat:** "Nieprawidłowy email lub hasło" (generyczny dla bezpieczeństwa)

**Obsługa:** Wyświetlenie alertu nad formularzem, NIE wskazujemy które pole jest błędne.

### 10.3 Konflikt email (409) - Rejestracja

**Komunikat:** "Ten adres email jest już zarejestrowany"

**Obsługa:** Wyświetlenie błędu przy polu email + link do logowania.

### 10.4 Błędy sieciowe

**Komunikat:** "Wystąpił błąd połączenia. Spróbuj ponownie."

**Obsługa:** Alert z przyciskiem retry, logowanie błędu w konsoli.

### 10.5 Błędy serwera (500)

**Komunikat:** "Wystąpił błąd serwera. Spróbuj ponownie później."

**Obsługa:** Alert informacyjny, możliwość retry.

## 11. Kroki implementacji

### Faza 1: Przygotowanie (1-2h)

1. **Instalacja zależności:**
   ```bash
   npm install react-hook-form zod @hookform/resolvers
   ```

2. **Utworzenie struktury folderów:**
   ```
   src/
   ├── components/auth/
   ├── contexts/
   ├── pages/
   └── api/
   ```

3. **Dodanie typów do `@types.ts`:**
   - `LoginFormData`
   - `RegisterFormData`
   - `PasswordStrength`
   - `PasswordStrengthLevel`
   - `PasswordRequirementsStatus`
   - Props interfaces dla komponentów

### Faza 2: API i kontekst (1-2h)

4. **Implementacja `src/api/auth.ts`:**
   - Funkcje `login`, `register`, `logout`
   - Obsługa błędów API

5. **Implementacja `src/contexts/AuthContext.tsx`:**
   - AuthProvider
   - Stan użytkownika i tokenów
   - Metody login/register/logout
   - Hook `useAuth`

6. **Aktualizacja `App.tsx`:**
   - Wrap w AuthProvider
   - Dodanie routingu dla `/login` i `/register`

### Faza 3: Komponenty bazowe (2-3h)

7. **Implementacja `PasswordInput.tsx`:**
   - Input z toggle visibility
   - Obsługa błędów z aria-describedby
   - Styling Tailwind

8. **Implementacja `PasswordStrengthIndicator.tsx`:**
   - Pasek postępu (4 segmenty)
   - Etykieta z aria-live
   - Kolory: czerwony → pomarańczowy → żółty → zielony

9. **Implementacja `PasswordRequirements.tsx`:**
   - Lista wymagań
   - Ikony ✓/✗
   - Dynamiczne kolory

10. **Implementacja `RememberMeCheckbox.tsx`:**
    - Checkbox z etykietą
    - Styling spójny z resztą UI

11. **Implementacja `PublicRoute.tsx`:**
    - Sprawdzenie auth state
    - Redirect jeśli zalogowany

### Faza 4: Hook usePasswordStrength (0.5h)

12. **Implementacja `src/hooks/usePasswordStrength.ts`:**
    - Logika obliczania siły
    - Memoizacja wyniku

### Faza 5: Formularze (2-3h)

13. **Implementacja `LoginForm.tsx`:**
    - React Hook Form + Zod schema
    - Integracja z PasswordInput
    - Obsługa submit i błędów
    - Auto-focus na email

14. **Implementacja `RegisterForm.tsx`:**
    - React Hook Form + Zod schema
    - Real-time password strength
    - Walidacja confirmPassword
    - Obsługa submit i błędów

### Faza 6: Strony (1h)

15. **Implementacja `LoginPage.tsx`:**
    - Layout z formularzem
    - Link do rejestracji
    - Obsługa redirect po logowaniu

16. **Implementacja `RegisterPage.tsx`:**
    - Layout z formularzem
    - Link do logowania
    - Obsługa redirect po rejestracji

### Faza 7: Integracja i testy (1-2h)

17. **Aktualizacja routingu w `App.tsx`:**
    - PublicRoute wrapper
    - Ścieżki `/login` i `/register`

18. **Testy manualne:**
    - Walidacja formularzy
    - Flow logowania
    - Flow rejestracji
    - Obsługa błędów
    - Redirect dla zalogowanych
    - Responsywność

19. **Poprawki UX:**
    - Auto-focus
    - Loading states
    - Error messages
    - Accessibility audit

### Podsumowanie czasowe

| Faza | Szacowany czas |
|------|----------------|
| Przygotowanie | 1-2h |
| API i kontekst | 1-2h |
| Komponenty bazowe | 2-3h |
| Hook usePasswordStrength | 0.5h |
| Formularze | 2-3h |
| Strony | 1h |
| Integracja i testy | 1-2h |
| **Razem** | **8-13h** |
