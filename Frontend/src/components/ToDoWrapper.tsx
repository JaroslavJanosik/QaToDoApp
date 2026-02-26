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

const toUiModel = (items: ToDoItemDto[]): ToDoItemModel[] =>
  items.map((item) => ({ ...item, isEditing: false }));

export const ToDoWrapper: React.FC = () => {
  const api = useMemo(() => makeApi(), []);

  const [todos, setTodos] = useState<ToDoItemModel[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchTodos = useCallback(async (signal?: AbortSignal) => {
    const { data } = await api.get<ApiResponse<ToDoItemDto[]>>("/api/ToDoItems", { signal });
    setTodos(toUiModel(unwrap(data)));
  }, [api]);

  useEffect(() => {
    const controller = new AbortController();

    (async () => {
      try {
        setLoading(true);
        setError(null);
        await fetchTodos(controller.signal);
      } catch (e: unknown) {
        if (axios.isCancel(e)) return;
        console.error("Error fetching todos:", e);
        setError(e instanceof Error ? e.message : "Failed to load todos.");
      } finally {
        setLoading(false);
      }
    })();

    return () => controller.abort();
  }, [fetchTodos]);

  const addTodo = useCallback(
    async (text: string) => {
      try {
        setError(null);
        const { data } = await api.post<ApiResponse<ToDoItemDto>>("/api/ToDoItems", { text });
        const created = unwrap(data);
        setTodos((prev) => [...prev, { ...created, isEditing: false }]);
      } catch (e: unknown) {
        console.error("Error creating todo:", e);
        setError(e instanceof Error ? e.message : "Failed to create todo.");
        throw e;
      }
    },
    [api]
  );

  const deleteTodo = useCallback(
    async (id: number) => {
      const snapshot = todos;
      setTodos((prev) => prev.filter((t) => t.id !== id));
      try {
        setError(null);
        await api.delete(`/api/ToDoItems/${id}`);
      } catch (e: unknown) {
        console.error("Error deleting todo:", e);
        setError(e instanceof Error ? e.message : "Failed to delete todo.");
        setTodos(snapshot);
      }
    },
    [api, todos]
  );

  const toggleComplete = useCallback(
    async (id: number) => {
      const snapshot = todos;
      const current = todos.find((todo) => todo.id === id);

      if (!current) {
        return;
      }

      const updated: ToDoItemDto = {
        id: current.id,
        text: current.text,
        completed: !current.completed,
      };

      setTodos((prev) =>
        prev.map((t) => (t.id === id ? { ...t, completed: updated.completed } : t))
      );

      try {
        setError(null);
        await api.put(`/api/ToDoItems/${id}`, updated);
      } catch (e: unknown) {
        console.error("Error updating todo:", e);
        setError(e instanceof Error ? e.message : "Failed to update todo.");
        setTodos(snapshot);
      }
    },
    [api, todos]
  );

  const editTodo = useCallback((id: number) => {
    setTodos((prev) =>
      prev.map((t) => (t.id === id ? { ...t, isEditing: !t.isEditing } : t))
    );
  }, []);

  const editTask = useCallback(
    async (newText: string, id: number) => {
      const snapshot = todos;
      const original = todos.find((t) => t.id === id);

      if (!original) {
        return;
      }

      setTodos((prev) =>
        prev.map((t) => (t.id === id ? { ...t, text: newText, isEditing: false } : t))
      );

      try {
        setError(null);
        const payload: ToDoItemDto = {
          id,
          text: newText,
          completed: original.completed,
        };
        const { data } = await api.put<ApiResponse<ToDoItemDto>>(`/api/ToDoItems/${id}`, payload);
        const saved = unwrap(data);
        setTodos((prev) =>
          prev.map((t) => (t.id === id ? { ...t, ...saved, isEditing: false } : t))
        );
      } catch (e: unknown) {
        console.error("Error updating task:", e);
        setError(e instanceof Error ? e.message : "Failed to update task.");
        setTodos(snapshot);
        throw e;
      }
    },
    [api, todos]
  );

  const completedCount = todos.filter((todo) => todo.completed).length;

  return (
    <div className="ToDoWrapper" data-testid="todo-wrapper">
      <h1 data-testid="todo-title">ToDo List</h1>
      <p className="todo-meta" data-testid="todo-summary">
        {todos.length} total • {completedCount} completed • {todos.length - completedCount} open
      </p>

      <ToDoForm addTodo={addTodo} />

      {loading && <p data-testid="todo-loading">Loading…</p>}
      {error && <p role="alert" className="error-banner" data-testid="todo-error">{error}</p>}
      {!loading && todos.length === 0 && <p className="empty-state" data-testid="todo-empty-state">No tasks yet. Add your first one.</p>}

      {!loading && (
        <div data-testid="todo-list">
          {todos.map((todo) =>
            todo.isEditing ? (
              <EditToDoForm key={todo.id} editTodo={editTask} task={todo} />
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
      )}
    </div>
  );
};
