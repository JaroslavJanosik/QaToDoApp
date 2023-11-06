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
    setupNodeEvents(on: any, config: any) {
      require('cypress-mochawesome-reporter/plugin')(on);
    }
  },
});
