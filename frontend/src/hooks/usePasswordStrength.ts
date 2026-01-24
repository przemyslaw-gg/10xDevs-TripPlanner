import { useMemo } from 'react';
import type { PasswordStrength, PasswordStrengthLevel } from '../@types';

/**
 * Hook to calculate password strength based on requirements
 *
 * Requirements:
 * - Minimum 8 characters
 * - At least one uppercase letter
 * - At least one digit
 *
 * Score calculation:
 * - 0: No requirements met
 * - 1: 1 requirement met
 * - 2: 2 requirements met
 * - 3: All requirements met
 * - 4: All requirements met + password length >= 12
 */
export function usePasswordStrength(password: string): PasswordStrength {
  return useMemo(() => {
    const requirements = {
      minLength: password.length >= 8,
      hasUppercase: /[A-Z]/.test(password),
      hasDigit: /\d/.test(password),
    };

    const metCount = Object.values(requirements).filter(Boolean).length;
    const hasLongPassword = password.length >= 12;

    // Score: 0-4
    // - 0-3 based on requirements met
    // - +1 bonus for long password (if all requirements met)
    const score = metCount === 3 && hasLongPassword ? 4 : metCount;

    const levels: PasswordStrengthLevel[] = ['weak', 'weak', 'fair', 'good', 'strong'];

    return {
      score,
      level: levels[score],
      requirements,
      isValid: metCount === 3,
    };
  }, [password]);
}
