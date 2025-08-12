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
    <div className="ToDoItem">
      <p className={task.completed ? "completed" : "incompleted"} onClick={() => toggleComplete(task.id)}>
        {task.text}
      </p>
      <div>
        <FontAwesomeIcon className="edit-icon" icon={faPenToSquare} onClick={() => editTodo(task.id)} />
        <FontAwesomeIcon className="delete-icon" icon={faTrash} onClick={() => deleteTodo(task.id)} />
      </div>
    </div>
  );
}

export default ToDoItem;