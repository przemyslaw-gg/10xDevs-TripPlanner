import {
  createContext,
  useContext,
  useState,
  useEffect,
  useCallback,
  type ReactNode,
} from 'react';
import type {
  AuthContextValue,
  LoginCommand,
  RegisterCommand,
  UserDTO,
  ApiErrorResponse,
} from '../@types';
import {
  authApi,
  getStoredToken,
  getStoredUser,
  storeAuth,
  clearStoredAuth,
} from '../api/auth';

const AuthContext = createContext<AuthContextValue | null>(null);

interface AuthProviderProps {
  children: ReactNode;
}

export function AuthProvider({ children }: AuthProviderProps) {
  const [user, setUser] = useState<UserDTO | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  // Check session on mount
  useEffect(() => {
    const checkSession = async () => {
      const token = getStoredToken();
      const storedUser = getStoredUser();

      if (!token || !storedUser) {
        setIsLoading(false);
        return;
      }

      try {
        // Verify token is still valid by fetching current user
        const currentUser = await authApi.getCurrentUser();
        setUser(currentUser);
      } catch {
        // Token is invalid, clear stored auth
        clearStoredAuth();
        setUser(null);
      } finally {
        setIsLoading(false);
      }
    };

    checkSession();
  }, []);

  const login = useCallback(async (data: LoginCommand, rememberMe = false) => {
    const response = await authApi.login(data);

    // Store auth data
    storeAuth(
      response.accessToken,
      response.user,
      rememberMe
    );

    setUser(response.user);
  }, []);

  const register = useCallback(async (data: RegisterCommand) => {
    // Register the user
    await authApi.register(data);

    // Auto-login after successful registration
    await login({ email: data.email, password: data.password }, false);
  }, [login]);

  const logout = useCallback(async () => {
    try {
      await authApi.logout();
    } catch {
      // Even if logout API fails, clear local state
    } finally {
      clearStoredAuth();
      setUser(null);
    }
  }, []);

  const value: AuthContextValue = {
    isAuthenticated: !!user,
    user,
    isLoading,
    login,
    register,
    logout,
  };

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  );
}

/**
 * Hook to access auth context
 * @throws Error if used outside of AuthProvider
 */
export function useAuthContext(): AuthContextValue {
  const context = useContext(AuthContext);

  if (!context) {
    throw new Error('useAuthContext must be used within an AuthProvider');
  }

  return context;
}

/**
 * Helper to parse API errors into user-friendly messages
 */
export function parseAuthError(error: unknown): string {
  if (!error) {
    return 'Wystąpił nieznany błąd';
  }

  const apiError = error as ApiErrorResponse;

  // Handle specific status codes
  if (apiError.status === 401) {
    return 'Nieprawidłowy email lub hasło';
  }

  if (apiError.status === 409) {
    return 'Ten adres email jest już zarejestrowany';
  }

  if (apiError.status === 400 && apiError.errors) {
    // Return first validation error
    const firstField = Object.keys(apiError.errors)[0];
    if (firstField && apiError.errors[firstField]?.[0]) {
      return apiError.errors[firstField][0];
    }
  }

  if (apiError.detail) {
    return apiError.detail;
  }

  // Network error
  if (error instanceof TypeError && error.message === 'Failed to fetch') {
    return 'Wystąpił błąd połączenia. Spróbuj ponownie.';
  }

  return 'Wystąpił błąd serwera. Spróbuj ponownie później.';
}
