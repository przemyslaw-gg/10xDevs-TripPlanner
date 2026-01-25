import type { AttractionRadioItemProps } from '../../@types';

function formatDuration(minutes: number | null): string {
  if (minutes === null) return 'Brak danych';
  if (minutes < 60) return `${minutes} min`;
  const hours = Math.floor(minutes / 60);
  const remainingMinutes = minutes % 60;
  if (remainingMinutes === 0) return `${hours} godz.`;
  return `${hours} godz. ${remainingMinutes} min`;
}

/**
 * Single attraction item with radio button for selection
 * Used in SelectStartingAttractionModal
 */
export function AttractionRadioItem({
  attraction,
  isSelected,
  onSelect,
  disabled = false,
}: AttractionRadioItemProps) {
  const { attraction: attractionData } = attraction;

  const placeholderImage =
    'data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" width="48" height="48" viewBox="0 0 48 48"%3E%3Crect fill="%23e5e7eb" width="48" height="48"/%3E%3Ctext fill="%239ca3af" font-family="sans-serif" font-size="8" x="50%25" y="50%25" text-anchor="middle" dy=".3em"%3EBrak%3C/text%3E%3C/svg%3E';

  const inputId = `attraction-radio-${attraction.attractionId}`;

  return (
    <label
      htmlFor={inputId}
      className={`
        flex items-center gap-3 p-3 rounded-lg border cursor-pointer transition-colors
        ${isSelected
          ? 'border-blue-500 bg-blue-50'
          : 'border-gray-200 hover:border-gray-300 hover:bg-gray-50'
        }
        ${disabled ? 'opacity-50 cursor-not-allowed' : ''}
      `}
    >
      {/* Radio button */}
      <input
        type="radio"
        id={inputId}
        name="starting-attraction"
        checked={isSelected}
        onChange={onSelect}
        disabled={disabled}
        className="
          w-4 h-4 text-blue-600 border-gray-300
          focus:ring-blue-500 focus:ring-2
          disabled:opacity-50
        "
      />

      {/* Thumbnail */}
      <div className="flex-shrink-0">
        <img
          src={attractionData.imageUrl ?? placeholderImage}
          alt=""
          className="w-12 h-12 rounded-md object-cover"
          onError={(e) => {
            const target = e.target as HTMLImageElement;
            target.src = placeholderImage;
          }}
        />
      </div>

      {/* Content */}
      <div className="flex-1 min-w-0">
        <p className="text-sm font-medium text-gray-900 truncate">
          {attractionData.name}
        </p>
        <div className="flex items-center gap-3 mt-0.5 text-xs text-gray-500">
          {/* Duration */}
          <span className="flex items-center gap-1">
            <svg
              className="w-3 h-3"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
              aria-hidden="true"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={2}
                d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"
              />
            </svg>
            {formatDuration(attractionData.estimatedDuration)}
          </span>

          {/* Rating */}
          {attractionData.rating !== null && (
            <span className="flex items-center gap-1">
              <svg
                className="w-3 h-3 text-yellow-400"
                fill="currentColor"
                viewBox="0 0 20 20"
                aria-hidden="true"
              >
                <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
              </svg>
              {attractionData.rating.toFixed(1)}
            </span>
          )}
        </div>
      </div>
    </label>
  );
}
