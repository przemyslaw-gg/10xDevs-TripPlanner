import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import type { LocationListItemDTO } from '../../@types';

const attractionSchema = z.object({
  name: z
    .string()
    .min(1, 'Nazwa jest wymagana')
    .max(255, 'Nazwa może mieć maksymalnie 255 znaków'),
  description: z
    .string()
    .max(5000, 'Opis może mieć maksymalnie 5000 znaków')
    .optional()
    .nullable(),
  locationId: z.string().uuid('Wybierz lokalizację'),
  latitude: z
    .number({ message: 'Szerokość geograficzna jest wymagana' })
    .min(-90, 'Szerokość musi być między -90 a 90')
    .max(90, 'Szerokość musi być między -90 a 90'),
  longitude: z
    .number({ message: 'Długość geograficzna jest wymagana' })
    .min(-180, 'Długość musi być między -180 a 180')
    .max(180, 'Długość musi być między -180 a 180'),
  estimatedDuration: z
    .number()
    .int('Czas musi być liczbą całkowitą')
    .min(1, 'Czas musi być większy niż 0')
    .optional()
    .nullable(),
  imageUrl: z
    .string()
    .url('Podaj prawidłowy adres URL')
    .optional()
    .nullable()
    .or(z.literal('')),
});

type AttractionFormData = z.infer<typeof attractionSchema>;

interface AttractionFormProps {
  locations: LocationListItemDTO[];
  locationsLoading?: boolean;
  onSubmit: (data: AttractionFormData) => Promise<void>;
  onCancel: () => void;
  isSubmitting?: boolean;
  error?: string | null;
}

