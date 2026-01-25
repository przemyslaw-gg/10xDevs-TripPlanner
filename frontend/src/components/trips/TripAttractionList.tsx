import { useMemo } from 'react';
import type { TripAttractionListProps, TripAttractionItemDTO } from '../../@types';
import { AttractionItem } from './AttractionItem';
import { DaySeparator } from './DaySeparator';

/**
 * Represents either an attraction item or a day separator in the rendered list
 */
type ListItem =
  | { type: 'attraction'; data: TripAttractionItemDTO; index: number }
  | { type: 'separator'; dayNumber: number };

/**
 * Calculate day breaks based on daily hours limit and attraction durations
 * Returns a list of items with separators inserted at day boundaries
 */
function calculateDayBreaks(
  attractions: TripAttractionItemDTO[],
  dailyMinutes: number
): ListItem[] {
  if (attractions.length === 0) return [];

  const items: ListItem[] = [];
  let currentDayMinutes = 0;
  let currentDay = 1;

  attractions.forEach((attraction, index) => {
    const duration = attraction.attraction.estimatedDuration ?? 0;

    // Check if adding this attraction exceeds daily limit
    // If so, start a new day (but only if we've already added something to current day)
    if (currentDayMinutes > 0 && currentDayMinutes + duration > dailyMinutes) {
      currentDay++;
      currentDayMinutes = 0;
      // Add separator before this attraction
      items.push({ type: 'separator', dayNumber: currentDay });
    }

    // Add the attraction
    items.push({ type: 'attraction', data: attraction, index });
    currentDayMinutes += duration;
  });

  return items;
}

/**
 * List of attractions assigned to a trip with reorder functionality
 * Shows day separators based on daily hours limit
 */
export function TripAttractionList({
  attractions,
  totalCount,
  maxCount,
  dailyHours,
  onMoveUp,
  onMoveDown,
  onRemove,
  isLoading = false,
  disabled = false,
}: TripAttractionListProps) {
  // Calculate day breaks
  const dailyMinutes = dailyHours * 60;
  const listItems = useMemo(
    () => calculateDayBreaks(attractions, dailyMinutes),
    [attractions, dailyMinutes]
  );
  // Loading skeleton
  if (isLoading) {
    return (
      <div className="space-y-3">
        {Array.from({ length: 3 }).map((_, index) => (
          <div
            key={index}
            className="flex items-center gap-4 p-4 bg-white rounded-lg border border-gray-200 animate-pulse"
          >
            <div className="w-16 h-16 bg-gray-200 rounded-md" />
            <div className="flex-1">
              <div className="h-4 bg-gray-200 rounded w-3/4 mb-2" />
              <div className="h-3 bg-gray-200 rounded w-1/2" />
            </div>
          </div>
        ))}
      </div>
    );
  }

  // Empty state
  if (attractions.length === 0) {
    return (
      <div className="text-center py-12 bg-gray-50 rounded-lg border-2 border-dashed border-gray-300">
        <svg
          className="mx-auto h-12 w-12 text-gray-400"
          fill="none"
          stroke="currentColor"
          viewBox="0 0 24 24"
          aria-hidden="true"
        >
          <path
            strokeLinecap="round"
            strokeLinejoin="round"
            strokeWidth={1.5}
            d="M5 3v4M3 5h4M6 17v4m-2-2h4m5-16l2.286 6.857L21 12l-5.714 2.143L13 21l-2.286-6.857L5 12l5.714-2.143L13 3z"
          />
        </svg>
        <h3 className="mt-4 text-sm font-medium text-gray-900">
          Brak atrakcji
        </h3>
        <p className="mt-1 text-sm text-gray-500">
          Dodaj atrakcje do swojej wycieczki, aby rozpocząć planowanie.
        </p>
      </div>
    );
  }

  return (
    <div>
      {/* Header with count */}
      <div className="flex items-center justify-between mb-4">
        <h2 className="text-lg font-semibold text-gray-900">
          Atrakcje
        </h2>
        <span className={`text-sm ${totalCount >= maxCount ? 'text-red-600 font-medium' : 'text-gray-500'}`}>
          {totalCount}/{maxCount}
        </span>
      </div>

      {/* Attraction list with day separators */}
      <div className="space-y-3">
        {listItems.map((item, idx) => {
          if (item.type === 'separator') {
            return (
              <DaySeparator
                key={`separator-day-${item.dayNumber}`}
                dayNumber={item.dayNumber}
              />
            );
          }

          const { data: attraction, index } = item;
          return (
            <AttractionItem
              key={attraction.id}
              attraction={attraction}
              isFirst={index === 0}
              isLast={index === attractions.length - 1}
              onMoveUp={() => onMoveUp(attraction.attractionId)}
              onMoveDown={() => onMoveDown(attraction.attractionId)}
              onRemove={() => onRemove(attraction.attractionId)}
              disabled={disabled}
            />
          );
        })}
      </div>

      {/* Limit warning */}
      {totalCount >= maxCount && (
        <p className="mt-4 text-sm text-red-600 flex items-center gap-2">
          <svg className="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
            <path
              fillRule="evenodd"
              d="M8.257 3.099c.765-1.36 2.722-1.36 3.486 0l5.58 9.92c.75 1.334-.213 2.98-1.742 2.98H4.42c-1.53 0-2.493-1.646-1.743-2.98l5.58-9.92zM11 13a1 1 0 11-2 0 1 1 0 012 0zm-1-8a1 1 0 00-1 1v3a1 1 0 002 0V6a1 1 0 00-1-1z"
              clipRule="evenodd"
            />
          </svg>
          Osiągnięto maksymalną liczbę atrakcji
        </p>
      )}
    </div>
  );
}
