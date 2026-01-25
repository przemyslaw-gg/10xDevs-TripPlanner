import { useAuthContext } from '../contexts/AuthContext';
import type { UUID } from '../@types';

interface UseAuthReturn {
  isAuthenticated: boolean;
  currentUserId: UUID | null;
  isLoading: boolean;
}

/**
 * Hook for authentication state.
 * Wraps useAuthContext for backward compatibility.
 */
export function useAuth(): UseAuthReturn {
  const { isAuthenticated, user, isLoading } = useAuthContext();

  return {
    isAuthenticated,
    currentUserId: user?.id ?? null,
    isLoading,
  };
}
