import { useAttractions } from '../hooks/useAttractions';
import { useAuth } from '../hooks/useAuth';
import { LocationFilter } from '../components/attractions/LocationFilter';
import { AttractionList } from '../components/attractions/AttractionList';
import { CreateAttractionButton } from '../components/attractions/CreateAttractionButton';
import { Pagination } from '../components/common/Pagination';
import { ErrorState } from '../components/common/ErrorState';

export function AttractionsPage() {
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

  const handleClearFilters = () => {
    setLocationFilter(null);
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Header */}
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900 mb-2">
            Atrakcje turystyczne
          </h1>
          <p className="text-gray-600">
            Odkryj najlepsze miejsca do zwiedzania
          </p>
        </div>

        {/* Filters and actions bar */}
        <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 mb-6">
          <LocationFilter
            locations={locations}
            selectedLocationId={selectedLocationId}
            onLocationChange={setLocationFilter}
            isLoading={isLoadingLocations}
          />

          {isAuthenticated && <CreateAttractionButton />}
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
