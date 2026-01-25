import { useEffect } from 'react';
import type { OptimizationResultToastProps } from '../../@types';

const AUTO_HIDE_DELAY = 5000; // 5 seconds

/**
 * Format distance in meters to a human-readable string
 * Shows meters for distances < 1km, kilometers otherwise
 */
function formatDistance(meters: number): string {
  if (meters < 1000) {
    return `${Math.round(meters)} m`;
  }
  const km = meters / 1000;
  return `${km.toFixed(1)} km`;
}

/**
 * Toast notification showing the optimization result
 * Auto-hides after 5 seconds
 */
export function OptimizationResultToast({
  isVisible,
  totalDistance,
  onClose,
}: OptimizationResultToastProps) {
  // Auto-hide after delay
  useEffect(() => {
    if (isVisible) {
      const timer = setTimeout(() => {
        onClose();
      }, AUTO_HIDE_DELAY);

      return () => clearTimeout(timer);
    }
  }, [isVisible, onClose]);

  if (!isVisible) {
    return null;
  }

  return (
    <div
      className="
        fixed bottom-4 right-4 z-50
        max-w-sm w-full
        bg-green-50 border border-green-200
        rounded-lg shadow-lg
        transform transition-all duration-300 ease-out
        animate-slide-in-right
      "
      role="alert"
      aria-live="polite"
    >
      <div className="p-4">
        <div className="flex items-start gap-3">
          {/* Success icon */}
          <div className="flex-shrink-0">
            <svg
              className="h-6 w-6 text-green-500"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
              aria-hidden="true"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={2}
                d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"
              />
            </svg>
          </div>

          {/* Content */}
          <div className="flex-1 min-w-0">
            <h4 className="text-sm font-medium text-green-800">
              Trasa zoptymalizowana!
            </h4>
            <p className="mt-1 text-sm text-green-700">
              Całkowita odległość trasy:{' '}
              <span className="font-semibold">{formatDistance(totalDistance)}</span>
            </p>
          </div>

          {/* Close button */}
          <button
            type="button"
            onClick={onClose}
            className="
              flex-shrink-0 ml-auto
              text-green-500 hover:text-green-700
              focus:outline-none focus:ring-2 focus:ring-green-500 focus:ring-offset-2
              rounded-md
              transition-colors
            "
            aria-label="Zamknij powiadomienie"
          >
            <svg className="h-5 w-5" fill="currentColor" viewBox="0 0 20 20">
              <path
                fillRule="evenodd"
                d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z"
                clipRule="evenodd"
              />
            </svg>
          </button>
        </div>
      </div>
    </div>
  );
}
