import type { TodoItem } from '../fixtures/todos';

interface ApiEnvelope<T> {
  statusCode: number;
  isSuccess: boolean;
  errorMessages: string[];
  result: T;
}

export class TodoPage {
  visit(): void {
    cy.visit('/');
  }

  interceptGetTodos(items: TodoItem[], alias = 'getTodos'): void {
    cy.intercept('GET', '**/api/ToDoItems', {
      statusCode: 200,
      body: this.buildResponse(items),
    }).as(alias);
  }

  interceptCreateTodo(createdTodo: TodoItem, expectedText: string, alias = 'createTodo'): void {
    cy.intercept('POST', '**/api/ToDoItems', (req) => {
      expect(req.body).to.deep.equal({ text: expectedText });

      req.reply({
        statusCode: 200,
        body: this.buildResponse(createdTodo),
      });
    }).as(alias);
  }

  interceptUpdateTodo(
    id: number,
    expectedBody: TodoItem,
    updatedTodo: TodoItem,
    alias = 'updateTodo',
  ): void {
    cy.intercept('PUT', `**/api/ToDoItems/${id}`, (req) => {
      expect(req.body).to.deep.equal(expectedBody);

      req.reply({
        statusCode: 200,
        body: this.buildResponse(updatedTodo),
      });
    }).as(alias);
  }

  interceptDeleteTodo(id: number, alias = 'deleteTodo'): void {
    cy.intercept('DELETE', `**/api/ToDoItems/${id}`, {
      statusCode: 204,
      body: '',
    }).as(alias);
  }

  waitFor(alias: string): void {
    cy.wait(`@${alias}`);
  }

  title(): Cypress.Chainable<JQuery<HTMLElement>> {
    return cy.getByTestId('todo-title');
  }

  summary(): Cypress.Chainable<JQuery<HTMLElement>> {
    return cy.getByTestId('todo-summary');
  }

  input(): Cypress.Chainable<JQuery<HTMLElement>> {
    return cy.getByTestId('todo-input');
  }

  submitButton(): Cypress.Chainable<JQuery<HTMLElement>> {
    return cy.getByTestId('todo-submit');
  }

  emptyState(): Cypress.Chainable<JQuery<HTMLElement>> {
    return cy.getByTestId('todo-empty-state');
  }

  item(id: number): Cypress.Chainable<JQuery<HTMLElement>> {
    return cy.getByTestId(`todo-item-${id}`);
  }

  toggleButton(id: number): Cypress.Chainable<JQuery<HTMLElement>> {
    return cy.getByTestId(`todo-toggle-${id}`);
  }

  editButton(id: number): Cypress.Chainable<JQuery<HTMLElement>> {
    return cy.getByTestId(`todo-edit-${id}`);
  }

  deleteButton(id: number): Cypress.Chainable<JQuery<HTMLElement>> {
    return cy.getByTestId(`todo-delete-${id}`);
  }

  editInput(id: number): Cypress.Chainable<JQuery<HTMLElement>> {
    return cy.getByTestId(`todo-edit-input-${id}`);
  }

  editSubmitButton(id: number): Cypress.Chainable<JQuery<HTMLElement>> {
    return cy.getByTestId(`todo-edit-submit-${id}`);
  }

  createTodo(text: string): void {
    this.input().clear().type(text);
    this.submitButton().click();
  }

  toggleTodo(id: number): void {
    this.toggleButton(id).click();
  }

  editTodo(id: number, text: string): void {
    this.editButton(id).click();
    this.editInput(id).clear().type(text);
    this.editSubmitButton(id).click();
  }

  deleteTodo(id: number): void {
    this.deleteButton(id).click();
  }

  assertTodoVisible(id: number, text: string): void {
    this.item(id).should('be.visible').and('contain', text);
  }

  assertSummaryContains(text: string): void {
    this.summary().should('be.visible').and('contain', text);
  }

  private buildResponse<T>(result: T): ApiEnvelope<T> {
    return {
      statusCode: 200,
      isSuccess: true,
      errorMessages: [],
      result,
    };
  }
}
