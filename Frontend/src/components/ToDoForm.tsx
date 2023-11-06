import React, { useState, ChangeEvent, FormEvent } from 'react';

interface ToDoFormProps {
  addTodo: (value: string) => void;
}

export const ToDoForm: React.FC<ToDoFormProps> = ({ addTodo }) => {
  const [value, setValue] = useState<string>('');

  const handleSubmit = (e: FormEvent<HTMLFormElement>): void => {
    e.preventDefault();
    if (value) {
      addTodo(value);
      setValue('');
    }
  };

  const handleInputChange = (e: ChangeEvent<HTMLInputElement>): void => {
    setValue(e.target.value);
  };

  return (
    <form onSubmit={handleSubmit} className="ToDoForm">
      <input
        type="text"
        value={value}
        onChange={handleInputChange}
        className="todo-input"
        placeholder="What is the task today?"
      />
      <button type="submit" className="todo-btn">
        Add Task
      </button>
    </form>
  );
};
