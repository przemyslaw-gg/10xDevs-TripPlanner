import { useEffect, useRef, type ReactNode } from 'react';

interface ModalProps {
  isOpen: boolean;
  onClose: () => void;
  title: string;
  children: ReactNode;
  footer?: ReactNode;
}

/**
 * Base modal component with overlay, escape key handling, and focus trap
 */
export function Modal({ isOpen, onClose, title, children, footer }: ModalProps) {
  const modalRef = useRef<HTMLDivElement>(null);
  const previousActiveElement = useRef<HTMLElement | null>(null);

  // Handle escape key
  useEffect(() => {
    const handleKeyDown = (e: KeyboardEvent) => {
      if (e.key === 'Escape' && isOpen) {
        onClose();
      }
    };

    document.addEventListener('keydown', handleKeyDown);
    return () => document.removeEventListener('keydown', handleKeyDown);
  }, [isOpen, onClose]);

  // Focus management
  useEffect(() => {
    if (isOpen) {
      previousActiveElement.current = document.activeElement as HTMLElement;
      modalRef.current?.focus();

      // Prevent body scroll
      document.body.style.overflow = 'hidden';
    } else {
      // Restore body scroll
      document.body.style.overflow = '';

      // Restore focus
      previousActiveElement.current?.focus();
    }

    return () => {
      document.body.style.overflow = '';
    };
  }, [isOpen]);

  if (!isOpen) {
    return null;
  }

  const handleOverlayClick = (e: React.MouseEvent) => {
    if (e.target === e.currentTarget) {
      onClose();
    }
  };

  return (
    <div
      className="fixed inset-0 z-50 overflow-y-auto"
      aria-labelledby="modal-title"
      role="dialog"
      aria-modal="true"
    >
      {/* Overlay */}
      <div
        className="fixed inset-0 bg-gray-500 bg-opacity-75 transition-opacity"
        aria-hidden="true"
        onClick={handleOverlayClick}
      />

      {/* Modal container */}
      <div
        className="fixed inset-0 z-10 overflow-y-auto"
        onClick={handleOverlayClick}
      >
        <div className="flex min-h-full items-center justify-center p-4 text-center sm:p-0">
          {/* Modal panel */}
          <div
            ref={modalRef}
            tabIndex={-1}
            className="
              relative transform overflow-hidden rounded-lg
              bg-white text-left shadow-xl
              transition-all sm:my-8 sm:w-full sm:max-w-lg
              focus:outline-none
            "
            onClick={(e) => e.stopPropagation()}
          >
            {/* Header */}
            <div className="bg-white px-4 pt-5 pb-4 sm:p-6 sm:pb-4">
              <div className="flex items-start justify-between">
                <h3
                  id="modal-title"
                  className="text-lg font-semibold leading-6 text-gray-900"
                >
                  {title}
                </h3>
                <button
                  type="button"
                  onClick={onClose}
                  className="
                    rounded-md bg-white text-gray-400 hover:text-gray-500
                    focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2
                  "
                >
                  <span className="sr-only">Zamknij</span>
                  <svg className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                  </svg>
                </button>
              </div>

              {/* Content */}
              <div className="mt-4">
                {children}
              </div>
            </div>

            {/* Footer */}
            {footer && (
              <div className="bg-gray-50 px-4 py-3 sm:flex sm:flex-row-reverse sm:px-6 gap-3">
                {footer}
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
