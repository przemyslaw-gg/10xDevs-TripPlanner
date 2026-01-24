import { useState, useEffect } from 'react';
import type { UUID } from '../@types';

interface UseAuthReturn {
  isAuthenticated: boolean;
  currentUserId: UUID | null;
  isLoading: boolean;
}

/**
 * Hook for authentication state.
 * TODO: Replace with actual authentication implementation (e.g., context, API call)
 */
export function useAuth(): UseAuthReturn {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [currentUserId, setCurrentUserId] = useState<UUID | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    // Placeholder: Check for auth token in localStorage
    const checkAuth = () => {
      try {
        const token = localStorage.getItem('accessToken');
        const userId = localStorage.getItem('userId');

        if (token && userId) {
          setIsAuthenticated(true);
          setCurrentUserId(userId);
        } else {
          setIsAuthenticated(false);
          setCurrentUserId(null);
        }
      } catch {
        setIsAuthenticated(false);
        setCurrentUserId(null);
      } finally {
        setIsLoading(false);
      }
    };

    checkAuth();
  }, []);

  return {
    isAuthenticated,
    currentUserId,
    isLoading,
  };
}
