const { defineConfig } = require('cypress');

module.exports = defineConfig({
  reporter: 'cypress-mochawesome-reporter',
  reporterOptions: {
    charts: true,
    reportDir: "cypress/reports",
    reportPageTitle: 'Cypress Test Report',
    embeddedScreenshots: true,
    inlineAssets: true
  },
  screenshotsFolder: "cypress/reports/screenshots",
  e2e: {
    baseUrl: 'http://localhost:3000',
    supportFile: 'cypress/support/e2e.ts',
    setupNodeEvents(on: any, config: any) {
      require('cypress-mochawesome-reporter/plugin')(on);
    }
  },
});