export function AttractionForm({
  locations,
  locationsLoading = false,
  onSubmit,
  onCancel,
  isSubmitting = false,
  error,
}: AttractionFormProps) {
  const [imagePreviewUrl, setImagePreviewUrl] = useState<string | null>(null);
  const [imageError, setImageError] = useState(false);

  const {
    register,
    handleSubmit,
    watch,
    formState: { errors },
  } = useForm<AttractionFormData>({
    resolver: zodResolver(attractionSchema),
    defaultValues: {
      name: '',
      description: '',
      locationId: '',
      latitude: undefined,
      longitude: undefined,
      estimatedDuration: undefined,
      imageUrl: '',
    },
  });

  const watchedImageUrl = watch('imageUrl');

  // Update image preview when URL changes
  const handleImageUrlBlur = () => {
    if (watchedImageUrl && watchedImageUrl.trim()) {
      setImagePreviewUrl(watchedImageUrl.trim());
      setImageError(false);
    } else {
      setImagePreviewUrl(null);
    }
  };

  const onFormSubmit = async (data: AttractionFormData) => {
    const cleanedData = {
      ...data,
      description: data.description || null,
      estimatedDuration: data.estimatedDuration || null,
      imageUrl: data.imageUrl?.trim() || null,
    };
    await onSubmit(cleanedData);
  };

  return (
    <form onSubmit={handleSubmit(onFormSubmit)} className="space-y-6">
      {/* Error message */}
      {error && (
        <div className="rounded-md bg-red-50 p-4">
          <div className="flex">
            <svg
              className="h-5 w-5 text-red-400"
              fill="currentColor"
              viewBox="0 0 20 20"
            >
              <path
                fillRule="evenodd"
                d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z"
                clipRule="evenodd"
              />
            </svg>
            <div className="ml-3">
              <p className="text-sm font-medium text-red-800">{error}</p>
            </div>
          </div>
        </div>
      )}

      {/* Name */}
      <div>
        <label htmlFor="name" className="block text-sm font-medium text-gray-700">
          Nazwa atrakcji *
        </label>
        <input
          type="text"
          id="name"
          {...register('name')}
          className={`
            mt-1 block w-full rounded-md shadow-sm sm:text-sm
            ${errors.name
              ? 'border-red-300 focus:border-red-500 focus:ring-red-500'
              : 'border-gray-300 focus:border-blue-500 focus:ring-blue-500'
            }
          `}
          placeholder="np. Wawel"
        />
        {errors.name && (
          <p className="mt-1 text-sm text-red-600">{errors.name.message}</p>
        )}
      </div>

      {/* Description */}
      <div>
        <label htmlFor="description" className="block text-sm font-medium text-gray-700">
          Opis
        </label>
        <textarea
          id="description"
          rows={4}
          {...register('description')}
          className={`
            mt-1 block w-full rounded-md shadow-sm sm:text-sm
            ${errors.description
              ? 'border-red-300 focus:border-red-500 focus:ring-red-500'
              : 'border-gray-300 focus:border-blue-500 focus:ring-blue-500'
            }
          `}
          placeholder="Opisz atrakcję..."
        />
        {errors.description && (
          <p className="mt-1 text-sm text-red-600">{errors.description.message}</p>
        )}
      </div>

      {/* Location */}
      <div>
        <label htmlFor="locationId" className="block text-sm font-medium text-gray-700">
          Lokalizacja *
        </label>
        <select
          id="locationId"
          {...register('locationId')}
          disabled={locationsLoading}
          className={`
            mt-1 block w-full rounded-md shadow-sm sm:text-sm
            ${errors.locationId
              ? 'border-red-300 focus:border-red-500 focus:ring-red-500'
              : 'border-gray-300 focus:border-blue-500 focus:ring-blue-500'
            }
            disabled:bg-gray-100 disabled:cursor-not-allowed
          `}
        >
          <option value="">
            {locationsLoading ? 'Ładowanie...' : 'Wybierz lokalizację'}
          </option>
          {locations.map((location) => (
            <option key={location.id} value={location.id}>
              {location.name}, {location.country}
            </option>
          ))}
        </select>
        {errors.locationId && (
          <p className="mt-1 text-sm text-red-600">{errors.locationId.message}</p>
        )}
      </div>

      {/* Coordinates */}
      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
        <div>
          <label htmlFor="latitude" className="block text-sm font-medium text-gray-700">
            Szerokość geograficzna *
          </label>
          <input
            type="number"
            step="any"
            id="latitude"
            {...register('latitude', { valueAsNumber: true })}
            className={`
              mt-1 block w-full rounded-md shadow-sm sm:text-sm
              ${errors.latitude
                ? 'border-red-300 focus:border-red-500 focus:ring-red-500'
                : 'border-gray-300 focus:border-blue-500 focus:ring-blue-500'
              }
            `}
            placeholder="np. 50.0614"
          />
          {errors.latitude && (
            <p className="mt-1 text-sm text-red-600">{errors.latitude.message}</p>
          )}
        </div>

        <div>
          <label htmlFor="longitude" className="block text-sm font-medium text-gray-700">
            Długość geograficzna *
          </label>
          <input
            type="number"
            step="any"
            id="longitude"
            {...register('longitude', { valueAsNumber: true })}
            className={`
              mt-1 block w-full rounded-md shadow-sm sm:text-sm
              ${errors.longitude
                ? 'border-red-300 focus:border-red-500 focus:ring-red-500'
                : 'border-gray-300 focus:border-blue-500 focus:ring-blue-500'
              }
            `}
            placeholder="np. 19.9366"
          />
          {errors.longitude && (
            <p className="mt-1 text-sm text-red-600">{errors.longitude.message}</p>
          )}
        </div>
      </div>

      {/* Estimated duration */}
      <div>
        <label htmlFor="estimatedDuration" className="block text-sm font-medium text-gray-700">
          Szacowany czas zwiedzania (minuty)
        </label>
        <input
          type="number"
          id="estimatedDuration"
          {...register('estimatedDuration', { valueAsNumber: true })}
          className={`
            mt-1 block w-full rounded-md shadow-sm sm:text-sm
            ${errors.estimatedDuration
              ? 'border-red-300 focus:border-red-500 focus:ring-red-500'
              : 'border-gray-300 focus:border-blue-500 focus:ring-blue-500'
            }
          `}
          placeholder="np. 120"
        />
        {errors.estimatedDuration && (
          <p className="mt-1 text-sm text-red-600">{errors.estimatedDuration.message}</p>
        )}
      </div>

      {/* Image URL */}
      <div>
        <label htmlFor="imageUrl" className="block text-sm font-medium text-gray-700">
          URL zdjęcia
        </label>
        <input
          type="url"
          id="imageUrl"
          {...register('imageUrl')}
          onBlur={handleImageUrlBlur}
          className={`
            mt-1 block w-full rounded-md shadow-sm sm:text-sm
            ${errors.imageUrl
              ? 'border-red-300 focus:border-red-500 focus:ring-red-500'
              : 'border-gray-300 focus:border-blue-500 focus:ring-blue-500'
            }
          `}
          placeholder="https://example.com/image.jpg"
        />
        {errors.imageUrl && (
          <p className="mt-1 text-sm text-red-600">{errors.imageUrl.message}</p>
        )}

        {/* Image preview */}
        {imagePreviewUrl && !imageError && (
          <div className="mt-3">
            <p className="text-sm text-gray-500 mb-2">Podgląd:</p>
            <img
              src={imagePreviewUrl}
              alt="Podgląd zdjęcia"
              className="max-w-xs max-h-48 rounded-lg border border-gray-200 object-cover"
              onError={() => setImageError(true)}
            />
          </div>
        )}
        {imageError && imagePreviewUrl && (
          <p className="mt-2 text-sm text-yellow-600">
            Nie udało się załadować podglądu zdjęcia
          </p>
        )}
      </div>

      {/* Form actions */}
      <div className="flex items-center justify-end gap-3 pt-4 border-t border-gray-200">
        <button
          type="button"
          onClick={onCancel}
          disabled={isSubmitting}
          className="
            px-4 py-2 text-sm font-medium text-gray-700
            bg-white border border-gray-300 rounded-md shadow-sm
            hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500
            disabled:opacity-50 disabled:cursor-not-allowed
          "
        >
          Anuluj
        </button>
        <button
          type="submit"
          disabled={isSubmitting}
          className="
            inline-flex items-center px-4 py-2 text-sm font-medium text-white
            bg-blue-600 border border-transparent rounded-md shadow-sm
            hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500
            disabled:opacity-50 disabled:cursor-not-allowed
          "
        >
          {isSubmitting ? (
            <>
              <svg
                className="animate-spin -ml-1 mr-2 h-4 w-4 text-white"
                fill="none"
                viewBox="0 0 24 24"
              >
                <circle
                  className="opacity-25"
                  cx="12"
                  cy="12"
                  r="10"
                  stroke="currentColor"
                  strokeWidth="4"
                />
                <path
                  className="opacity-75"
                  fill="currentColor"
                  d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
                />
              </svg>
              Tworzenie...
            </>
          ) : (
            'Utwórz atrakcję'
          )}
        </button>
      </div>
    </form>
  );
}
