import React, { useState } from 'react';

interface EditToDoFormProps {
  editTodo: (text: string, id: number) => void;
  task: { text: string; id: number };
}

export const EditToDoForm: React.FC<EditToDoFormProps> = ({ editTodo, task }) => {
  const [value, setValue] = useState<string>(task.text);

  const handleSubmit = (e: React.FormEvent) => {
    // Prevent default action
    e.preventDefault();
    // Edit todo
    editTodo(value, task.id);
  };

  return (
    <form onSubmit={handleSubmit} className="ToDoForm">
      <input
        type="text"
        value={value}
        onChange={(e) => setValue(e.target.value)}
        className="todo-input"
        placeholder="Update task"
      />
      <button type="submit" className="todo-btn">
        Update Task
      </button>
    </form>
  );
};