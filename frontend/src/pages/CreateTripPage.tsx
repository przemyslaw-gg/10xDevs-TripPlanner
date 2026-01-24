import { useState, useEffect } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import type { LocationListItemDTO, TripFormData } from '../@types';
import { TripForm } from '../components/trips/TripForm';
import { createTrip, parseTripError } from '../api/trips';
import { fetchLocations } from '../api/locations';

export function CreateTripPage() {
  const navigate = useNavigate();
  const [locations, setLocations] = useState<LocationListItemDTO[]>([]);
  const [isLoadingLocations, setIsLoadingLocations] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);

  // Load locations on mount
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
        console.error('Failed to fetch locations:', err);
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

  const handleSubmit = async (data: TripFormData) => {
    setIsSubmitting(true);

    try {
      const trip = await createTrip({
        name: data.name,
        locationId: data.locationId,
        dailyHours: data.dailyHours,
        maxExtensionHours: data.maxExtensionHours,
        startTime: data.startTime,
      });

      // Redirect to the new trip details page
      navigate(`/trips/${trip.id}`, { replace: true });
    } catch (error) {
      setIsSubmitting(false);
      throw new Error(parseTripError(error));
    }
  };

  const handleCancel = () => {
    navigate('/trips');
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="max-w-2xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Breadcrumb */}
        <nav className="mb-6" aria-label="Breadcrumb">
          <ol className="flex items-center gap-2 text-sm text-gray-500">
            <li>
              <Link to="/trips" className="hover:text-gray-700">
                Moje wycieczki
              </Link>
            </li>
            <li aria-hidden="true">/</li>
            <li className="text-gray-900 font-medium">Nowa wycieczka</li>
          </ol>
        </nav>

        {/* Header */}
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900 mb-2">
            Utwórz nową wycieczkę
          </h1>
          <p className="text-gray-600">
            Wypełnij podstawowe informacje o swojej wycieczce
          </p>
        </div>

        {/* Form Card */}
        <div className="bg-white shadow-md rounded-lg p-6">
          <TripForm
            locations={locations}
            isLoadingLocations={isLoadingLocations}
            onSubmit={handleSubmit}
            onCancel={handleCancel}
            submitLabel="Utwórz wycieczkę"
            isSubmitting={isSubmitting}
          />
        </div>
      </div>
    </div>
  );
}
