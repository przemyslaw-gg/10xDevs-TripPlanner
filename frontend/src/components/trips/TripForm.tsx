import { useState, useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import type { TripFormProps, TripFormData } from '../../@types';
import { CharacterCounter } from './CharacterCounter';

const tripFormSchema = z.object({
  name: z
    .string()
    .min(1, 'Nazwa wycieczki jest wymagana')
    .max(100, 'Nazwa może mieć maksymalnie 100 znaków'),
  locationId: z
    .string()
    .uuid('Nieprawidłowa lokalizacja')
    .nullable()
    .optional()
    .or(z.literal('')),
  startTime: z
    .string()
    .min(1, 'Godzina rozpoczęcia jest wymagana')
    .regex(/^([01]?[0-9]|2[0-3]):[0-5][0-9]$/, 'Nieprawidłowy format godziny (HH:mm)'),
  dailyHours: z
    .number({ message: 'Musi być liczbą' })
    .int('Musi być liczbą całkowitą')
    .min(1, 'Minimum 1 godzina')
    .max(24, 'Maksimum 24 godziny'),
  maxExtensionHours: z
    .number({ message: 'Musi być liczbą' })
    .int('Musi być liczbą całkowitą')
    .min(0, 'Minimum 0 godzin')
    .max(8, 'Maksimum 8 godzin'),
});

type FormSchema = z.infer<typeof tripFormSchema>;

const defaultValues: FormSchema = {
  name: '',
  locationId: '',
  startTime: '09:00',
  dailyHours: 8,
  maxExtensionHours: 2,
};

/**
 * Form for creating/editing a trip
 */
export function TripForm({
  initialData,
  locations,
  isLoadingLocations,
  onSubmit,
  onCancel,
  submitLabel = 'Utwórz',
  isSubmitting = false,
}: TripFormProps) {
  const [apiError, setApiError] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    watch,
    formState: { errors },
    reset,
  } = useForm<FormSchema>({
    resolver: zodResolver(tripFormSchema),
    defaultValues: {
      ...defaultValues,
      ...initialData,
      locationId: initialData?.locationId ?? '',
    },
    mode: 'onBlur',
  });

  // Reset form when initialData changes (for edit mode)
  useEffect(() => {
    if (initialData) {
      reset({
        ...defaultValues,
        ...initialData,
        locationId: initialData.locationId ?? '',
      });
    }
  }, [initialData, reset]);

  const nameValue = watch('name');

  const handleFormSubmit = async (data: FormSchema) => {
    setApiError(null);

    try {
      const formData: TripFormData = {
        name: data.name,
        locationId: data.locationId && data.locationId !== '' ? data.locationId : null,
        dailyHours: data.dailyHours,
        maxExtensionHours: data.maxExtensionHours,
        startTime: data.startTime,
      };
      await onSubmit(formData);
    } catch (error) {
      if (error instanceof Error) {
        setApiError(error.message);
      } else {
        setApiError('Wystąpił nieoczekiwany błąd');
      }
    }
  };

  return (
    <form onSubmit={handleSubmit(handleFormSubmit)} className="space-y-6">
      {/* API Error Alert */}
      {apiError && (
        <div
          className="p-4 rounded-md bg-red-50 border border-red-200"
          role="alert"
        >
          <p className="text-sm text-red-700">{apiError}</p>
        </div>
      )}

      {/* Name Field */}
      <div className="space-y-1">
        <div className="flex items-center justify-between">
          <label
            htmlFor="name"
            className="block text-sm font-medium text-gray-700"
          >
            Nazwa wycieczki <span className="text-red-500">*</span>
          </label>
          <CharacterCounter current={nameValue?.length ?? 0} max={100} />
        </div>
        <input
          type="text"
          id="name"
          autoFocus
          maxLength={100}
          {...register('name')}
          aria-invalid={!!errors.name}
          aria-describedby={errors.name ? 'name-error' : undefined}
          disabled={isSubmitting}
          className={`
            block w-full px-3 py-2 rounded-md shadow-sm border
            focus:outline-none focus:ring-2 focus:ring-offset-0
            disabled:bg-gray-100 disabled:cursor-not-allowed
            ${errors.name
              ? 'border-red-500 focus:border-red-500 focus:ring-red-500'
              : 'border-gray-300 focus:border-blue-500 focus:ring-blue-500'
            }
          `}
          placeholder="np. Ateny 2026"
        />
        {errors.name && (
          <p id="name-error" className="text-sm text-red-600" role="alert">
            {errors.name.message}
          </p>
        )}
      </div>

      {/* Location Field */}
      <div className="space-y-1">
        <label
          htmlFor="locationId"
          className="block text-sm font-medium text-gray-700"
        >
          Lokalizacja
        </label>
        <select
          id="locationId"
          {...register('locationId')}
          disabled={isSubmitting || isLoadingLocations}
          className={`
            block w-full px-3 py-2 rounded-md shadow-sm border
            focus:outline-none focus:ring-2 focus:ring-offset-0
            disabled:bg-gray-100 disabled:cursor-not-allowed
            border-gray-300 focus:border-blue-500 focus:ring-blue-500
          `}
        >
          <option value="">-- Wybierz lokalizację (opcjonalnie) --</option>
          {locations.map((location) => (
            <option key={location.id} value={location.id}>
              {location.name}, {location.country}
            </option>
          ))}
        </select>
        {isLoadingLocations && (
          <p className="text-xs text-gray-500">Ładowanie lokalizacji...</p>
        )}
      </div>

      {/* Start Time Field */}
      <div className="space-y-1">
        <label
          htmlFor="startTime"
          className="block text-sm font-medium text-gray-700"
        >
          Godzina rozpoczęcia dnia <span className="text-red-500">*</span>
        </label>
        <input
          type="time"
          id="startTime"
          {...register('startTime')}
          aria-invalid={!!errors.startTime}
          aria-describedby={errors.startTime ? 'startTime-error' : undefined}
          disabled={isSubmitting}
          className={`
            block w-full px-3 py-2 rounded-md shadow-sm border
            focus:outline-none focus:ring-2 focus:ring-offset-0
            disabled:bg-gray-100 disabled:cursor-not-allowed
            ${errors.startTime
              ? 'border-red-500 focus:border-red-500 focus:ring-red-500'
              : 'border-gray-300 focus:border-blue-500 focus:ring-blue-500'
            }
          `}
        />
        {errors.startTime && (
          <p id="startTime-error" className="text-sm text-red-600" role="alert">
            {errors.startTime.message}
          </p>
        )}
      </div>

      {/* Daily Hours Field */}
      <div className="space-y-1">
        <label
          htmlFor="dailyHours"
          className="block text-sm font-medium text-gray-700"
        >
          Godziny zwiedzania dziennie <span className="text-red-500">*</span>
        </label>
        <input
          type="number"
          id="dailyHours"
          min={1}
          max={24}
          {...register('dailyHours', { valueAsNumber: true })}
          aria-invalid={!!errors.dailyHours}
          aria-describedby={errors.dailyHours ? 'dailyHours-error' : undefined}
          disabled={isSubmitting}
          className={`
            block w-full px-3 py-2 rounded-md shadow-sm border
            focus:outline-none focus:ring-2 focus:ring-offset-0
            disabled:bg-gray-100 disabled:cursor-not-allowed
            ${errors.dailyHours
              ? 'border-red-500 focus:border-red-500 focus:ring-red-500'
              : 'border-gray-300 focus:border-blue-500 focus:ring-blue-500'
            }
          `}
        />
        {errors.dailyHours && (
          <p id="dailyHours-error" className="text-sm text-red-600" role="alert">
            {errors.dailyHours.message}
          </p>
        )}
        <p className="text-xs text-gray-500">
          Ile godzin dziennie planujesz zwiedzać (1-24)
        </p>
      </div>

      {/* Max Extension Hours Field */}
      <div className="space-y-1">
        <label
          htmlFor="maxExtensionHours"
          className="block text-sm font-medium text-gray-700"
        >
          Maksymalne wydłużenie dnia <span className="text-red-500">*</span>
        </label>
        <input
          type="number"
          id="maxExtensionHours"
          min={0}
          max={8}
          {...register('maxExtensionHours', { valueAsNumber: true })}
          aria-invalid={!!errors.maxExtensionHours}
          aria-describedby={errors.maxExtensionHours ? 'maxExtensionHours-error' : undefined}
          disabled={isSubmitting}
          className={`
            block w-full px-3 py-2 rounded-md shadow-sm border
            focus:outline-none focus:ring-2 focus:ring-offset-0
            disabled:bg-gray-100 disabled:cursor-not-allowed
            ${errors.maxExtensionHours
              ? 'border-red-500 focus:border-red-500 focus:ring-red-500'
              : 'border-gray-300 focus:border-blue-500 focus:ring-blue-500'
            }
          `}
        />
        {errors.maxExtensionHours && (
          <p id="maxExtensionHours-error" className="text-sm text-red-600" role="alert">
            {errors.maxExtensionHours.message}
          </p>
        )}
        <p className="text-xs text-gray-500">
          O ile godzin można wydłużyć dzień w razie potrzeby (0-8)
        </p>
      </div>

      {/* Form Actions */}
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
            transition-colors duration-150
          "
        >
          Anuluj
        </button>
        <button
          type="submit"
          disabled={isSubmitting}
          className="
            px-4 py-2 text-sm font-medium text-white
            bg-blue-600 border border-transparent rounded-md shadow-sm
            hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500
            disabled:opacity-50 disabled:cursor-not-allowed
            transition-colors duration-150
          "
        >
          {isSubmitting ? (
            <span className="flex items-center gap-2">
              <svg
                className="animate-spin h-4 w-4"
                xmlns="http://www.w3.org/2000/svg"
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
              Zapisywanie...
            </span>
          ) : (
            submitLabel
          )}
        </button>
      </div>
    </form>
  );
}
