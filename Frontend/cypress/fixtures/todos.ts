export interface TodoItem {
  id: number;
  text: string;
  completed: boolean;
}

export const defaultTodos: TodoItem[] = [
  { id: 1, text: 'Buy milk', completed: false },
  { id: 2, text: 'Write tests', completed: true },
];

export const emptyTodos: TodoItem[] = [];
