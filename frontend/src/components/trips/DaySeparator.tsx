import type { DaySeparatorProps } from '../../@types';

/**
 * Visual separator between days in the attraction list
 * Shows the day number to indicate when a new day of sightseeing begins
 */
export function DaySeparator({ dayNumber }: DaySeparatorProps) {
  return (
    <div className="flex items-center gap-4 py-3">
      <div className="flex-1 h-px bg-gray-300" />
      <span className="px-3 py-1 text-sm font-medium text-gray-600 bg-gray-100 rounded-full">
        Dzień {dayNumber}
      </span>
      <div className="flex-1 h-px bg-gray-300" />
    </div>
  );
}
