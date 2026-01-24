import { Link } from 'react-router-dom';

/**
 * Button/link that navigates to the create trip page
 */
export function CreateTripButton() {
  return (
    <Link
      to="/trips/new"
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
      Nowa wycieczka
    </Link>
  );
}
