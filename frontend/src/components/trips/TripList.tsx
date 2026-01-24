import type { TripListProps } from '../../@types';
import { TripCard } from './TripCard';
import { SkeletonTripCard } from './SkeletonTripCard';
import { EmptyState } from '../common/EmptyState';

/**
 * Container for trip cards with loading and empty states
 */
export function TripList({ trips, isLoading, onClearFilters }: TripListProps) {
  // Loading state - show skeleton cards
  if (isLoading) {
    return (
      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
        {Array.from({ length: 6 }).map((_, index) => (
          <SkeletonTripCard key={index} />
        ))}
      </div>
    );
  }

  // Empty state
  if (trips.length === 0) {
    return (
      <EmptyState
        title="Brak wycieczek"
        message="Nie masz jeszcze żadnych zapisanych wycieczek. Utwórz swoją pierwszą wycieczkę!"
        onClearFilters={onClearFilters}
      />
    );
  }

  // Trip cards grid
  return (
    <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
      {trips.map((trip) => (
        <TripCard key={trip.id} trip={trip} />
      ))}
    </div>
  );
}
