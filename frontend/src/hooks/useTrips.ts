import { useState, useEffect, useCallback } from 'react';
import { useSearchParams } from 'react-router-dom';
import type {
  TripListItemDTO,
  PaginationDTO,
  UseTripsReturn,
} from '../@types';
import { fetchTrips, parseTripError } from '../api/trips';

const PAGE_SIZE = 10;

function parsePageParam(value: string | null): number {
  if (!value) return 1;
  const parsed = parseInt(value, 10);
  return isNaN(parsed) || parsed < 1 ? 1 : parsed;
}

export function useTrips(): UseTripsReturn {
  const [searchParams, setSearchParams] = useSearchParams();

  // Parse URL params
  const currentPage = parsePageParam(searchParams.get('page'));

  // State
  const [trips, setTrips] = useState<TripListItemDTO[]>([]);
  const [pagination, setPagination] = useState<PaginationDTO | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // Fetch trips
  const loadTrips = useCallback(async () => {
    setIsLoading(true);
    setError(null);

    try {
      const response = await fetchTrips({
        onlyMine: true,
        page: currentPage,
        pageSize: PAGE_SIZE,
      });

      setTrips(response.items);
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
      const message = parseTripError(err);
      setError(message);
      setTrips([]);
      setPagination(null);
    } finally {
      setIsLoading(false);
    }
  }, [currentPage, setSearchParams]);

  useEffect(() => {
    loadTrips();
  }, [loadTrips]);

  // Actions
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

  const refresh = useCallback(() => {
    loadTrips();
  }, [loadTrips]);

  return {
    trips,
    pagination,
    isLoading,
    error,
    currentPage,
    setPage,
    refresh,
  };
}
