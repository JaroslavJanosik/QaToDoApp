import { defaultTodos, emptyTodos } from '../fixtures/todos';
import { TodoPage } from '../pages/TodoPage';

describe('ToDo app', () => {
  const page = new TodoPage();

  beforeEach(() => {
    page.interceptGetTodos(defaultTodos);
    page.visit();
    page.waitFor('getTodos');
  });

  it('renders fetched todos and summary', () => {
    page.title().should('be.visible').and('contain', 'ToDo List');

    page.assertSummaryContains('2 total');
    page.assertTodoVisible(1, 'Buy milk');
    page.assertTodoVisible(2, 'Write tests');
  });

  it('creates a todo with trimmed text', () => {
    page.interceptCreateTodo(
      { id: 3, text: 'New task', completed: false },
      'New task',
    );

    page.createTodo('   New task   ');

    page.waitFor('createTodo');
    page.assertTodoVisible(3, 'New task');
    page.input().should('have.value', '');
    page.assertSummaryContains('3 total');
  });

  it('toggles a todo completion state', () => {
    page.interceptUpdateTodo(
      1,
      { id: 1, text: 'Buy milk', completed: true },
      { id: 1, text: 'Buy milk', completed: true },
    );

    page.toggleTodo(1);

    page.waitFor('updateTodo');
    page.toggleButton(1).should('have.class', 'completed');
    page.assertSummaryContains('2 completed');
  });

  it('edits a todo with trimmed text', () => {
    page.interceptUpdateTodo(
      1,
      { id: 1, text: 'Buy oat milk', completed: false },
      { id: 1, text: 'Buy oat milk', completed: false },
    );

    page.editTodo(1, '  Buy oat milk  ');

    page.waitFor('updateTodo');
    page.assertTodoVisible(1, 'Buy oat milk');
  });

  it('deletes a todo', () => {
    page.interceptDeleteTodo(1);

    page.deleteTodo(1);

    page.waitFor('deleteTodo');
    page.item(1).should('not.exist');
    page.assertSummaryContains('1 total');
  });

  it('shows the empty state when no todos are returned', () => {
    page.interceptGetTodos(emptyTodos, 'getEmptyTodos');
    page.visit();
    page.waitFor('getEmptyTodos');

    page.emptyState().should('be.visible').and('contain', 'No tasks yet');
  });
});
