/// <reference types="cypress" />

describe('template spec', () => {
  it('passes', () => {
    cy.visit('http://localhost:3000')
    cy.get(".ToDoApp").should("have.text", "bla");
  })
})