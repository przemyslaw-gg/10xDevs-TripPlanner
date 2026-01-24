import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import type { RegisterFormData } from '../../@types';
import { useAuthContext, parseAuthError } from '../../contexts/AuthContext';
import { usePasswordStrength } from '../../hooks/usePasswordStrength';
import { PasswordInput } from './PasswordInput';
import { PasswordStrengthIndicator } from './PasswordStrengthIndicator';
import { PasswordRequirements } from './PasswordRequirements';

const registerSchema = z
  .object({
    email: z
      .string()
      .min(1, 'Email jest wymagany')
      .email('Nieprawidłowy format email'),
    displayName: z
      .string()
      .min(1, 'Nazwa wyświetlana jest wymagana')
      .max(100, 'Nazwa może mieć max 100 znaków'),
    password: z
      .string()
      .min(1, 'Hasło jest wymagane')
      .min(8, 'Hasło musi mieć min 8 znaków')
      .regex(/[A-Z]/, 'Hasło musi zawierać wielką literę')
      .regex(/\d/, 'Hasło musi zawierać cyfrę'),
    confirmPassword: z
      .string()
      .min(1, 'Potwierdzenie hasła jest wymagane'),
  })
  .refine((data) => data.password === data.confirmPassword, {
    message: 'Hasła muszą być identyczne',
    path: ['confirmPassword'],
  });

interface RegisterFormProps {
  onSuccess?: () => void;
}

export function RegisterForm({ onSuccess }: RegisterFormProps) {
  const { register: registerUser } = useAuthContext();
  const [apiError, setApiError] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    watch,
    setValue,
    formState: { errors, isSubmitting },
  } = useForm<RegisterFormData>({
    resolver: zodResolver(registerSchema),
    defaultValues: {
      email: '',
      displayName: '',
      password: '',
      confirmPassword: '',
    },
    mode: 'onBlur',
  });

  const password = watch('password');
  const confirmPassword = watch('confirmPassword');
  const passwordStrength = usePasswordStrength(password);

  const onSubmit = async (data: RegisterFormData) => {
    setApiError(null);

    try {
      await registerUser({
        email: data.email,
        password: data.password,
        displayName: data.displayName,
      });
      onSuccess?.();
    } catch (error) {
      setApiError(parseAuthError(error));
    }
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">
      {/* API Error Alert */}
      {apiError && (
        <div
          className="p-4 rounded-md bg-red-50 border border-red-200"
          role="alert"
        >
          <p className="text-sm text-red-700">{apiError}</p>
        </div>
      )}

      {/* Email Field */}
      <div className="space-y-1">
        <label
          htmlFor="email"
          className="block text-sm font-medium text-gray-700"
        >
          Email
        </label>
        <input
          type="email"
          id="email"
          autoComplete="email"
          autoFocus
          {...register('email')}
          aria-invalid={!!errors.email}
          aria-describedby={errors.email ? 'email-error' : undefined}
          className={`
            block w-full px-3 py-2 rounded-md shadow-sm border
            focus:outline-none focus:ring-2 focus:ring-offset-0
            ${errors.email
              ? 'border-red-500 focus:border-red-500 focus:ring-red-500'
              : 'border-gray-300 focus:border-blue-500 focus:ring-blue-500'
            }
          `}
        />
        {errors.email && (
          <p id="email-error" className="text-sm text-red-600" role="alert">
            {errors.email.message}
          </p>
        )}
      </div>

      {/* Display Name Field */}
      <div className="space-y-1">
        <label
          htmlFor="displayName"
          className="block text-sm font-medium text-gray-700"
        >
          Nazwa wyświetlana
        </label>
        <input
          type="text"
          id="displayName"
          autoComplete="name"
          {...register('displayName')}
          aria-invalid={!!errors.displayName}
          aria-describedby={errors.displayName ? 'displayName-error' : undefined}
          className={`
            block w-full px-3 py-2 rounded-md shadow-sm border
            focus:outline-none focus:ring-2 focus:ring-offset-0
            ${errors.displayName
              ? 'border-red-500 focus:border-red-500 focus:ring-red-500'
              : 'border-gray-300 focus:border-blue-500 focus:ring-blue-500'
            }
          `}
        />
        {errors.displayName && (
          <p id="displayName-error" className="text-sm text-red-600" role="alert">
            {errors.displayName.message}
          </p>
        )}
      </div>

      {/* Password Field */}
      <div className="space-y-2">
        <PasswordInput
          id="password"
          name="password"
          label="Hasło"
          value={password}
          onChange={(e) => setValue('password', e.target.value, { shouldValidate: true })}
          error={errors.password?.message}
          autoComplete="new-password"
          disabled={isSubmitting}
        />

        {/* Password Strength Indicator */}
        {password.length > 0 && (
          <PasswordStrengthIndicator strength={passwordStrength} />
        )}

        {/* Password Requirements */}
        <PasswordRequirements requirements={passwordStrength.requirements} />
      </div>

      {/* Confirm Password Field */}
      <PasswordInput
        id="confirmPassword"
        name="confirmPassword"
        label="Potwierdź hasło"
        value={confirmPassword}
        onChange={(e) => setValue('confirmPassword', e.target.value, { shouldValidate: true })}
        error={errors.confirmPassword?.message}
        autoComplete="new-password"
        disabled={isSubmitting}
      />

      {/* Submit Button */}
      <button
        type="submit"
        disabled={isSubmitting}
        className="
          w-full flex justify-center py-2.5 px-4
          border border-transparent rounded-md shadow-sm
          text-sm font-medium text-white
          bg-blue-600 hover:bg-blue-700
          focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500
          disabled:opacity-50 disabled:cursor-not-allowed
          transition-colors duration-150
        "
      >
        {isSubmitting ? (
          <span className="flex items-center gap-2">
            <svg
              className="animate-spin h-4 w-4"
              xmlns="http://www.w3.org/2000/svg"
              fill="none"
              viewBox="0 0 24 24"
            >
              <circle
                className="opacity-25"
                cx="12"
                cy="12"
                r="10"
                stroke="currentColor"
                strokeWidth="4"
              />
              <path
                className="opacity-75"
                fill="currentColor"
                d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
              />
            </svg>
            Rejestracja...
          </span>
        ) : (
          'Zarejestruj się'
        )}
      </button>
    </form>
  );
}
