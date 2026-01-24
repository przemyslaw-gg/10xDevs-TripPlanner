import type {
  LoginCommand,
  LoginResponseDTO,
  RegisterCommand,
  RegisterResponseDTO,
  ApiErrorResponse,
  CurrentUserDTO,
  UserDTO,
} from '../@types';

const API_BASE_URL = '/api/auth';

/**
 * Parse API error response and throw appropriate error
 */
async function handleErrorResponse(response: Response): Promise<never> {
  let errorData: ApiErrorResponse;

  try {
    errorData = await response.json();
  } catch {
    throw {
      type: 'https://tools.ietf.org/html/rfc7231#section-6.6.1',
      title: 'Error',
      status: response.status,
      detail: 'An unexpected error occurred',
    } as ApiErrorResponse;
  }

  throw errorData;
}

/**
 * Get the stored access token from localStorage or sessionStorage
 */
export function getStoredToken(): string | null {
  return localStorage.getItem('accessToken') || sessionStorage.getItem('accessToken');
}

/**
 * Get the stored user from localStorage or sessionStorage
 */
export function getStoredUser(): UserDTO | null {
  const userJson = localStorage.getItem('user') || sessionStorage.getItem('user');
  if (!userJson) return null;

  try {
    return JSON.parse(userJson);
  } catch {
    return null;
  }
}

/**
 * Clear all stored auth data
 */
export function clearStoredAuth(): void {
  localStorage.removeItem('accessToken');
  localStorage.removeItem('user');
  sessionStorage.removeItem('accessToken');
  sessionStorage.removeItem('user');
}

/**
 * Store auth data in the appropriate storage
 */
export function storeAuth(
  accessToken: string,
  user: UserDTO,
  rememberMe: boolean
): void {
  const storage = rememberMe ? localStorage : sessionStorage;
  storage.setItem('accessToken', accessToken);
  storage.setItem('user', JSON.stringify(user));
}

export const authApi = {
  /**
   * POST /api/auth/login
   * Authenticate user and return access token
   */
  login: async (data: LoginCommand): Promise<LoginResponseDTO> => {
    const response = await fetch(`${API_BASE_URL}/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      await handleErrorResponse(response);
    }

    return response.json();
  },

  /**
   * POST /api/auth/register
   * Register a new user account
   */
  register: async (data: RegisterCommand): Promise<RegisterResponseDTO> => {
    const response = await fetch(`${API_BASE_URL}/register`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      await handleErrorResponse(response);
    }

    return response.json();
  },

  /**
   * POST /api/auth/logout
   * Invalidate the current session
   */
  logout: async (): Promise<void> => {
    const token = getStoredToken();

    if (!token) {
      clearStoredAuth();
      return;
    }

    try {
      await fetch(`${API_BASE_URL}/logout`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`,
        },
      });
    } finally {
      clearStoredAuth();
    }
  },

  /**
   * GET /api/auth/me
   * Get current user information
   */
  getCurrentUser: async (): Promise<CurrentUserDTO> => {
    const token = getStoredToken();

    if (!token) {
      throw {
        type: 'https://tools.ietf.org/html/rfc7235#section-3.1',
        title: 'Unauthorized',
        status: 401,
        detail: 'No access token available',
      } as ApiErrorResponse;
    }

    const response = await fetch(`${API_BASE_URL}/me`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`,
      },
    });

    if (!response.ok) {
      await handleErrorResponse(response);
    }

    return response.json();
  },
};
