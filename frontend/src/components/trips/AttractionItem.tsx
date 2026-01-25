import type { TripAttractionItemProps } from '../../@types';

function formatDuration(minutes: number | null): string {
  if (minutes === null) return 'Brak danych';
  if (minutes < 60) return `${minutes} min`;
  const hours = Math.floor(minutes / 60);
  const remainingMinutes = minutes % 60;
  if (remainingMinutes === 0) return `${hours} godz.`;
  return `${hours} godz. ${remainingMinutes} min`;
}

/**
 * Single attraction item in trip's attraction list
 * Includes reorder buttons and remove functionality
 */
export function AttractionItem({
  attraction,
  isFirst,
  isLast,
  onMoveUp,
  onMoveDown,
  onRemove,
  disabled = false,
}: TripAttractionItemProps) {
  const { attraction: attractionData, dayNumber, plannedStartTime } = attraction;

  const placeholderImage =
    'data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" width="80" height="80" viewBox="0 0 80 80"%3E%3Crect fill="%23e5e7eb" width="80" height="80"/%3E%3Ctext fill="%239ca3af" font-family="sans-serif" font-size="10" x="50%25" y="50%25" text-anchor="middle" dy=".3em"%3EBrak%3C/text%3E%3C/svg%3E';

  return (
    <div className="flex items-center gap-4 p-4 bg-white rounded-lg border border-gray-200 hover:border-gray-300 transition-colors">
      {/* Thumbnail */}
      <div className="flex-shrink-0">
        <img
          src={attractionData.imageUrl ?? placeholderImage}
          alt=""
          className="w-16 h-16 rounded-md object-cover"
          onError={(e) => {
            const target = e.target as HTMLImageElement;
            target.src = placeholderImage;
          }}
        />
      </div>

      {/* Content */}
      <div className="flex-1 min-w-0">
        <h4 className="text-sm font-medium text-gray-900 truncate">
          {attractionData.name}
        </h4>
        <div className="flex items-center gap-3 mt-1 text-xs text-gray-500">
          {/* Duration */}
          <span className="flex items-center gap-1">
            <svg
              className="w-3.5 h-3.5"
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

          {/* Day number */}
          <span className="flex items-center gap-1">
            <svg
              className="w-3.5 h-3.5"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
              aria-hidden="true"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={2}
                d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"
              />
            </svg>
            Dzień {dayNumber}
          </span>

          {/* Planned time */}
          {plannedStartTime && (
            <span className="text-blue-600">
              {plannedStartTime.slice(0, 5)}
            </span>
          )}
        </div>

        {/* Rating */}
        {attractionData.rating !== null && (
          <div className="flex items-center gap-1 mt-1">
            <svg
              className="w-3.5 h-3.5 text-yellow-400"
              fill="currentColor"
              viewBox="0 0 20 20"
              aria-hidden="true"
            >
              <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
            </svg>
            <span className="text-xs text-gray-600">
              {attractionData.rating.toFixed(1)}
            </span>
          </div>
        )}
      </div>

      {/* Reorder buttons */}
      <div className="flex flex-col gap-1">
        <button
          type="button"
          onClick={onMoveUp}
          disabled={disabled || isFirst}
          className="
            p-1.5 rounded-md text-gray-400
            hover:text-gray-600 hover:bg-gray-100
            disabled:opacity-30 disabled:cursor-not-allowed disabled:hover:bg-transparent disabled:hover:text-gray-400
            focus:outline-none focus:ring-2 focus:ring-blue-500
            transition-colors
          "
          aria-label="Przenieś w górę"
          title={isFirst ? 'Już na początku listy' : 'Przenieś w górę'}
        >
          <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 15l7-7 7 7" />
          </svg>
        </button>
        <button
          type="button"
          onClick={onMoveDown}
          disabled={disabled || isLast}
          className="
            p-1.5 rounded-md text-gray-400
            hover:text-gray-600 hover:bg-gray-100
            disabled:opacity-30 disabled:cursor-not-allowed disabled:hover:bg-transparent disabled:hover:text-gray-400
            focus:outline-none focus:ring-2 focus:ring-blue-500
            transition-colors
          "
          aria-label="Przenieś w dół"
          title={isLast ? 'Już na końcu listy' : 'Przenieś w dół'}
        >
          <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" />
          </svg>
        </button>
      </div>

      {/* Remove button */}
      <button
        type="button"
        onClick={onRemove}
        disabled={disabled}
        className="
          p-2 rounded-md text-gray-400
          hover:text-red-600 hover:bg-red-50
          disabled:opacity-50 disabled:cursor-not-allowed
          focus:outline-none focus:ring-2 focus:ring-red-500
          transition-colors
        "
        aria-label="Usuń atrakcję z wycieczki"
        title="Usuń z wycieczki"
      >
        <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path
            strokeLinecap="round"
            strokeLinejoin="round"
            strokeWidth={2}
            d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"
          />
        </svg>
      </button>
    </div>
  );
}
