import selectors from "../support/selectors.js";
import { generateRandomString } from "../support/helpers";

describe("ToDo App Tests", () => {
  beforeEach(() => {
    cy.visit("/");
  });

  it("Checks App header", () => {
    cy.contains("h1", "ToDo App");
  });

  it("Adds a new ToDo item", () => {
    var newToDo = generateRandomString();
    cy.get(selectors.newToDoInput).type(newToDo);
    3;
    cy.get(selectors.addToDoButton).click();
    cy.get("ul li")
      .last()
      .should("contain", newToDo);
    cy.get(`li:contains('${newToDo}') button`).click();
    cy.get(`li:contains('${newToDo}')`).should("not.exist");
  });
});
