import type {
  AttractionsQueryParams,
  AttractionsResponseDTO,
  CreateAttractionCommand,
  AttractionDTO,
  ApiErrorResponse,
} from '../@types';
import { getStoredToken } from './auth';

const API_BASE_URL = '/api';

export async function fetchAttractions(
  params: AttractionsQueryParams
): Promise<AttractionsResponseDTO> {
  const queryString = new URLSearchParams();

  if (params.locationId) {
    queryString.set('locationId', params.locationId);
  }
  if (params.search) {
    queryString.set('search', params.search);
  }
  if (params.sortBy) {
    queryString.set('sortBy', params.sortBy);
  }
  if (params.sortOrder) {
    queryString.set('sortOrder', params.sortOrder);
  }
  if (params.isVerified !== undefined) {
    queryString.set('isVerified', String(params.isVerified));
  }

  queryString.set('page', String(params.page ?? 1));
  queryString.set('pageSize', String(params.pageSize ?? 10));

  const response = await fetch(`${API_BASE_URL}/attractions?${queryString}`);

  if (!response.ok) {
    const errorData = await response.json().catch(() => null);
    throw new Error(
      errorData?.detail || errorData?.message || 'Failed to fetch attractions'
    );
  }

  return response.json();
}

/**
 * Get authorization headers with token
 */
function getAuthHeaders(): Record<string, string> {
  const token = getStoredToken();
  return {
    'Content-Type': 'application/json',
    ...(token ? { Authorization: `Bearer ${token}` } : {}),
  };
}

/**
 * POST /api/attractions
 * Create a new attraction
 */
export async function createAttraction(
  data: CreateAttractionCommand
): Promise<AttractionDTO> {
  const response = await fetch(`${API_BASE_URL}/attractions`, {
    method: 'POST',
    headers: getAuthHeaders(),
    body: JSON.stringify(data),
  });

  if (!response.ok) {
    const errorData = await response.json().catch(() => null);
    throw errorData || { status: response.status, detail: 'Failed to create attraction' };
  }

  return response.json();
}

/**
 * Helper to parse attraction API errors into user-friendly messages
 */
export function parseAttractionError(error: unknown): string {
  if (error && typeof error === 'object' && 'status' in error) {
    const apiError = error as ApiErrorResponse;

    switch (apiError.status) {
      case 400:
        if (apiError.errors) {
          const firstField = Object.keys(apiError.errors)[0];
          if (firstField && apiError.errors[firstField]?.[0]) {
            return apiError.errors[firstField][0];
          }
        }
        return apiError.detail || 'Nieprawidłowe dane formularza';
      case 401:
        return 'Sesja wygasła. Zaloguj się ponownie.';
      case 404:
        return 'Lokalizacja nie została znaleziona';
      default:
        return apiError.detail || 'Wystąpił nieoczekiwany błąd';
    }
  }

  if (error instanceof Error) {
    return error.message;
  }

  return 'Wystąpił nieoczekiwany błąd';
}
