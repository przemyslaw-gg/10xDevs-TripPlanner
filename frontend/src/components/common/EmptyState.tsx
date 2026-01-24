interface EmptyStateProps {
  title?: string;
  message?: string;
  onClearFilters?: () => void;
}

export function EmptyState({
  title = 'Brak wyników',
  message = 'Nie znaleziono żadnych elementów spełniających kryteria wyszukiwania.',
  onClearFilters,
}: EmptyStateProps) {
  return (
    <div className="flex flex-col items-center justify-center py-12 px-4 text-center">
      <svg
        className="w-16 h-16 text-gray-400 mb-4"
        fill="none"
        stroke="currentColor"
        viewBox="0 0 24 24"
        aria-hidden="true"
      >
        <path
          strokeLinecap="round"
          strokeLinejoin="round"
          strokeWidth={1.5}
          d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"
        />
      </svg>
      <h3 className="text-lg font-medium text-gray-900 mb-2">{title}</h3>
      <p className="text-gray-500 max-w-sm mb-4">{message}</p>
      {onClearFilters && (
        <button
          onClick={onClearFilters}
          className="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 transition-colors"
        >
          Wyczyść filtry
        </button>
      )}
    </div>
  );
}
