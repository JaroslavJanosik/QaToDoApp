import React, { useCallback, useEffect, useMemo, useState } from "react";
import axios, { AxiosInstance } from "axios";
import ToDoItem from "./ToDoItem";
import { ToDoForm } from "./ToDoForm";
import { EditToDoForm } from "./EditToDoForm";
import { API_URL } from "../config";


interface ApiResponse<T> {
  statusCode: number;
  isSuccess: boolean;
  errorMessages: string[];
  result: T;
}

interface ToDoItemDto {
  id: number;
  text: string;
  completed: boolean;
}

type ToDoItemModel = ToDoItemDto & { isEditing?: boolean };

const makeApi = (): AxiosInstance =>
  axios.create({
    baseURL: API_URL,
    headers: { "Content-Type": "application/json" },
  });

const unwrap = <T,>(resp: ApiResponse<T>): T => {
  if (!resp.isSuccess) {
    const err = resp.errorMessages?.join("; ") || "Unknown API error";
    throw new Error(err);
  }
  return resp.result;
};

export const ToDoWrapper: React.FC = () => {
  const api = useMemo(() => makeApi(), []); // stable instance

  const [todos, setTodos] = useState<ToDoItemModel[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // Initial load
  useEffect(() => {
    const controller = new AbortController();
    (async () => {
      try {
        setLoading(true);
        setError(null);
        const { data } = await api.get<ApiResponse<ToDoItemDto[]>>(
          "/api/ToDoItems",
          { signal: controller.signal }
        );
        const items = unwrap(data).map((t) => ({ ...t, isEditing: false }));
        setTodos(items);
      } catch (e: any) {
        if (axios.isCancel(e)) return;
        console.error("Error fetching todos:", e);
        setError(e?.message ?? "Failed to load todos.");
      } finally {
        setLoading(false);
      }
    })();
    return () => controller.abort();
  }, [api]);

  // Add
  const addTodo = useCallback(
    async (text: string) => {
      try {
        setError(null);
        const { data } = await api.post<ApiResponse<ToDoItemDto>>(
          "/api/ToDoItems",
          { text }
        );
        const created = unwrap(data);
        setTodos((prev) => [...prev, { ...created, isEditing: false }]);
      } catch (e: any) {
        console.error("Error creating todo:", e);
        setError(e?.message ?? "Failed to create todo.");
      }
    },
    [api]
  );

  // Delete (optimistic with rollback)
  const deleteTodo = useCallback(
    async (id: number) => {
      const snapshot = todos; // for rollback
      setTodos((prev) => prev.filter((t) => t.id !== id));
      try {
        await api.delete(`/api/ToDoItems/${id}`);
      } catch (e: any) {
        console.error("Error deleting todo:", e);
        setError(e?.message ?? "Failed to delete todo.");
        setTodos(snapshot); // rollback
      }
    },
    [api, todos]
  );

  // Toggle complete (optimistic with rollback)
  const toggleComplete = useCallback(
    async (id: number) => {
      const snapshot = todos;
      let updated: ToDoItemModel | undefined;
      setTodos((prev) =>
        prev.map((t) => {
          if (t.id !== id) return t;
          updated = { ...t, completed: !t.completed };
          return updated;
        })
      );

      try {
        if (!updated) return;
        // send the full server DTO (without isEditing)
        const payload: ToDoItemDto = {
          id: updated.id,
          text: updated.text,
          completed: updated.completed,
        };
        await api.put(`/api/ToDoItems/${id}`, payload);
      } catch (e: any) {
        console.error("Error updating todo:", e);
        setError(e?.message ?? "Failed to update todo.");
        setTodos(snapshot); // rollback
      }
    },
    [api, todos]
  );

  // Toggle edit mode (UI-only)
  const editTodo = useCallback((id: number) => {
    setTodos((prev) =>
      prev.map((t) => (t.id === id ? { ...t, isEditing: !t.isEditing } : t))
    );
  }, []);

  // Save edited text (optimistic with rollback)
  const editTask = useCallback(
    async (newText: string, id: number) => {
      const snapshot = todos;
      let original: ToDoItemModel | undefined = todos.find((t) => t.id === id);

      // Optimistically update UI
      setTodos((prev) =>
        prev.map((t) =>
          t.id === id ? { ...t, text: newText, isEditing: false } : t
        )
      );

      try {
        if (!original) return;
        const payload: ToDoItemDto = {
          id,
          text: newText,
          completed: original.completed, // preserve completion state
        };
        const { data } = await api.put<ApiResponse<ToDoItemDto>>(
          `/api/ToDoItems/${id}`,
          payload
        );
        const saved = unwrap(data);
        // Ensure UI matches server response
        setTodos((prev) =>
          prev.map((t) =>
            t.id === id ? { ...t, ...saved, isEditing: false } : t
          )
        );
      } catch (e: any) {
        console.error("Error updating task:", e);
        setError(e?.message ?? "Failed to update task.");
        setTodos(snapshot); // rollback
      }
    },
    [api, todos]
  );

  return (
    <div className="ToDoWrapper">
      <h1>ToDo List</h1>

      <ToDoForm addTodo={addTodo} />

      {loading && <p>Loading…</p>}
      {error && <p role="alert">{error}</p>}

      {!loading &&
        todos.map((todo) =>
          todo.isEditing ? (
            <EditToDoForm
              key={todo.id}
              editTodo={editTask}
              task={todo}
            />
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