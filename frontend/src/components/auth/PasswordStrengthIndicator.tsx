import type { PasswordStrengthIndicatorProps } from '../../@types';

const strengthConfig = {
  weak: {
    label: 'Słabe',
    color: 'bg-red-500',
    textColor: 'text-red-600',
  },
  fair: {
    label: 'Średnie',
    color: 'bg-orange-500',
    textColor: 'text-orange-600',
  },
  good: {
    label: 'Dobre',
    color: 'bg-yellow-500',
    textColor: 'text-yellow-600',
  },
  strong: {
    label: 'Silne',
    color: 'bg-green-500',
    textColor: 'text-green-600',
  },
};

/**
 * Visual password strength indicator with progress bar and label
 */
export function PasswordStrengthIndicator({ strength }: PasswordStrengthIndicatorProps) {
  const config = strengthConfig[strength.level];
  const segments = 4;

  return (
    <div className="space-y-1">
      {/* Progress bar with segments */}
      <div className="flex gap-1">
        {Array.from({ length: segments }).map((_, index) => (
          <div
            key={index}
            className={`
              h-1.5 flex-1 rounded-full transition-colors duration-200
              ${index < strength.score ? config.color : 'bg-gray-200'}
            `}
          />
        ))}
      </div>

      {/* Strength label */}
      {strength.score > 0 && (
        <p
          className={`text-xs font-medium ${config.textColor}`}
          aria-live="polite"
        >
          Siła hasła: {config.label}
        </p>
      )}
    </div>
  );
}
