import React, { useState, ChangeEvent, FormEvent } from 'react';

interface ToDoFormProps {
  addTodo: (value: string) => Promise<void>;
}

export const ToDoForm: React.FC<ToDoFormProps> = ({ addTodo }) => {
  const [value, setValue] = useState<string>('');
  const [isSubmitting, setIsSubmitting] = useState(false);

  const normalizedValue = value.trim();

  const handleSubmit = async (e: FormEvent<HTMLFormElement>): Promise<void> => {
    e.preventDefault();

    if (!normalizedValue || isSubmitting) {
      return;
    }

    try {
      setIsSubmitting(true);
      await addTodo(normalizedValue);
      setValue('');
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleInputChange = (e: ChangeEvent<HTMLInputElement>): void => {
    setValue(e.target.value);
  };

  return (
    <form onSubmit={handleSubmit} className="ToDoForm" data-testid="todo-form">
      <input
        type="text"
        value={value}
        onChange={handleInputChange}
        className="todo-input"
        placeholder="What is the task today?"
        maxLength={500}
        aria-label="New task"
        data-testid="todo-input"
      />
      <button
        type="submit"
        className="todo-btn"
        disabled={!normalizedValue || isSubmitting}
        data-testid="todo-submit"
      >
        {isSubmitting ? 'Adding…' : 'Add Task'}
      </button>
    </form>
  );
};
