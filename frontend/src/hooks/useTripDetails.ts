import { useState, useEffect, useCallback, useRef, useMemo } from 'react';
import type {
  TripDTO,
  TripAttractionItemDTO,
  UpdateTripCommand,
  SaveStatus,
  UUID,
} from '../@types';
import {
  fetchTrip,
  fetchTripAttractions,
  updateTrip,
  deleteTrip as deleteTripApi,
  removeTripAttraction,
  reorderTripAttractions,
  parseTripError,
} from '../api/trips';

// Simple debounce implementation
function debounce<T extends (...args: Parameters<T>) => void>(
  fn: T,
  delay: number
): { (...args: Parameters<T>): void; cancel: () => void } {
  let timeoutId: ReturnType<typeof setTimeout> | null = null;

  const debouncedFn = (...args: Parameters<T>) => {
    if (timeoutId) {
      clearTimeout(timeoutId);
    }
    timeoutId = setTimeout(() => {
      fn(...args);
      timeoutId = null;
    }, delay);
  };

  debouncedFn.cancel = () => {
    if (timeoutId) {
      clearTimeout(timeoutId);
      timeoutId = null;
    }
  };

  return debouncedFn;
}

export interface UseTripDetailsReturn {
  trip: TripDTO | null;
  attractions: TripAttractionItemDTO[];
  isLoading: boolean;
  isSaving: boolean;
  saveStatus: SaveStatus;
  error: string | null;

  // Actions
  updateTripData: (updates: Partial<UpdateTripCommand>) => void;
  moveAttractionUp: (attractionId: UUID) => void;
  moveAttractionDown: (attractionId: UUID) => void;
  removeAttraction: (attractionId: UUID) => Promise<void>;
  deleteTrip: () => Promise<void>;
  refresh: () => void;
}

const DEBOUNCE_DELAY = 2000; // 2 seconds
const SAVED_STATUS_DURATION = 2000; // Show "Saved" for 2 seconds

