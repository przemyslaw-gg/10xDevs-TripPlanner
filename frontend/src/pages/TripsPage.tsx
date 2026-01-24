import { useTrips } from '../hooks/useTrips';
import { TripList } from '../components/trips/TripList';
import { CreateTripButton } from '../components/trips/CreateTripButton';
import { Pagination } from '../components/common/Pagination';
import { ErrorState } from '../components/common/ErrorState';

export function TripsPage() {
  const {
    trips,
    pagination,
    isLoading,
    error,
    currentPage,
    setPage,
    refresh,
  } = useTrips();

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Header */}
        <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 mb-8">
          <div>
            <h1 className="text-3xl font-bold text-gray-900 mb-2">
              Moje wycieczki
            </h1>
            <p className="text-gray-600">
              Zarządzaj swoimi planami podróży
            </p>
          </div>

          <CreateTripButton />
        </div>

        {/* Error state */}
        {error && (
          <ErrorState message={error} onRetry={refresh} />
        )}

        {/* Trip list */}
        {!error && (
          <>
            <TripList trips={trips} isLoading={isLoading} />

            {/* Pagination */}
            {pagination && pagination.totalPages > 1 && !isLoading && (
              <div className="mt-8">
                <Pagination
                  currentPage={currentPage}
                  totalPages={pagination.totalPages}
                  onPageChange={setPage}
                />
              </div>
            )}

            {/* Results info */}
            {pagination && !isLoading && trips.length > 0 && (
              <div className="mt-4 text-center text-sm text-gray-500">
                Wyświetlanie {trips.length} z {pagination.totalItems} wycieczek
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
