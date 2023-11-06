import React, { useState, useEffect } from "react";
import ToDoItem from "./ToDoItem";
import { ToDoForm } from "./ToDoForm";
import { EditToDoForm } from "./EditToDoForm";
import axios from 'axios';
import { API_URL } from '../config';

interface ToDo {
  id: number;
  text: string;
  completed: boolean;
  isEditing: boolean;
}

export const ToDoWrapper: React.FC = () => {
  const [todos, setTodos] = useState<ToDo[]>([]);

  useEffect(() => {
    // Fetch the initial list of todos when the component mounts
    axios.get<ToDo[]>(API_URL + '/api/ToDoItems')
      .then((response) => {
        setTodos(response.data);
      })
      .catch((error) => {
        console.error('Error fetching todos:', error);
      });
  }, []);

  const addTodo = (todo: string) => {
    // Send a POST request to create a new todo
    axios.post<ToDo>(API_URL + '/api/ToDoItems', { text: todo })
      .then((response) => {
        setTodos([...todos, response.data]);
      })
      .catch((error) => {
        console.error('Error creating todo:', error);
      });
  }

  const deleteTodo = (id: number) => {
    // Send a DELETE request to delete a todo
    axios.delete(API_URL + `/api/ToDoItems/${id}`)
      .then(() => {
        setTodos(todos.filter((todo) => todo.id !== id));
      })
      .catch((error) => {
        console.error('Error deleting todo:', error);
      });
  }

  const toggleComplete = (id: number) => {
    // Send a PUT request to update the completion status of a todo
    const todoToUpdate = todos.find((todo) => todo.id === id);
    if (todoToUpdate) {
      axios.put(API_URL + `/api/ToDoItems/${id}`, { completed: !todoToUpdate.completed })
        .then((response) => {
          setTodos(
            todos.map((todo) => (todo.id === id ? { ...todo, completed: !todoToUpdate.completed } : todo))
          );
        })
        .catch((error) => {
          console.error('Error updating todo:', error);
        });
    }
  }  

  const editTodo = (id: number) => {
    // Toggle the editing state of a todo
    setTodos(
      todos.map((todo) =>
        todo.id === id ? { ...todo, isEditing: !todo.isEditing } : todo
      )
    );
  }

  const editTask = (task: string, id: number) => {
    // Send a PUT request to update the task of a todo
    axios.put(API_URL + `/api/ToDoItems/${id}`, { text: task, completed: false })
      .then((response) => {
        // Update the todos state with the modified text
        setTodos(
          todos.map((todo) => (todo.id === id ? { ...todo, text: task, isEditing: false } : todo))
        );
      })
      .catch((error) => {
        console.error('Error updating task:', error);
      });
  };  

  return (
    <div className="ToDoWrapper">
      <h1>ToDo List</h1>
      <ToDoForm addTodo={addTodo} />
      {/* display todos */}
      {todos.map((todo) =>
        todo.isEditing ? (
          <EditToDoForm editTodo={editTask} key={todo.id} task={todo} />
        ) : (
          <ToDoItem
            key={todo.id}
            task={todo}
            deleteTodo={deleteTodo}
            editTodo={editTodo}
            toggleComplete={toggleComplete}
          />
        )
      )}
    </div>
  );
};
