import React, { useState } from 'react';

interface EditToDoFormProps {
  editTodo: (text: string, id: number) => Promise<void>;
  task: { text: string; id: number };
}

export const EditToDoForm: React.FC<EditToDoFormProps> = ({ editTodo, task }) => {
  const [value, setValue] = useState<string>(task.text);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const normalizedValue = value.trim();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!normalizedValue || isSubmitting) {
      return;
    }

    try {
      setIsSubmitting(true);
      await editTodo(normalizedValue, task.id);
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="ToDoForm" data-testid={`todo-edit-form-${task.id}`}>
      <input
        type="text"
        value={value}
        onChange={(e) => setValue(e.target.value)}
        className="todo-input"
        placeholder="Update task"
        maxLength={500}
        aria-label={`Edit task ${task.id}`}
        data-testid={`todo-edit-input-${task.id}`}
      />
      <button
        type="submit"
        className="todo-btn"
        disabled={!normalizedValue || isSubmitting}
        data-testid={`todo-edit-submit-${task.id}`}
      >
        {isSubmitting ? 'Saving…' : 'Update Task'}
      </button>
    </form>
  );
};
