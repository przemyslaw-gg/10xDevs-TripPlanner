import { useState, useEffect, useCallback } from 'react';
import { useSearchParams } from 'react-router-dom';
import type {
  AttractionListItemDTO,
  LocationListItemDTO,
  PaginationDTO,
  UUID,
} from '../@types';
import { fetchAttractions } from '../api/attractions';
import { fetchLocations } from '../api/locations';

export interface UseAttractionsReturn {
  // Data
  attractions: AttractionListItemDTO[];
  locations: LocationListItemDTO[];
  pagination: PaginationDTO | null;

  // Loading states
  isLoadingAttractions: boolean;
  isLoadingLocations: boolean;

  // Errors
  error: string | null;

  // Filters
  selectedLocationId: UUID | null;
  currentPage: number;

  // Actions
  setLocationFilter: (locationId: UUID | null) => void;
  setPage: (page: number) => void;
  refreshAttractions: () => void;
}

const PAGE_SIZE = 10;

function isValidUUID(value: string): boolean {
  const uuidRegex =
    /^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i;
  return uuidRegex.test(value);
}

function parsePageParam(value: string | null): number {
  if (!value) return 1;
  const parsed = parseInt(value, 10);
  return isNaN(parsed) || parsed < 1 ? 1 : parsed;
}

function parseLocationIdParam(value: string | null): UUID | null {
  if (!value) return null;
  return isValidUUID(value) ? value : null;
}

export function useAttractions(): UseAttractionsReturn {
  const [searchParams, setSearchParams] = useSearchParams();

  // Parse URL params
  const selectedLocationId = parseLocationIdParam(
    searchParams.get('locationId')
  );
  const currentPage = parsePageParam(searchParams.get('page'));

  // State
  const [attractions, setAttractions] = useState<AttractionListItemDTO[]>([]);
  const [locations, setLocations] = useState<LocationListItemDTO[]>([]);
  const [pagination, setPagination] = useState<PaginationDTO | null>(null);
  const [isLoadingAttractions, setIsLoadingAttractions] = useState(true);
  const [isLoadingLocations, setIsLoadingLocations] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // Fetch locations (once on mount)
  useEffect(() => {
    let isCancelled = false;

    async function loadLocations() {
      setIsLoadingLocations(true);
      try {
        const response = await fetchLocations({ pageSize: 100 });
        if (!isCancelled) {
          setLocations(response.items);
        }
      } catch (err) {
        if (!isCancelled) {
          console.error('Failed to fetch locations:', err);
        }
      } finally {
        if (!isCancelled) {
          setIsLoadingLocations(false);
        }
      }
    }

    loadLocations();

    return () => {
      isCancelled = true;
    };
  }, []);

  // Fetch attractions when filters change
  const loadAttractions = useCallback(async () => {
    setIsLoadingAttractions(true);
    setError(null);

    try {
      const response = await fetchAttractions({
        locationId: selectedLocationId ?? undefined,
        page: currentPage,
        pageSize: PAGE_SIZE,
        sortBy: 'rating',
        sortOrder: 'desc',
      });

      setAttractions(response.items);
      setPagination(response.pagination);

      // If current page is greater than total pages, redirect to last page
      if (
        response.pagination.totalPages > 0 &&
        currentPage > response.pagination.totalPages
      ) {
        setSearchParams((prev) => {
          const newParams = new URLSearchParams(prev);
          newParams.set('page', String(response.pagination.totalPages));
          return newParams;
        });
      }
    } catch (err) {
      const message =
        err instanceof Error ? err.message : 'Failed to fetch attractions';
      setError(message);
      setAttractions([]);
      setPagination(null);
    } finally {
      setIsLoadingAttractions(false);
    }
  }, [selectedLocationId, currentPage, setSearchParams]);

  useEffect(() => {
    loadAttractions();
  }, [loadAttractions]);

  // Actions
  const setLocationFilter = useCallback(
    (locationId: UUID | null) => {
      setSearchParams((prev) => {
        const newParams = new URLSearchParams(prev);
        if (locationId) {
          newParams.set('locationId', locationId);
        } else {
          newParams.delete('locationId');
        }
        // Reset page to 1 when changing location filter
        newParams.set('page', '1');
        return newParams;
      });
    },
    [setSearchParams]
  );

  const setPage = useCallback(
    (page: number) => {
      if (page < 1) return;
      setSearchParams((prev) => {
        const newParams = new URLSearchParams(prev);
        newParams.set('page', String(page));
        return newParams;
      });
    },
    [setSearchParams]
  );

  const refreshAttractions = useCallback(() => {
    loadAttractions();
  }, [loadAttractions]);

  return {
    attractions,
    locations,
    pagination,
    isLoadingAttractions,
    isLoadingLocations,
    error,
    selectedLocationId,
    currentPage,
    setLocationFilter,
    setPage,
    refreshAttractions,
  };
}
