import { useState, useRef, useEffect } from 'react';
import type { EditableTitleProps } from '../../@types';
import { CharacterCounter } from './CharacterCounter';

/**
 * Inline editable title component
 * Click to edit, Enter/blur to save, Escape to cancel
 */
export function EditableTitle({
  value,
  onChange,
  maxLength = 100,
  placeholder = 'Nazwa wycieczki',
}: EditableTitleProps) {
  const [isEditing, setIsEditing] = useState(false);
  const [editValue, setEditValue] = useState(value);
  const inputRef = useRef<HTMLInputElement>(null);

  // Focus input when entering edit mode
  useEffect(() => {
    if (isEditing && inputRef.current) {
      inputRef.current.focus();
      inputRef.current.select();
    }
  }, [isEditing]);

  // Update edit value when prop changes (e.g., after save)
  useEffect(() => {
    if (!isEditing) {
      setEditValue(value);
    }
  }, [value, isEditing]);

  const handleStartEdit = () => {
    setEditValue(value);
    setIsEditing(true);
  };

  const handleSave = () => {
    const trimmedValue = editValue.trim();
    if (trimmedValue && trimmedValue !== value) {
      onChange(trimmedValue);
    } else {
      setEditValue(value); // Revert if empty
    }
    setIsEditing(false);
  };

  const handleCancel = () => {
    setEditValue(value);
    setIsEditing(false);
  };

  const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === 'Enter') {
      e.preventDefault();
      handleSave();
    } else if (e.key === 'Escape') {
      e.preventDefault();
      handleCancel();
    }
  };

  if (isEditing) {
    return (
      <div className="flex-1">
        <div className="flex items-center gap-2">
          <input
            ref={inputRef}
            type="text"
            value={editValue}
            onChange={(e) => setEditValue(e.target.value)}
            onBlur={handleSave}
            onKeyDown={handleKeyDown}
            maxLength={maxLength}
            placeholder={placeholder}
            className="
              flex-1 text-2xl font-bold text-gray-900
              px-2 py-1 -mx-2 -my-1
              border-2 border-blue-500 rounded-md
              focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-1
            "
            aria-label="Edytuj nazwę wycieczki"
          />
          <CharacterCounter
            current={editValue.length}
            max={maxLength}
            warningThreshold={80}
          />
        </div>
        <p className="text-xs text-gray-500 mt-1">
          Enter aby zapisać, Escape aby anulować
        </p>
      </div>
    );
  }

  return (
    <button
      onClick={handleStartEdit}
      className="
        group flex items-center gap-2
        text-left focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 rounded-md
      "
      aria-label="Kliknij aby edytować nazwę wycieczki"
    >
      <h1 className="text-2xl font-bold text-gray-900">
        {value || <span className="text-gray-400">{placeholder}</span>}
      </h1>
      <svg
        className="w-5 h-5 text-gray-400 opacity-0 group-hover:opacity-100 group-focus:opacity-100 transition-opacity"
        fill="none"
        stroke="currentColor"
        viewBox="0 0 24 24"
        aria-hidden="true"
      >
        <path
          strokeLinecap="round"
          strokeLinejoin="round"
          strokeWidth={2}
          d="M15.232 5.232l3.536 3.536m-2.036-5.036a2.5 2.5 0 113.536 3.536L6.5 21.036H3v-3.572L16.732 3.732z"
        />
      </svg>
    </button>
  );
}
