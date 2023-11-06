import './commands';
import 'cypress-mochawesome-reporter/register';
import addContext from "mochawesome/addContext";

require('cypress-xpath');

Cypress.on("test:after:run", (test, runnable: any) => {
    if (test.state === "failed") {
      const screenshot = `reports/assets/${Cypress.spec.name}/${runnable.parent.title} -- ${test.title}.png`;
      addContext({test}, screenshot);
    }
});