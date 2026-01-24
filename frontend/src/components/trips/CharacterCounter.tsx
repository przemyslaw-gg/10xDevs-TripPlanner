import type { CharacterCounterProps } from '../../@types';

/**
 * Character counter for text inputs
 * Only visible when current count exceeds warning threshold
 */
export function CharacterCounter({
  current,
  max,
  warningThreshold = 80,
}: CharacterCounterProps) {
  // Don't render if below warning threshold
  if (current < warningThreshold) {
    return null;
  }

  const isOverLimit = current > max;
  const isNearLimit = current >= max - 10;

  return (
    <span
      className={`text-xs ${
        isOverLimit
          ? 'text-red-600 font-medium'
          : isNearLimit
          ? 'text-yellow-600'
          : 'text-gray-500'
      }`}
      aria-live="polite"
    >
      {current}/{max}
    </span>
  );
}
