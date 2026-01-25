import type { ConfirmDeleteModalProps } from '../../@types';
import { Modal } from '../common/Modal';

/**
 * Modal for confirming trip deletion
 */
export function ConfirmDeleteModal({
  isOpen,
  tripName,
  onConfirm,
  onCancel,
  isDeleting = false,
}: ConfirmDeleteModalProps) {
  return (
    <Modal
      isOpen={isOpen}
      onClose={onCancel}
      title="Usuń wycieczkę"
      footer={
        <>
          <button
            type="button"
            onClick={onConfirm}
            disabled={isDeleting}
            className="
              w-full sm:w-auto inline-flex justify-center
              rounded-md border border-transparent
              bg-red-600 px-4 py-2
              text-base font-medium text-white shadow-sm
              hover:bg-red-700
              focus:outline-none focus:ring-2 focus:ring-red-500 focus:ring-offset-2
              disabled:opacity-50 disabled:cursor-not-allowed
              sm:ml-3 sm:text-sm
            "
          >
            {isDeleting ? (
              <span className="flex items-center gap-2">
                <svg
                  className="animate-spin h-4 w-4"
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
                Usuwanie...
              </span>
            ) : (
              'Usuń'
            )}
          </button>
          <button
            type="button"
            onClick={onCancel}
            disabled={isDeleting}
            className="
              mt-3 w-full sm:w-auto sm:mt-0 inline-flex justify-center
              rounded-md border border-gray-300
              bg-white px-4 py-2
              text-base font-medium text-gray-700 shadow-sm
              hover:bg-gray-50
              focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2
              disabled:opacity-50 disabled:cursor-not-allowed
              sm:text-sm
            "
          >
            Anuluj
          </button>
        </>
      }
    >
      <div className="sm:flex sm:items-start">
        <div className="mx-auto flex h-12 w-12 flex-shrink-0 items-center justify-center rounded-full bg-red-100 sm:mx-0 sm:h-10 sm:w-10">
          <svg
            className="h-6 w-6 text-red-600"
            fill="none"
            viewBox="0 0 24 24"
            strokeWidth="1.5"
            stroke="currentColor"
            aria-hidden="true"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              d="M12 9v3.75m-9.303 3.376c-.866 1.5.217 3.374 1.948 3.374h14.71c1.73 0 2.813-1.874 1.948-3.374L13.949 3.378c-.866-1.5-3.032-1.5-3.898 0L2.697 16.126zM12 15.75h.007v.008H12v-.008z"
            />
          </svg>
        </div>
        <div className="mt-3 text-center sm:ml-4 sm:mt-0 sm:text-left">
          <p className="text-sm text-gray-500">
            Czy na pewno chcesz usunąć wycieczkę{' '}
            <span className="font-medium text-gray-900">"{tripName}"</span>?
            Ta akcja jest nieodwracalna. Wszystkie dane wycieczki, w tym
            przypisane atrakcje, zostaną trwale usunięte.
          </p>
        </div>
      </div>
    </Modal>
  );
}
