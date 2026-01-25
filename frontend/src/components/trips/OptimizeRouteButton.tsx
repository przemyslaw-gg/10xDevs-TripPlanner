import type { OptimizeRouteButtonProps } from '../../@types';

const MIN_ATTRACTIONS_FOR_OPTIMIZATION = 2;

/**
 * Button to trigger route optimization
 * Disabled when there are less than 2 attractions or during saving/optimizing
 */
export function OptimizeRouteButton({
  onClick,
  disabled = false,
  isOptimizing = false,
  attractionsCount,
}: OptimizeRouteButtonProps) {
  const hasEnoughAttractions = attractionsCount >= MIN_ATTRACTIONS_FOR_OPTIMIZATION;
  const isDisabled = disabled || isOptimizing || !hasEnoughAttractions;

  const getTooltip = (): string | undefined => {
    if (!hasEnoughAttractions) {
      return `Dodaj minimum ${MIN_ATTRACTIONS_FOR_OPTIMIZATION} atrakcje, aby zoptymalizować trasę`;
    }
    if (isOptimizing) {
      return 'Trwa optymalizacja...';
    }
    return 'Oblicz optymalną kolejność zwiedzania';
  };

  return (
    <button
      type="button"
      onClick={onClick}
      disabled={isDisabled}
      title={getTooltip()}
      className={`
        inline-flex items-center gap-2 px-4 py-2.5
        text-sm font-medium rounded-md shadow-sm
        focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500
        transition-colors duration-150
        ${isDisabled
          ? 'bg-gray-100 text-gray-400 cursor-not-allowed'
          : 'bg-green-600 hover:bg-green-700 text-white'
        }
      `}
    >
      {isOptimizing ? (
        <>
          <svg
            className="animate-spin w-5 h-5"
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
          Optymalizuję...
        </>
      ) : (
        <>
          {/* Route/Map icon */}
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
              d="M9 20l-5.447-2.724A1 1 0 013 16.382V5.618a1 1 0 011.447-.894L9 7m0 13l6-3m-6 3V7m6 10l4.553 2.276A1 1 0 0021 18.382V7.618a1 1 0 00-.553-.894L15 4m0 13V4m0 0L9 7"
            />
          </svg>
          Optymalizuj trasę
        </>
      )}
    </button>
  );
}
