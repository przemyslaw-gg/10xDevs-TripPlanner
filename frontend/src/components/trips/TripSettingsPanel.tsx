import type { TripSettingsPanelProps } from '../../@types';

/**
 * Panel with editable trip settings
 * Changes trigger autosave via onChange callback
 */
export function TripSettingsPanel({
  trip,
  locations,
  onChange,
  disabled = false,
}: TripSettingsPanelProps) {
  const handleLocationChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    const value = e.target.value;
    onChange({ locationId: value === '' ? null : value });
  };

  const handleStartTimeChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    onChange({ startTime: e.target.value });
  };

  const handleDailyHoursChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = parseInt(e.target.value, 10);
    if (!isNaN(value) && value >= 1 && value <= 24) {
      onChange({ dailyHours: value });
    }
  };

  const handleMaxExtensionChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = parseInt(e.target.value, 10);
    if (!isNaN(value) && value >= 0 && value <= 8) {
      onChange({ maxExtensionHours: value });
    }
  };

  return (
    <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
      <h2 className="text-lg font-semibold text-gray-900 mb-4">
        Ustawienia wycieczki
      </h2>

      <div className="grid gap-4 sm:grid-cols-2">
        {/* Location */}
        <div className="sm:col-span-2">
          <label
            htmlFor="settings-location"
            className="block text-sm font-medium text-gray-700 mb-1"
          >
            Lokalizacja
          </label>
          <select
            id="settings-location"
            value={trip.locationId ?? ''}
            onChange={handleLocationChange}
            disabled={disabled}
            className="
              block w-full px-3 py-2 rounded-md shadow-sm border border-gray-300
              focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500
              disabled:bg-gray-100 disabled:cursor-not-allowed
            "
          >
            <option value="">-- Brak lokalizacji --</option>
            {locations.map((location) => (
              <option key={location.id} value={location.id}>
                {location.name}, {location.country}
              </option>
            ))}
          </select>
        </div>

        {/* Start time */}
        <div>
          <label
            htmlFor="settings-startTime"
            className="block text-sm font-medium text-gray-700 mb-1"
          >
            Godzina rozpoczęcia
          </label>
          <input
            type="time"
            id="settings-startTime"
            value={trip.startTime.slice(0, 5)} // Convert HH:mm:ss to HH:mm
            onChange={handleStartTimeChange}
            disabled={disabled}
            className="
              block w-full px-3 py-2 rounded-md shadow-sm border border-gray-300
              focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500
              disabled:bg-gray-100 disabled:cursor-not-allowed
            "
          />
        </div>

        {/* Daily hours */}
        <div>
          <label
            htmlFor="settings-dailyHours"
            className="block text-sm font-medium text-gray-700 mb-1"
          >
            Godziny dziennie
          </label>
          <input
            type="number"
            id="settings-dailyHours"
            value={trip.dailyHours}
            onChange={handleDailyHoursChange}
            min={1}
            max={24}
            disabled={disabled}
            className="
              block w-full px-3 py-2 rounded-md shadow-sm border border-gray-300
              focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500
              disabled:bg-gray-100 disabled:cursor-not-allowed
            "
          />
          <p className="text-xs text-gray-500 mt-1">1-24 godzin</p>
        </div>

        {/* Max extension hours */}
        <div>
          <label
            htmlFor="settings-maxExtension"
            className="block text-sm font-medium text-gray-700 mb-1"
          >
            Maks. wydłużenie dnia
          </label>
          <input
            type="number"
            id="settings-maxExtension"
            value={trip.maxExtensionHours}
            onChange={handleMaxExtensionChange}
            min={0}
            max={8}
            disabled={disabled}
            className="
              block w-full px-3 py-2 rounded-md shadow-sm border border-gray-300
              focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500
              disabled:bg-gray-100 disabled:cursor-not-allowed
            "
          />
          <p className="text-xs text-gray-500 mt-1">0-8 godzin</p>
        </div>
      </div>
    </div>
  );
}
