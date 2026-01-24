export function SkeletonAttractionCard() {
  return (
    <div className="bg-white rounded-lg shadow-md overflow-hidden animate-pulse">
      {/* Image placeholder */}
      <div className="w-full h-48 bg-gray-300" />

      <div className="p-4">
        {/* Title placeholder */}
        <div className="h-6 bg-gray-300 rounded w-3/4 mb-2" />

        {/* Description placeholder */}
        <div className="space-y-2 mb-4">
          <div className="h-4 bg-gray-200 rounded w-full" />
          <div className="h-4 bg-gray-200 rounded w-5/6" />
        </div>

        {/* Rating and duration placeholder */}
        <div className="flex items-center justify-between">
          <div className="flex items-center gap-2">
            <div className="h-4 w-4 bg-gray-300 rounded" />
            <div className="h-4 bg-gray-200 rounded w-16" />
          </div>
          <div className="h-4 bg-gray-200 rounded w-20" />
        </div>
      </div>
    </div>
  );
}