export function useTripDetails(tripId: UUID): UseTripDetailsReturn {
  // State
  const [trip, setTrip] = useState<TripDTO | null>(null);
  const [attractions, setAttractions] = useState<TripAttractionItemDTO[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);
  const [saveStatus, setSaveStatus] = useState<SaveStatus>('idle');
  const [error, setError] = useState<string | null>(null);

  // Refs for pending updates
  const pendingUpdatesRef = useRef<Partial<UpdateTripCommand>>({});
  const savedStatusTimeoutRef = useRef<ReturnType<typeof setTimeout> | null>(null);

  // Clear saved status after delay
  const clearSavedStatus = useCallback(() => {
    if (savedStatusTimeoutRef.current) {
      clearTimeout(savedStatusTimeoutRef.current);
    }
    savedStatusTimeoutRef.current = setTimeout(() => {
      setSaveStatus((current) => (current === 'saved' ? 'idle' : current));
    }, SAVED_STATUS_DURATION);
  }, []);

  // Save trip to API
  const saveTrip = useCallback(async () => {
    if (!trip || Object.keys(pendingUpdatesRef.current).length === 0) {
      return;
    }

    setIsSaving(true);
    setSaveStatus('saving');

    try {
      const currentUpdates = { ...pendingUpdatesRef.current };
      pendingUpdatesRef.current = {};

      const updateData: UpdateTripCommand = {
        name: currentUpdates.name ?? trip.name,
        locationId: currentUpdates.locationId !== undefined ? currentUpdates.locationId : trip.locationId,
        dailyHours: currentUpdates.dailyHours ?? trip.dailyHours,
        maxExtensionHours: currentUpdates.maxExtensionHours ?? trip.maxExtensionHours,
        startTime: currentUpdates.startTime ?? trip.startTime,
      };

      const updatedTrip = await updateTrip(tripId, updateData);
      setTrip(updatedTrip);
      setSaveStatus('saved');
      clearSavedStatus();
    } catch (err) {
      setSaveStatus('error');
      setError(parseTripError(err));
    } finally {
      setIsSaving(false);
    }
  }, [trip, tripId, clearSavedStatus]);

  // Debounced save
  const debouncedSave = useMemo(
    () => debounce(saveTrip, DEBOUNCE_DELAY),
    [saveTrip]
  );

  // Clean up on unmount
  useEffect(() => {
    return () => {
      debouncedSave.cancel();
      if (savedStatusTimeoutRef.current) {
        clearTimeout(savedStatusTimeoutRef.current);
      }
    };
  }, [debouncedSave]);

  // Flatten attractions from days structure
  const flattenAttractions = useCallback((response: { days: Array<{ attractions: TripAttractionItemDTO[] }> }): TripAttractionItemDTO[] => {
    return response.days.flatMap((day) => day.attractions);
  }, []);

  // Load trip and attractions
  const loadData = useCallback(async () => {
    setIsLoading(true);
    setError(null);

    try {
      const [tripData, attractionsData] = await Promise.all([
        fetchTrip(tripId),
        fetchTripAttractions(tripId),
      ]);

      setTrip(tripData);
      setAttractions(flattenAttractions(attractionsData));
    } catch (err) {
      setError(parseTripError(err));
      setTrip(null);
      setAttractions([]);
    } finally {
      setIsLoading(false);
    }
  }, [tripId, flattenAttractions]);

  // Initial load
  useEffect(() => {
    loadData();
  }, [loadData]);

  // Update trip data (triggers autosave)
  const updateTripData = useCallback(
    (updates: Partial<UpdateTripCommand>) => {
      // Update local state immediately
      setTrip((prev) => {
        if (!prev) return prev;
        return { ...prev, ...updates };
      });

      // Queue updates for save
      pendingUpdatesRef.current = {
        ...pendingUpdatesRef.current,
        ...updates,
      };

      // Trigger debounced save
      setSaveStatus('saving');
      debouncedSave();
    },
    [debouncedSave]
  );

  // Move attraction up in order
  const moveAttractionUp = useCallback(
    async (attractionId: UUID) => {
      const index = attractions.findIndex((a) => a.attractionId === attractionId);
      if (index <= 0) return;

      // Optimistic update
      const newAttractions = [...attractions];
      [newAttractions[index - 1], newAttractions[index]] = [
        newAttractions[index],
        newAttractions[index - 1],
      ];

      // Update order indices
      const reorderedAttractions = newAttractions.map((a, i) => ({
        ...a,
        orderIndex: i,
      }));

      setAttractions(reorderedAttractions);

      // Save to API
      try {
        await reorderTripAttractions(tripId, {
          attractions: reorderedAttractions.map((a) => ({
            attractionId: a.attractionId,
            dayNumber: a.dayNumber,
            orderIndex: a.orderIndex,
          })),
        });
      } catch (err) {
        // Revert on error
        setAttractions(attractions);
        setError(parseTripError(err));
      }
    },
    [attractions, tripId]
  );

  // Move attraction down in order
  const moveAttractionDown = useCallback(
    async (attractionId: UUID) => {
      const index = attractions.findIndex((a) => a.attractionId === attractionId);
      if (index < 0 || index >= attractions.length - 1) return;

      // Optimistic update
      const newAttractions = [...attractions];
      [newAttractions[index], newAttractions[index + 1]] = [
        newAttractions[index + 1],
        newAttractions[index],
      ];

      // Update order indices
      const reorderedAttractions = newAttractions.map((a, i) => ({
        ...a,
        orderIndex: i,
      }));

      setAttractions(reorderedAttractions);

      // Save to API
      try {
        await reorderTripAttractions(tripId, {
          attractions: reorderedAttractions.map((a) => ({
            attractionId: a.attractionId,
            dayNumber: a.dayNumber,
            orderIndex: a.orderIndex,
          })),
        });
      } catch (err) {
        // Revert on error
        setAttractions(attractions);
        setError(parseTripError(err));
      }
    },
    [attractions, tripId]
  );

  // Remove attraction from trip
  const removeAttraction = useCallback(
    async (attractionId: UUID) => {
      // Optimistic update
      const previousAttractions = [...attractions];
      setAttractions((prev) => prev.filter((a) => a.attractionId !== attractionId));

      try {
        await removeTripAttraction(tripId, attractionId);
      } catch (err) {
        // Revert on error
        setAttractions(previousAttractions);
        throw new Error(parseTripError(err));
      }
    },
    [attractions, tripId]
  );

  // Delete the entire trip
  const deleteTrip = useCallback(async () => {
    try {
      await deleteTripApi(tripId);
    } catch (err) {
      throw new Error(parseTripError(err));
    }
  }, [tripId]);

  // Refresh data
  const refresh = useCallback(() => {
    loadData();
  }, [loadData]);

  return {
    trip,
    attractions,
    isLoading,
    isSaving,
    saveStatus,
    error,
    updateTripData,
    moveAttractionUp,
    moveAttractionDown,
    removeAttraction,
    deleteTrip,
    refresh,
  };
}
