import { Link } from 'react-router-dom';
import type { TripCardProps } from '../../@types';

function formatDate(dateString: string): string {
  const date = new Date(dateString);
  return date.toLocaleDateString('pl-PL', {
    day: 'numeric',
    month: 'short',
    year: 'numeric',
  });
}

/**
 * Card displaying trip summary information
 * Entire card is clickable and navigates to trip details
 */
export function TripCard({ trip }: TripCardProps) {
  const {
    id,
    name,
    location,
    attractionCount,
    totalDays,
    isPublic,
    updatedAt,
  } = trip;

  return (
    <article className="bg-white rounded-lg shadow-md hover:shadow-lg transition-shadow duration-200">
      <Link
        to={`/trips/${id}`}
        className="block p-5 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 rounded-lg"
      >
        {/* Header with title and public badge */}
        <div className="flex items-start justify-between gap-2 mb-2">
          <h3 className="text-lg font-semibold text-gray-900 line-clamp-2">
            {name}
          </h3>
          {isPublic && (
            <span className="flex-shrink-0 inline-flex items-center px-2 py-0.5 rounded text-xs font-medium bg-green-100 text-green-800">
              Publiczna
            </span>
          )}
        </div>

        {/* Location */}
        {location && (
          <p className="text-sm text-gray-600 mb-3 flex items-center gap-1">
            <svg
              className="w-4 h-4 text-gray-400"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
              aria-hidden="true"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={2}
                d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z"
              />
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={2}
                d="M15 11a3 3 0 11-6 0 3 3 0 016 0z"
              />
            </svg>
            <span>
              {location.name}, {location.country}
            </span>
          </p>
        )}

        {/* Stats */}
        <div className="flex items-center gap-4 text-sm text-gray-600 mb-3">
          {/* Attractions count */}
          <div className="flex items-center gap-1">
            <svg
              className="w-4 h-4 text-gray-400"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
              aria-hidden="true"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={2}
                d="M5 3v4M3 5h4M6 17v4m-2-2h4m5-16l2.286 6.857L21 12l-5.714 2.143L13 21l-2.286-6.857L5 12l5.714-2.143L13 3z"
              />
            </svg>
            <span>
              {attractionCount} {attractionCount === 1 ? 'atrakcja' : attractionCount < 5 ? 'atrakcje' : 'atrakcji'}
            </span>
          </div>

          {/* Days count */}
          <div className="flex items-center gap-1">
            <svg
              className="w-4 h-4 text-gray-400"
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
            <span>
              {totalDays} {totalDays === 1 ? 'dzień' : 'dni'}
            </span>
          </div>
        </div>

        {/* Last updated */}
        <p className="text-xs text-gray-400">
          Ostatnia zmiana: {formatDate(updatedAt)}
        </p>
      </Link>
    </article>
  );
}
