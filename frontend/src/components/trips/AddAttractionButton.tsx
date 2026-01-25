import { Link } from 'react-router-dom';
import type { AddAttractionButtonProps } from '../../@types';

/**
 * Button to navigate to attractions page for adding to trip
 * Disabled when max attractions limit is reached
 */
export function AddAttractionButton({
  tripId,
  disabled = false,
  currentCount,
  maxCount,
}: AddAttractionButtonProps) {
  const isAtLimit = currentCount >= maxCount;
  const isDisabled = disabled || isAtLimit;

  if (isDisabled) {
    return (
      <button
        type="button"
        disabled
        className="
          inline-flex items-center gap-2 px-4 py-2.5
          bg-gray-100 text-gray-400
          text-sm font-medium rounded-md
          cursor-not-allowed
        "
        title={isAtLimit ? `Osiągnięto limit ${maxCount} atrakcji` : undefined}
      >
        <svg
          className="w-5 h-5"
          fill="none"
          stroke="currentColor"
          viewBox="0 0 24 24"
          aria-hidden="true"
        >
          <path
            strokeLinecap="round"
            strokeLinejoin="round"
            strokeWidth={2}
            d="M12 4v16m8-8H4"
          />
        </svg>
        Dodaj atrakcje
        {isAtLimit && (
          <span className="text-xs">({currentCount}/{maxCount})</span>
        )}
      </button>
    );
  }

  return (
    <Link
      to={`/attractions?tripId=${tripId}`}
      className="
        inline-flex items-center gap-2 px-4 py-2.5
        bg-blue-600 hover:bg-blue-700
        text-white text-sm font-medium
        rounded-md shadow-sm
        focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500
        transition-colors duration-150
      "
    >
      <svg
        className="w-5 h-5"
        fill="none"
        stroke="currentColor"
        viewBox="0 0 24 24"
        aria-hidden="true"
      >
        <path
          strokeLinecap="round"
          strokeLinejoin="round"
          strokeWidth={2}
          d="M12 4v16m8-8H4"
        />
      </svg>
      Dodaj atrakcje
    </Link>
  );
}
