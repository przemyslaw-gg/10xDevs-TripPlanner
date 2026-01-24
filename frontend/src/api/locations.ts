import type { LocationsQueryParams, LocationsResponseDTO } from '../@types';

const API_BASE_URL = '/api';

export async function fetchLocations(
  params: LocationsQueryParams = {}
): Promise<LocationsResponseDTO> {
  const queryString = new URLSearchParams();

  if (params.search) {
    queryString.set('search', params.search);
  }

  queryString.set('page', String(params.page ?? 1));
  queryString.set('pageSize', String(params.pageSize ?? 100));

  const response = await fetch(`${API_BASE_URL}/locations?${queryString}`);

  if (!response.ok) {
    const errorData = await response.json().catch(() => null);
    throw new Error(
      errorData?.detail || errorData?.message || 'Failed to fetch locations'
    );
  }

  return response.json();
}
