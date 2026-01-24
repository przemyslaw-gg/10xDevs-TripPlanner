import type { PasswordRequirementsProps } from '../../@types';

interface Requirement {
  key: keyof PasswordRequirementsProps['requirements'];
  label: string;
}

const requirements: Requirement[] = [
  { key: 'minLength', label: 'Minimum 8 znaków' },
  { key: 'hasUppercase', label: 'Przynajmniej jedna wielka litera' },
  { key: 'hasDigit', label: 'Przynajmniej jedna cyfra' },
];

/**
 * List of password requirements with visual indicators
 */
export function PasswordRequirements({ requirements: status }: PasswordRequirementsProps) {
  return (
    <ul className="space-y-1 text-sm">
      {requirements.map(({ key, label }) => {
        const isMet = status[key];

        return (
          <li
            key={key}
            className={`flex items-center gap-2 ${
              isMet ? 'text-green-600' : 'text-gray-500'
            }`}
          >
            {isMet ? (
              // Checkmark icon
              <svg
                xmlns="http://www.w3.org/2000/svg"
                fill="none"
                viewBox="0 0 24 24"
                strokeWidth={2}
                stroke="currentColor"
                className="w-4 h-4"
                aria-hidden="true"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  d="M4.5 12.75l6 6 9-13.5"
                />
              </svg>
            ) : (
              // X icon
              <svg
                xmlns="http://www.w3.org/2000/svg"
                fill="none"
                viewBox="0 0 24 24"
                strokeWidth={2}
                stroke="currentColor"
                className="w-4 h-4"
                aria-hidden="true"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  d="M6 18L18 6M6 6l12 12"
                />
              </svg>
            )}
            <span>{label}</span>
          </li>
        );
      })}
    </ul>
  );
}
