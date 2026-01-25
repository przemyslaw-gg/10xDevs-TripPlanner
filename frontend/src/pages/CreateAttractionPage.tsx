import { useState, useEffect } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import type { LocationListItemDTO, CreateAttractionCommand } from '../@types';
import { fetchLocations } from '../api/locations';
import { createAttraction, parseAttractionError } from '../api/attractions';
import { AttractionForm } from '../components/attractions/AttractionForm';

/**
 * Page for creating a new user attraction
 * URL: /attractions/new
 */
export function CreateAttractionPage() {
  const navigate = useNavigate();

  const [locations, setLocations] = useState<LocationListItemDTO[]>([]);
  const [locationsLoading, setLocationsLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Load locations on mount
  useEffect(() => {
    async function loadLocations() {
      try {
        const response = await fetchLocations({ pageSize: 100 });
        setLocations(response.items);
      } catch (err) {
        console.error('Failed to load locations:', err);
        setError('Nie udało się załadować listy lokalizacji');
      } finally {
        setLocationsLoading(false);
      }
    }

    loadLocations();
  }, []);

  const handleSubmit = async (data: CreateAttractionCommand) => {
    setIsSubmitting(true);
    setError(null);

    try {
      await createAttraction(data);
      navigate('/attractions', { replace: true });
    } catch (err) {
      setError(parseAttractionError(err));
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleCancel = () => {
    navigate('/attractions');
  };

  return (
    <div className="min-h-screen bg-gray-50 py-8">
      <div className="max-w-2xl mx-auto px-4 sm:px-6 lg:px-8">
        {/* Back link */}
        <Link
          to="/attractions"
          className="inline-flex items-center gap-2 text-sm text-gray-600 hover:text-gray-900 mb-6"
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
          Wróć do listy atrakcji
        </Link>

        {/* Page header */}
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900">
            Dodaj nową atrakcję
          </h1>
          <p className="mt-2 text-gray-600">
            Uzupełnij formularz, aby dodać własną atrakcję turystyczną.
            Pola oznaczone * są wymagane.
          </p>
        </div>

        {/* Form card */}
        <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
          <AttractionForm
            locations={locations}
            locationsLoading={locationsLoading}
            onSubmit={handleSubmit}
            onCancel={handleCancel}
            isSubmitting={isSubmitting}
            error={error}
          />
        </div>

        {/* Info box */}
        <div className="mt-6 bg-blue-50 border border-blue-200 rounded-lg p-4">
          <div className="flex">
            <svg
              className="h-5 w-5 text-blue-400 flex-shrink-0"
              fill="currentColor"
              viewBox="0 0 20 20"
            >
              <path
                fillRule="evenodd"
                d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z"
                clipRule="evenodd"
              />
            </svg>
            <div className="ml-3">
              <h3 className="text-sm font-medium text-blue-800">
                Informacja
              </h3>
              <p className="mt-1 text-sm text-blue-700">
                Twoja atrakcja zostanie oznaczona jako "własna" i będzie widoczna
                na liście atrakcji. Możesz ją później dodać do swoich wycieczek.
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
