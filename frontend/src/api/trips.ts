import type {
  TripsQueryParams,
  TripsResponseDTO,
  TripDTO,
  CreateTripCommand,
  UpdateTripCommand,
  TripAttractionsResponseDTO,
  AddTripAttractionCommand,
  TripAttractionItemDTO,
  ReorderTripAttractionsCommand,
  ReorderAttractionsResponseDTO,
  OptimizeRouteCommand,
  OptimizeRouteResponseDTO,
  ApiErrorResponse,
  UUID,
} from '../@types';
import { getStoredToken } from './auth';

const API_BASE_URL = '/api';

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
 * Get authorization headers with token
 */
function getAuthHeaders(): Record<string, string> {
  const token = getStoredToken();
  return {
    'Content-Type': 'application/json',
    ...(token ? { 'Authorization': `Bearer ${token}` } : {}),
  };
}

/**
 * GET /api/trips
 * Fetch paginated list of trips with optional filtering
 */
export async function fetchTrips(
  params: TripsQueryParams = {}
): Promise<TripsResponseDTO> {
  const queryString = new URLSearchParams();

  if (params.locationId) {
    queryString.set('locationId', params.locationId);
  }
  if (params.onlyMine) {
    queryString.set('onlyMine', 'true');
  }
  if (params.onlyPublic) {
    queryString.set('onlyPublic', 'true');
  }
  if (params.search) {
    queryString.set('search', params.search);
  }

  queryString.set('page', String(params.page ?? 1));
  queryString.set('pageSize', String(params.pageSize ?? 20));

  const response = await fetch(`${API_BASE_URL}/trips?${queryString}`, {
    method: 'GET',
    headers: getAuthHeaders(),
  });

  if (!response.ok) {
    await handleErrorResponse(response);
  }

  return response.json();
}

/**
 * GET /api/trips/{id}
 * Fetch a single trip by ID
 */
export async function fetchTrip(id: UUID): Promise<TripDTO> {
  const response = await fetch(`${API_BASE_URL}/trips/${id}`, {
    method: 'GET',
    headers: getAuthHeaders(),
  });

  if (!response.ok) {
    await handleErrorResponse(response);
  }

  return response.json();
}

/**
 * POST /api/trips
 * Create a new trip
 */
export async function createTrip(data: CreateTripCommand): Promise<TripDTO> {
  const response = await fetch(`${API_BASE_URL}/trips`, {
    method: 'POST',
    headers: getAuthHeaders(),
    body: JSON.stringify(data),
  });

  if (!response.ok) {
    await handleErrorResponse(response);
  }

  return response.json();
}

/**
 * PUT /api/trips/{id}
 * Update an existing trip
 */
export async function updateTrip(id: UUID, data: UpdateTripCommand): Promise<TripDTO> {
  const response = await fetch(`${API_BASE_URL}/trips/${id}`, {
    method: 'PUT',
    headers: getAuthHeaders(),
    body: JSON.stringify(data),
  });

  if (!response.ok) {
    await handleErrorResponse(response);
  }

  return response.json();
}

/**
 * DELETE /api/trips/{id}
 * Delete a trip
 */
export async function deleteTrip(id: UUID): Promise<void> {
  const response = await fetch(`${API_BASE_URL}/trips/${id}`, {
    method: 'DELETE',
    headers: getAuthHeaders(),
  });

  if (!response.ok) {
    await handleErrorResponse(response);
  }
}

/**
 * GET /api/trips/{tripId}/attractions
 * Fetch trip attractions grouped by day
 */
export async function fetchTripAttractions(tripId: UUID): Promise<TripAttractionsResponseDTO> {
  const response = await fetch(`${API_BASE_URL}/trips/${tripId}/attractions`, {
    method: 'GET',
    headers: getAuthHeaders(),
  });

  if (!response.ok) {
    await handleErrorResponse(response);
  }

  return response.json();
}

/**
 * POST /api/trips/{tripId}/attractions
 * Add an attraction to a trip
 */
export async function addAttractionToTrip(
  tripId: UUID,
  data: AddTripAttractionCommand
): Promise<TripAttractionItemDTO> {
  const response = await fetch(`${API_BASE_URL}/trips/${tripId}/attractions`, {
    method: 'POST',
    headers: getAuthHeaders(),
    body: JSON.stringify(data),
  });

  if (!response.ok) {
    await handleErrorResponse(response);
  }

  return response.json();
}

/**
 * DELETE /api/trips/{tripId}/attractions/{attractionId}
 * Remove an attraction from a trip
 */
export async function removeTripAttraction(tripId: UUID, attractionId: UUID): Promise<void> {
  const response = await fetch(`${API_BASE_URL}/trips/${tripId}/attractions/${attractionId}`, {
    method: 'DELETE',
    headers: getAuthHeaders(),
  });

  if (!response.ok) {
    await handleErrorResponse(response);
  }
}

/**
 * POST /api/trips/{tripId}/attractions/reorder
 * Batch reorder attractions in a trip
 */
export async function reorderTripAttractions(
  tripId: UUID,
  data: ReorderTripAttractionsCommand
): Promise<ReorderAttractionsResponseDTO> {
  const response = await fetch(`${API_BASE_URL}/trips/${tripId}/attractions/reorder`, {
    method: 'POST',
    headers: getAuthHeaders(),
    body: JSON.stringify(data),
  });

  if (!response.ok) {
    await handleErrorResponse(response);
  }

  return response.json();
}

/**
 * POST /api/trips/{tripId}/optimize-route
 * Optimize the visiting order of attractions using nearest neighbor algorithm
 */
export async function optimizeRoute(
  tripId: UUID,
  data: OptimizeRouteCommand
): Promise<OptimizeRouteResponseDTO> {
  const response = await fetch(`${API_BASE_URL}/trips/${tripId}/optimize-route`, {
    method: 'POST',
    headers: getAuthHeaders(),
    body: JSON.stringify(data),
  });

  if (!response.ok) {
    await handleErrorResponse(response);
  }

  return response.json();
}

/**
 * Helper to parse trip-related API errors into user-friendly messages
 */
export function parseTripError(error: unknown): string {
  if (error && typeof error === 'object' && 'status' in error) {
    const apiError = error as ApiErrorResponse;

    switch (apiError.status) {
      case 400:
        return apiError.detail || 'Nieprawidłowe dane formularza';
      case 401:
        return 'Sesja wygasła. Zaloguj się ponownie.';
      case 403:
        return 'Nie masz uprawnień do tej wycieczki';
      case 404:
        return 'Wycieczka nie została znaleziona';
      case 409:
        return 'Atrakcja jest już dodana do wycieczki';
      case 422:
        return 'Dodaj atrakcje do wycieczki przed optymalizacją';
      default:
        return apiError.detail || 'Wystąpił nieoczekiwany błąd';
    }
  }

  if (error instanceof Error) {
    return error.message;
  }

  return 'Wystąpił nieoczekiwany błąd';
}
