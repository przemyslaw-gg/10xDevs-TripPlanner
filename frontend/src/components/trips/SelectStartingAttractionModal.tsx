import { useState } from 'react';
import type { SelectStartingAttractionModalProps, UUID } from '../../@types';
import { Modal } from '../common/Modal';
import { AttractionRadioItem } from './AttractionRadioItem';

/**
 * Modal for selecting the starting attraction for route optimization
 * Displays a list of attractions with radio buttons
 */
export function SelectStartingAttractionModal({
  isOpen,
  attractions,
  onConfirm,
  onCancel,
  isOptimizing = false,
}: SelectStartingAttractionModalProps) {
  const [selectedAttractionId, setSelectedAttractionId] = useState<UUID | null>(null);

  const handleConfirm = () => {
    if (selectedAttractionId) {
      onConfirm(selectedAttractionId);
    }
  };

  const handleCancel = () => {
    if (!isOptimizing) {
      setSelectedAttractionId(null);
      onCancel();
    }
  };

  const footer = (
    <>
      <button
        type="button"
        onClick={handleConfirm}
        disabled={!selectedAttractionId || isOptimizing}
        className="
          inline-flex justify-center items-center gap-2
          rounded-md bg-blue-600 px-4 py-2
          text-sm font-semibold text-white shadow-sm
          hover:bg-blue-500
          focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2
          disabled:opacity-50 disabled:cursor-not-allowed disabled:hover:bg-blue-600
          transition-colors
        "
      >
        {isOptimizing ? (
          <>
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
            Optymalizuję...
          </>
        ) : (
          <>
            <svg
              className="w-4 h-4"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={2}
                d="M9 20l-5.447-2.724A1 1 0 013 16.382V5.618a1 1 0 011.447-.894L9 7m0 13l6-3m-6 3V7m6 10l4.553 2.276A1 1 0 0021 18.382V7.618a1 1 0 00-.553-.894L15 4m0 13V4m0 0L9 7"
              />
            </svg>
            Optymalizuj
          </>
        )}
      </button>
      <button
        type="button"
        onClick={handleCancel}
        disabled={isOptimizing}
        className="
          inline-flex justify-center rounded-md bg-white px-4 py-2
          text-sm font-semibold text-gray-900 shadow-sm
          ring-1 ring-inset ring-gray-300
          hover:bg-gray-50
          focus:outline-none focus:ring-2 focus:ring-gray-500 focus:ring-offset-2
          disabled:opacity-50 disabled:cursor-not-allowed
          transition-colors
        "
      >
        Anuluj
      </button>
    </>
  );

  return (
    <Modal
      isOpen={isOpen}
      onClose={handleCancel}
      title="Wybierz atrakcję startową"
      footer={footer}
    >
      <div className="space-y-4">
        <p className="text-sm text-gray-600">
          Wybierz atrakcję, od której chcesz rozpocząć zwiedzanie.
          Algorytm obliczy optymalną trasę minimalizującą dystans między atrakcjami.
        </p>

        {/* Attractions list */}
        <div className="space-y-2 max-h-80 overflow-y-auto pr-1">
          {attractions.map((attraction) => (
            <AttractionRadioItem
              key={attraction.attractionId}
              attraction={attraction}
              isSelected={selectedAttractionId === attraction.attractionId}
              onSelect={() => setSelectedAttractionId(attraction.attractionId)}
              disabled={isOptimizing}
            />
          ))}
        </div>

        {attractions.length === 0 && (
          <div className="text-center py-8 text-gray-500">
            <svg
              className="mx-auto h-12 w-12 text-gray-400"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={1.5}
                d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z"
              />
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={1.5}
                d="M15 11a3 3 0 11-6 0 3 3 0 016 0z"
              />
            </svg>
            <p className="mt-2">Brak atrakcji w wycieczce</p>
          </div>
        )}
      </div>
    </Modal>
  );
}
