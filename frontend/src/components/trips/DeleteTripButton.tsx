import type { DeleteTripButtonProps } from '../../@types';

/**
 * Danger button for initiating trip deletion
 */
export function DeleteTripButton({ onClick, disabled = false }: DeleteTripButtonProps) {
  return (
    <button
      type="button"
      onClick={onClick}
      disabled={disabled}
      className="
        inline-flex items-center gap-2 px-4 py-2
        text-sm font-medium text-red-700
        bg-white border border-red-300 rounded-md shadow-sm
        hover:bg-red-50 hover:border-red-400
        focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-red-500
        disabled:opacity-50 disabled:cursor-not-allowed
        transition-colors duration-150
      "
    >
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
          d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"
        />
      </svg>
      Usuń wycieczkę
    </button>
  );
}
