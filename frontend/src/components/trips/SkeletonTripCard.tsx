/**
 * Placeholder card displayed while loading trips
 */
export function SkeletonTripCard() {
  return (
    <article className="bg-white rounded-lg shadow-md p-5 animate-pulse">
      {/* Title skeleton */}
      <div className="h-6 bg-gray-200 rounded w-3/4 mb-3" />

      {/* Location skeleton */}
      <div className="h-4 bg-gray-200 rounded w-1/2 mb-4" />

      {/* Stats row skeleton */}
      <div className="flex items-center gap-4 mb-4">
        <div className="h-4 bg-gray-200 rounded w-20" />
        <div className="h-4 bg-gray-200 rounded w-16" />
      </div>

      {/* Date skeleton */}
      <div className="h-3 bg-gray-200 rounded w-1/3" />
    </article>
  );
}
