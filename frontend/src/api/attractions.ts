import type { AttractionsQueryParams, AttractionsResponseDTO } from '../@types';

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
