import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import type { LoginFormData } from '../../@types';
import { useAuthContext, parseAuthError } from '../../contexts/AuthContext';
import { PasswordInput } from './PasswordInput';
import { RememberMeCheckbox } from './RememberMeCheckbox';

const loginSchema = z.object({
  email: z
    .string()
    .min(1, 'Email jest wymagany')
    .email('Nieprawidłowy format email'),
  password: z
    .string()
    .min(1, 'Hasło jest wymagane'),
  rememberMe: z.boolean(),
});

interface LoginFormProps {
  onSuccess?: () => void;
}

export function LoginForm({ onSuccess }: LoginFormProps) {
  const { login } = useAuthContext();
  const [apiError, setApiError] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    watch,
    setValue,
    formState: { errors, isSubmitting },
  } = useForm<LoginFormData>({
    resolver: zodResolver(loginSchema),
    defaultValues: {
      email: '',
      password: '',
      rememberMe: false,
    },
  });

  const password = watch('password');
  const rememberMe = watch('rememberMe');

  const onSubmit = async (data: LoginFormData) => {
    setApiError(null);

    try {
      await login(
        { email: data.email, password: data.password },
        data.rememberMe
      );
      onSuccess?.();
    } catch (error) {
      setApiError(parseAuthError(error));
    }
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
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

      {/* Password Field */}
      <PasswordInput
        id="password"
        name="password"
        label="Hasło"
        value={password}
        onChange={(e) => setValue('password', e.target.value, { shouldValidate: true })}
        error={errors.password?.message}
        autoComplete="current-password"
        disabled={isSubmitting}
      />

      {/* Remember Me & Forgot Password */}
      <div className="flex items-center justify-between">
        <RememberMeCheckbox
          id="rememberMe"
          checked={rememberMe}
          onChange={(checked) => setValue('rememberMe', checked)}
          disabled={isSubmitting}
        />
      </div>

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
            Logowanie...
          </span>
        ) : (
          'Zaloguj się'
        )}
      </button>
    </form>
  );
}
