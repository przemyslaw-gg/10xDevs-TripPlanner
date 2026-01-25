import { Link } from 'react-router-dom';
import type { TripHeaderProps } from '../../@types';
import { EditableTitle } from './EditableTitle';
import { SaveStatusIndicator } from './SaveStatusIndicator';

/**
 * Header section for trip details page
 * Contains editable title, location display, and save status
 */
export function TripHeader({ trip, onNameChange, saveStatus }: TripHeaderProps) {
  return (
    <div className="bg-white shadow-sm border-b border-gray-200">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
        {/* Breadcrumb */}
        <nav className="mb-4" aria-label="Breadcrumb">
          <ol className="flex items-center gap-2 text-sm text-gray-500">
            <li>
              <Link to="/trips" className="hover:text-gray-700">
                Moje wycieczki
              </Link>
            </li>
            <li aria-hidden="true">/</li>
            <li className="text-gray-900 font-medium truncate max-w-[200px]">
              {trip.name}
            </li>
          </ol>
        </nav>

        {/* Title row */}
        <div className="flex flex-col sm:flex-row sm:items-start sm:justify-between gap-4">
          <div className="flex-1 min-w-0">
            {/* Editable title */}
            <EditableTitle
              value={trip.name}
              onChange={onNameChange}
              maxLength={100}
              placeholder="Nazwa wycieczki"
            />

            {/* Location */}
            {trip.location && (
              <p className="mt-2 text-sm text-gray-600 flex items-center gap-1">
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
                  {trip.location.name}, {trip.location.country}
                </span>
              </p>
            )}

            {/* Public badge */}
            {trip.isPublic && (
              <span className="mt-2 inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800">
                Publiczna
              </span>
            )}
          </div>

          {/* Save status */}
          <div className="flex-shrink-0">
            <SaveStatusIndicator status={saveStatus} />
          </div>
        </div>
      </div>
    </div>
  );
}
