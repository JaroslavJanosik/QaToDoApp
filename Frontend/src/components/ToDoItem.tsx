import React from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faPenToSquare, faTrash } from '@fortawesome/free-solid-svg-icons';

interface ToDoItemProps {
  task: {
    id: number;
    text: string;
    completed: boolean;
  };
  deleteTodo: (id: number) => void;
  editTodo: (id: number) => void;
  toggleComplete: (id: number) => void;
}

const ToDoItem: React.FC<ToDoItemProps> = ({ task, deleteTodo, editTodo, toggleComplete }) => {
  return (
    <div className="ToDoItem" data-testid={`todo-item-${task.id}`}>
      <button
        type="button"
        className={`todo-text-btn ${task.completed ? 'completed' : 'incompleted'}`}
        onClick={() => toggleComplete(task.id)}
        aria-label={`Mark task ${task.text} as ${task.completed ? 'incomplete' : 'complete'}`}
        data-testid={`todo-toggle-${task.id}`}
      >
        <span data-testid={`todo-text-${task.id}`}>{task.text}</span>
      </button>
      <div className="todo-actions" data-testid={`todo-actions-${task.id}`}>
        <button
          type="button"
          className="icon-btn"
          onClick={() => editTodo(task.id)}
          aria-label={`Edit task ${task.text}`}
          data-testid={`todo-edit-${task.id}`}
        >
          <FontAwesomeIcon className="edit-icon" icon={faPenToSquare} />
        </button>
        <button
          type="button"
          className="icon-btn"
          onClick={() => deleteTodo(task.id)}
          aria-label={`Delete task ${task.text}`}
          data-testid={`todo-delete-${task.id}`}
        >
          <FontAwesomeIcon className="delete-icon" icon={faTrash} />
        </button>
      </div>
    </div>
  );
}

export default ToDoItem;
