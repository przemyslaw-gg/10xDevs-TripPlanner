import type { RememberMeCheckboxProps } from '../../@types';

/**
 * Remember me checkbox with label
 */
export function RememberMeCheckbox({
  id,
  checked,
  onChange,
  disabled = false,
}: RememberMeCheckboxProps) {
  return (
    <div className="flex items-center">
      <input
        type="checkbox"
        id={id}
        checked={checked}
        onChange={(e) => onChange(e.target.checked)}
        disabled={disabled}
        className="
          h-4 w-4 rounded border-gray-300
          text-blue-600 focus:ring-blue-500
          disabled:cursor-not-allowed disabled:opacity-50
        "
      />
      <label
        htmlFor={id}
        className={`
          ml-2 text-sm text-gray-700
          ${disabled ? 'cursor-not-allowed opacity-50' : 'cursor-pointer'}
        `}
      >
        Zapamiętaj mnie
      </label>
    </div>
  );
}
