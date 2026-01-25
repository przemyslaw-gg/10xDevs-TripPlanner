import { useState } from 'react';
import { Link } from 'react-router-dom';
import type { AttractionListItemDTO, UUID } from '../../@types';

interface AttractionCardProps {
  attraction: AttractionListItemDTO;
  isOwned?: boolean;
  /** When provided, shows "Add" button instead of being a link */
  onAddToTrip?: (attractionId: UUID) => Promise<void>;
  /** Whether the add action is in progress */
  isAdding?: boolean;
}

function formatDuration(minutes: number | null): string {
  if (minutes === null) return 'Brak danych';
  if (minutes < 60) return `${minutes} min`;
  const hours = Math.floor(minutes / 60);
  const remainingMinutes = minutes % 60;
  if (remainingMinutes === 0) return `${hours} godz.`;
  return `${hours} godz. ${remainingMinutes} min`;
}

function formatRating(rating: number | null): string {
  if (rating === null) return 'Brak oceny';
  return rating.toFixed(1);
}

function formatReviewCount(count: number | null): string {
  if (count === null || count === 0) return '';
  if (count >= 1000) {
    return `(${(count / 1000).toFixed(1)}k opinii)`;
  }
  return `(${count} opinii)`;
}

export function AttractionCard({
  attraction,
  isOwned = false,
  onAddToTrip,
  isAdding = false,
}: AttractionCardProps) {
  const [localAdding, setLocalAdding] = useState(false);
  const [added, setAdded] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const {
    id,
    name,
    description,
    rating,
    reviewCount,
    estimatedDuration,
    imageUrl,
    isVerified,
  } = attraction;

  const truncatedDescription = description
    ? description.length > 120
      ? `${description.slice(0, 120)}...`
      : description
    : 'Brak opisu';

  const placeholderImage =
    'data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" width="400" height="300" viewBox="0 0 400 300"%3E%3Crect fill="%23e5e7eb" width="400" height="300"/%3E%3Ctext fill="%239ca3af" font-family="sans-serif" font-size="24" x="50%25" y="50%25" text-anchor="middle" dy=".3em"%3EBrak zdjęcia%3C/text%3E%3C/svg%3E';

  const handleAddClick = async (e: React.MouseEvent) => {
    e.preventDefault();
    e.stopPropagation();

    if (!onAddToTrip || localAdding || added) return;

    setLocalAdding(true);
    setError(null);

    try {
      await onAddToTrip(id);
      setAdded(true);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Nie udało się dodać');
    } finally {
      setLocalAdding(false);
    }
  };

  const isProcessing = isAdding || localAdding;

  // Card content - shared between link and div variants
  const cardContent = (
    <>
      {/* Image */}
      <div className="relative">
        <img
          src={imageUrl ?? placeholderImage}
          alt={`Zdjęcie atrakcji: ${name}`}
          className="w-full h-48 object-cover"
          loading="lazy"
          onError={(e) => {
            const target = e.target as HTMLImageElement;
            target.src = placeholderImage;
          }}
        />
        {/* Badges */}
        <div className="absolute top-2 right-2 flex flex-col gap-1">
          {isVerified && (
            <span className="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium bg-green-100 text-green-800">
              <svg
                className="w-3 h-3 mr-1"
                fill="currentColor"
                viewBox="0 0 20 20"
                aria-hidden="true"
              >
                <path
                  fillRule="evenodd"
                  d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z"
                  clipRule="evenodd"
                />
              </svg>
              Zweryfikowana
            </span>
          )}
          {isOwned && (
            <span className="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium bg-blue-100 text-blue-800">
              Własna
            </span>
          )}
        </div>
      </div>

      {/* Content */}
      <div className="p-4 flex-1 flex flex-col">
        <h3 className="text-lg font-semibold text-gray-900 mb-2 line-clamp-2">
          {name}
        </h3>
        <p className="text-sm text-gray-600 mb-4 flex-1">
          {truncatedDescription}
        </p>

        {/* Rating and duration */}
        <div className="flex items-center justify-between text-sm">
          <div className="flex items-center gap-1">
            {rating !== null && (
              <>
                <svg
                  className="w-4 h-4 text-yellow-400"
                  fill="currentColor"
                  viewBox="0 0 20 20"
                  aria-hidden="true"
                >
                  <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
                </svg>
                <span className="font-medium text-gray-900">
                  {formatRating(rating)}
                </span>
                <span className="text-gray-500">
                  {formatReviewCount(reviewCount)}
                </span>
              </>
            )}
            {rating === null && (
              <span className="text-gray-400">Brak oceny</span>
            )}
          </div>
          <div className="flex items-center gap-1 text-gray-500">
            <svg
              className="w-4 h-4"
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
            <span>{formatDuration(estimatedDuration)}</span>
          </div>
        </div>

        {/* Add to trip button - only shown when onAddToTrip is provided */}
        {onAddToTrip && (
          <div className="mt-4 pt-4 border-t border-gray-100">
            {added ? (
              <div className="flex items-center justify-center gap-2 py-2 text-green-600">
                <svg className="w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
                  <path
                    fillRule="evenodd"
                    d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z"
                    clipRule="evenodd"
                  />
                </svg>
                <span className="font-medium">Dodano do wycieczki</span>
              </div>
            ) : error ? (
              <div className="text-center">
                <p className="text-sm text-red-600 mb-2">{error}</p>
                <button
                  type="button"
                  onClick={handleAddClick}
                  className="text-sm text-blue-600 hover:text-blue-800 underline"
                >
                  Spróbuj ponownie
                </button>
              </div>
            ) : (
              <button
                type="button"
                onClick={handleAddClick}
                disabled={isProcessing}
                className="
                  w-full flex items-center justify-center gap-2 px-4 py-2
                  bg-blue-600 hover:bg-blue-700 text-white
                  font-medium rounded-md shadow-sm
                  focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500
                  disabled:opacity-50 disabled:cursor-not-allowed
                  transition-colors duration-150
                "
              >
                {isProcessing ? (
                  <>
                    <svg
                      className="animate-spin h-4 w-4"
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
                    Dodawanie...
                  </>
                ) : (
                  <>
                    <svg
                      className="w-5 h-5"
                      fill="none"
                      stroke="currentColor"
                      viewBox="0 0 24 24"
                    >
                      <path
                        strokeLinecap="round"
                        strokeLinejoin="round"
                        strokeWidth={2}
                        d="M12 4v16m8-8H4"
                      />
                    </svg>
                    Dodaj do wycieczki
                  </>
                )}
              </button>
            )}
          </div>
        )}
      </div>
    </>
  );

  // When onAddToTrip is provided, render as div (not clickable link)
  if (onAddToTrip) {
    return (
      <article className="bg-white rounded-lg shadow-md overflow-hidden flex flex-col h-full">
        {cardContent}
      </article>
    );
  }

  // Default: render as link to attraction details
  return (
    <article className="bg-white rounded-lg shadow-md overflow-hidden hover:shadow-lg transition-shadow duration-200 flex flex-col h-full">
      <Link to={`/attractions/${id}`} className="block flex-1 flex flex-col">
        {cardContent}
      </Link>
    </article>
  );
}
