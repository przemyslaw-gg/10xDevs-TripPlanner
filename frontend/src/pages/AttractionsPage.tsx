import { useCallback, useState, useEffect } from 'react';
import { useSearchParams, Link } from 'react-router-dom';
import { useAttractions } from '../hooks/useAttractions';
import { useAuth } from '../hooks/useAuth';
import { LocationFilter } from '../components/attractions/LocationFilter';
import { AttractionList } from '../components/attractions/AttractionList';
import { CreateAttractionButton } from '../components/attractions/CreateAttractionButton';
import { Pagination } from '../components/common/Pagination';
import { ErrorState } from '../components/common/ErrorState';
import { addAttractionToTrip, fetchTrip, fetchTripAttractions, parseTripError } from '../api/trips';
import type { UUID, TripDTO } from '../@types';

export function AttractionsPage() {
  const [searchParams] = useSearchParams();
  const tripId = searchParams.get('tripId') as UUID | null;

  const {
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
  } = useAttractions();

  const { isAuthenticated, currentUserId } = useAuth();

  // Trip info when adding attractions to a trip
  const [trip, setTrip] = useState<TripDTO | null>(null);
  const [tripAttractionCount, setTripAttractionCount] = useState(0);
  const [tripError, setTripError] = useState<string | null>(null);

  // Load trip info when tripId is present
  useEffect(() => {
    if (!tripId) {
      setTrip(null);
      setTripAttractionCount(0);
      return;
    }

    async function loadTrip() {
      try {
        const [tripData, attractionsData] = await Promise.all([
          fetchTrip(tripId!),
          fetchTripAttractions(tripId!),
        ]);
        setTrip(tripData);
        const count = attractionsData.days.reduce(
          (sum, day) => sum + day.attractions.length,
          0
        );
        setTripAttractionCount(count);
        setTripError(null);
      } catch (err) {
        setTripError(parseTripError(err));
        setTrip(null);
      }
    }

    loadTrip();
  }, [tripId]);

  const handleClearFilters = () => {
    setLocationFilter(null);
  };

  // Handle adding attraction to trip
  const handleAddToTrip = useCallback(
    async (attractionId: UUID) => {
      if (!tripId) return;

      try {
        await addAttractionToTrip(tripId, {
          attractionId,
          dayNumber: 1,
          orderIndex: tripAttractionCount,
        });
        setTripAttractionCount((prev) => prev + 1);
      } catch (err) {
        throw new Error(parseTripError(err));
      }
    },
    [tripId, tripAttractionCount]
  );

  const isAddingMode = Boolean(tripId);
  const maxAttractions = 20;
  const isAtLimit = tripAttractionCount >= maxAttractions;

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Header - different when adding to trip */}
        {isAddingMode && trip ? (
          <div className="mb-8">
            {/* Back link */}
            <Link
              to={`/trips/${tripId}`}
              className="inline-flex items-center gap-2 text-sm text-gray-600 hover:text-gray-900 mb-4"
            >
              <svg
                className="w-4 h-4"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth={2}
                  d="M10 19l-7-7m0 0l7-7m-7 7h18"
                />
              </svg>
              Wróć do wycieczki
            </Link>

            <h1 className="text-3xl font-bold text-gray-900 mb-2">
              Dodaj atrakcje do wycieczki
            </h1>
            <p className="text-gray-600">
              Wybierz atrakcje, które chcesz dodać do wycieczki{' '}
              <span className="font-medium text-gray-900">"{trip.name}"</span>
            </p>

            {/* Attraction count indicator */}
            <div className="mt-4 flex items-center gap-4">
              <span
                className={`text-sm ${
                  isAtLimit ? 'text-red-600 font-medium' : 'text-gray-600'
                }`}
              >
                Atrakcje w wycieczce: {tripAttractionCount}/{maxAttractions}
              </span>
              {isAtLimit && (
                <span className="inline-flex items-center gap-1 px-2 py-1 rounded-full text-xs font-medium bg-red-100 text-red-800">
                  <svg
                    className="w-3 h-3"
                    fill="currentColor"
                    viewBox="0 0 20 20"
                  >
                    <path
                      fillRule="evenodd"
                      d="M8.257 3.099c.765-1.36 2.722-1.36 3.486 0l5.58 9.92c.75 1.334-.213 2.98-1.742 2.98H4.42c-1.53 0-2.493-1.646-1.743-2.98l5.58-9.92zM11 13a1 1 0 11-2 0 1 1 0 012 0zm-1-8a1 1 0 00-1 1v3a1 1 0 002 0V6a1 1 0 00-1-1z"
                      clipRule="evenodd"
                    />
                  </svg>
                  Osiągnięto limit
                </span>
              )}
            </div>
          </div>
        ) : isAddingMode && tripError ? (
          <div className="mb-8">
            <Link
              to="/trips"
              className="inline-flex items-center gap-2 text-sm text-gray-600 hover:text-gray-900 mb-4"
            >
              <svg
                className="w-4 h-4"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth={2}
                  d="M10 19l-7-7m0 0l7-7m-7 7h18"
                />
              </svg>
              Wróć do wycieczek
            </Link>
            <div className="bg-red-50 border border-red-200 rounded-lg p-4">
              <p className="text-red-800">{tripError}</p>
            </div>
          </div>
        ) : (
          <div className="mb-8">
            <h1 className="text-3xl font-bold text-gray-900 mb-2">
              Atrakcje turystyczne
            </h1>
            <p className="text-gray-600">
              Odkryj najlepsze miejsca do zwiedzania
            </p>
          </div>
        )}

        {/* Filters and actions bar */}
        <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 mb-6">
          <LocationFilter
            locations={locations}
            selectedLocationId={selectedLocationId}
            onLocationChange={setLocationFilter}
            isLoading={isLoadingLocations}
          />

          {isAuthenticated && !isAddingMode && <CreateAttractionButton />}
        </div>

        {/* Error state */}
        {error && (
          <ErrorState message={error} onRetry={refreshAttractions} />
        )}

        {/* Attractions list */}
        {!error && (
          <>
            <AttractionList
              attractions={attractions}
              isLoading={isLoadingAttractions}
              currentUserId={currentUserId}
              onClearFilters={selectedLocationId ? handleClearFilters : undefined}
              onAddToTrip={
                isAddingMode && trip && !isAtLimit ? handleAddToTrip : undefined
              }
            />

            {/* Pagination */}
            {pagination && pagination.totalPages > 1 && !isLoadingAttractions && (
              <div className="mt-8">
                <Pagination
                  currentPage={currentPage}
                  totalPages={pagination.totalPages}
                  onPageChange={setPage}
                />
              </div>
            )}

            {/* Results info */}
            {pagination && !isLoadingAttractions && attractions.length > 0 && (
              <div className="mt-4 text-center text-sm text-gray-500">
                Wyświetlanie {attractions.length} z {pagination.totalItems} atrakcji
                {pagination.totalPages > 1 && (
                  <span> (strona {currentPage} z {pagination.totalPages})</span>
                )}
              </div>
            )}
          </>
        )}
      </div>
    </div>
  );
}
