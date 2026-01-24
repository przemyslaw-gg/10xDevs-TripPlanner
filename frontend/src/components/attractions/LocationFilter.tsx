import type { LocationListItemDTO, UUID } from '../../@types';

interface LocationFilterProps {
  locations: LocationListItemDTO[];
  selectedLocationId: UUID | null;
  onLocationChange: (locationId: UUID | null) => void;
  isLoading?: boolean;
}

export function LocationFilter({
  locations,
  selectedLocationId,
  onLocationChange,
  isLoading = false,
}: LocationFilterProps) {
  const handleChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    const value = e.target.value;
    onLocationChange(value === '' ? null : value);
  };

  return (
    <div className="flex flex-col sm:flex-row sm:items-center gap-2">
      <label
        htmlFor="location-filter"
        className="text-sm font-medium text-gray-700"
      >
        Lokalizacja:
      </label>
      <select
        id="location-filter"
        value={selectedLocationId ?? ''}
        onChange={handleChange}
        disabled={isLoading}
        className="block w-full sm:w-64 px-3 py-2 bg-white border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-sm disabled:opacity-50 disabled:cursor-not-allowed"
      >
        <option value="">Wszystkie lokalizacje</option>
        {locations.map((location) => (
          <option key={location.id} value={location.id}>
            {location.name}, {location.country}
          </option>
        ))}
      </select>
      {isLoading && (
        <span className="text-sm text-gray-500">Ładowanie...</span>
      )}
    </div>
  );
}
