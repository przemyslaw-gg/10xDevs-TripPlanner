import { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import type { UUID, LocationListItemDTO } from '../@types';
import { useTripDetails } from '../hooks/useTripDetails';
import { fetchLocations } from '../api/locations';
import { TripHeader } from '../components/trips/TripHeader';
import { TripSettingsPanel } from '../components/trips/TripSettingsPanel';
import { TripAttractionList } from '../components/trips/TripAttractionList';
import { AddAttractionButton } from '../components/trips/AddAttractionButton';
import { DeleteTripButton } from '../components/trips/DeleteTripButton';
import { ConfirmDeleteModal } from '../components/trips/ConfirmDeleteModal';

const MAX_ATTRACTIONS = 20;

/**
 * Trip details page with inline editing and autosave
 * URL: /trips/:id
 */
export function TripDetailsPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const tripId = id as UUID;

  // Trip details hook
  const {
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
  } = useTripDetails(tripId);

  // Locations for settings panel
  const [locations, setLocations] = useState<LocationListItemDTO[]>([]);
  const [locationsLoading, setLocationsLoading] = useState(true);

  // Delete modal state
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [isDeleting, setIsDeleting] = useState(false);
  const [deleteError, setDeleteError] = useState<string | null>(null);

  // Load locations
  useEffect(() => {
    async function loadLocations() {
      try {
        const response = await fetchLocations({ pageSize: 100 });
        setLocations(response.items);
      } catch (err) {
        console.error('Failed to load locations:', err);
      } finally {
        setLocationsLoading(false);
      }
    }

    loadLocations();
  }, []);

  // Handle trip deletion
  const handleDeleteClick = () => {
    setShowDeleteModal(true);
    setDeleteError(null);
  };

  const handleDeleteConfirm = async () => {
    setIsDeleting(true);
    setDeleteError(null);

    try {
      await deleteTrip();
      navigate('/trips', { replace: true });
    } catch (err) {
      setDeleteError(err instanceof Error ? err.message : 'Nie udało się usunąć wycieczki');
      setIsDeleting(false);
    }
  };

  const handleDeleteCancel = () => {
    if (!isDeleting) {
      setShowDeleteModal(false);
      setDeleteError(null);
    }
  };

  // Handle attraction removal
  const handleRemoveAttraction = async (attractionId: UUID) => {
    try {
      await removeAttraction(attractionId);
    } catch (err) {
      console.error('Failed to remove attraction:', err);
    }
  };

  // Loading state
  if (isLoading) {
    return (
      <div className="min-h-screen bg-gray-50">
        {/* Header skeleton */}
        <div className="bg-white shadow-sm border-b border-gray-200">
          <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
            <div className="animate-pulse">
              <div className="h-4 bg-gray-200 rounded w-32 mb-4" />
              <div className="h-8 bg-gray-200 rounded w-64 mb-2" />
              <div className="h-4 bg-gray-200 rounded w-48" />
            </div>
          </div>
        </div>

        {/* Content skeleton */}
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
          <div className="grid gap-8 lg:grid-cols-3">
            {/* Settings skeleton */}
            <div className="lg:col-span-1">
              <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6 animate-pulse">
                <div className="h-6 bg-gray-200 rounded w-40 mb-4" />
                <div className="space-y-4">
                  <div className="h-10 bg-gray-200 rounded" />
                  <div className="h-10 bg-gray-200 rounded" />
                  <div className="h-10 bg-gray-200 rounded" />
                </div>
              </div>
            </div>

            {/* Attractions skeleton */}
            <div className="lg:col-span-2">
              <div className="space-y-3">
                {Array.from({ length: 3 }).map((_, index) => (
                  <div
                    key={index}
                    className="flex items-center gap-4 p-4 bg-white rounded-lg border border-gray-200 animate-pulse"
                  >
                    <div className="w-16 h-16 bg-gray-200 rounded-md" />
                    <div className="flex-1">
                      <div className="h-4 bg-gray-200 rounded w-3/4 mb-2" />
                      <div className="h-3 bg-gray-200 rounded w-1/2" />
                    </div>
                  </div>
                ))}
              </div>
            </div>
          </div>
        </div>
      </div>
    );
  }

  // Error state - trip not found or access denied
  if (error || !trip) {
    const is404 = error?.includes('nie znaleziona') || error?.includes('not found');
    const is403 = error?.includes('brak dostępu') || error?.includes('forbidden') || error?.includes('403');

    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center px-4">
        <div className="max-w-md w-full text-center">
          <div className="mx-auto h-16 w-16 flex items-center justify-center rounded-full bg-red-100 mb-6">
            {is403 ? (
              <svg
                className="h-8 w-8 text-red-600"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
                aria-hidden="true"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth={2}
                  d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z"
                />
              </svg>
            ) : (
              <svg
                className="h-8 w-8 text-red-600"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
                aria-hidden="true"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth={2}
                  d="M9.172 16.172a4 4 0 015.656 0M9 10h.01M15 10h.01M12 2C6.477 2 2 6.477 2 12s4.477 10 10 10 10-4.477 10-10S17.523 2 12 2z"
                />
              </svg>
            )}
          </div>

          <h1 className="text-2xl font-bold text-gray-900 mb-2">
            {is403 ? 'Brak dostępu' : is404 ? 'Wycieczka nie znaleziona' : 'Wystąpił błąd'}
          </h1>
          <p className="text-gray-600 mb-8">
            {is403
              ? 'Nie masz uprawnień do wyświetlenia tej wycieczki.'
              : is404
                ? 'Wycieczka, której szukasz, nie istnieje lub została usunięta.'
                : error || 'Nie udało się załadować wycieczki.'}
          </p>

          <Link
            to="/trips"
            className="
              inline-flex items-center gap-2 px-4 py-2
              bg-blue-600 hover:bg-blue-700
              text-white font-medium rounded-md shadow-sm
              focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500
              transition-colors duration-150
            "
          >
            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M10 19l-7-7m0 0l7-7m-7 7h18" />
            </svg>
            Wróć do listy wycieczek
          </Link>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header with editable title */}
      <TripHeader
        trip={trip}
        onNameChange={(name) => updateTripData({ name })}
        saveStatus={saveStatus}
      />

      {/* Main content */}
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="grid gap-8 lg:grid-cols-3">
          {/* Left column - Settings */}
          <div className="lg:col-span-1 space-y-6">
            {/* Settings panel */}
            <TripSettingsPanel
              trip={trip}
              locations={locations}
              onChange={updateTripData}
              disabled={isSaving || locationsLoading}
            />

            {/* Delete section */}
            <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
              <h2 className="text-lg font-semibold text-gray-900 mb-2">
                Strefa niebezpieczna
              </h2>
              <p className="text-sm text-gray-600 mb-4">
                Usunięcie wycieczki jest nieodwracalne. Wszystkie dane zostaną trwale usunięte.
              </p>
              <DeleteTripButton onClick={handleDeleteClick} disabled={isSaving} />
            </div>
          </div>

          {/* Right column - Attractions */}
          <div className="lg:col-span-2">
            {/* Action bar */}
            <div className="flex items-center justify-between mb-6">
              <h2 className="text-xl font-semibold text-gray-900">
                Zaplanowane atrakcje
              </h2>
              <AddAttractionButton
                tripId={tripId}
                disabled={isSaving}
                currentCount={attractions.length}
                maxCount={MAX_ATTRACTIONS}
              />
            </div>

            {/* Attractions list */}
            <TripAttractionList
              attractions={attractions}
              totalCount={attractions.length}
              maxCount={MAX_ATTRACTIONS}
              onMoveUp={moveAttractionUp}
              onMoveDown={moveAttractionDown}
              onRemove={handleRemoveAttraction}
              disabled={isSaving}
            />
          </div>
        </div>
      </div>

      {/* Delete confirmation modal */}
      <ConfirmDeleteModal
        isOpen={showDeleteModal}
        tripName={trip.name}
        onConfirm={handleDeleteConfirm}
        onCancel={handleDeleteCancel}
        isDeleting={isDeleting}
      />

      {/* Delete error toast */}
      {deleteError && (
        <div className="fixed bottom-4 right-4 max-w-md bg-red-50 border border-red-200 rounded-lg shadow-lg p-4">
          <div className="flex items-start gap-3">
            <svg
              className="h-5 w-5 text-red-600 flex-shrink-0 mt-0.5"
              fill="currentColor"
              viewBox="0 0 20 20"
            >
              <path
                fillRule="evenodd"
                d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z"
                clipRule="evenodd"
              />
            </svg>
            <div>
              <h4 className="text-sm font-medium text-red-800">Błąd usuwania</h4>
              <p className="text-sm text-red-700 mt-1">{deleteError}</p>
            </div>
            <button
              onClick={() => setDeleteError(null)}
              className="ml-auto flex-shrink-0 text-red-500 hover:text-red-700"
            >
              <span className="sr-only">Zamknij</span>
              <svg className="h-5 w-5" fill="currentColor" viewBox="0 0 20 20">
                <path
                  fillRule="evenodd"
                  d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z"
                  clipRule="evenodd"
                />
              </svg>
            </button>
          </div>
        </div>
      )}
    </div>
  );
}
