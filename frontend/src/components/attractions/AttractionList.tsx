import type { AttractionListItemDTO, UUID } from '../../@types';
import { AttractionCard } from './AttractionCard';
import { SkeletonAttractionCard } from './SkeletonAttractionCard';
import { EmptyState } from '../common/EmptyState';

interface AttractionListProps {
  attractions: AttractionListItemDTO[];
  isLoading: boolean;
  currentUserId?: UUID | null;
  onClearFilters?: () => void;
  /** When provided, shows "Add" button on cards */
  onAddToTrip?: (attractionId: UUID) => Promise<void>;
}

const SKELETON_COUNT = 6;

export function AttractionList({
  attractions,
  isLoading,
  currentUserId,
  onClearFilters,
  onAddToTrip,
}: AttractionListProps) {
  // Loading state - show skeletons
  if (isLoading) {
    return (
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {Array.from({ length: SKELETON_COUNT }).map((_, index) => (
          <SkeletonAttractionCard key={index} />
        ))}
      </div>
    );
  }

  // Empty state - no attractions found
  if (attractions.length === 0) {
    return (
      <EmptyState
        title="Brak atrakcji"
        message="Nie znaleziono żadnych atrakcji dla wybranych kryteriów. Spróbuj zmienić filtr lokalizacji."
        onClearFilters={onClearFilters}
      />
    );
  }

  // Data state - show attraction cards
  return (
    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      {attractions.map((attraction) => (
        <AttractionCard
          key={attraction.id}
          attraction={attraction}
          isOwned={
            currentUserId !== null &&
            currentUserId !== undefined &&
            attraction.createdByUserId === currentUserId
          }
          onAddToTrip={onAddToTrip}
        />
      ))}
    </div>
  );
}
