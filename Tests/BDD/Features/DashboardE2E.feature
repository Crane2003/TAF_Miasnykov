@e2e
Feature: Dashboard E2E Tests
  As a test automation engineer
  I want to verify dashboard functionality through the UI
  So that end-to-end user flows are covered

  Background:
	Given the user is authenticated and logged in to the application

  Rule: Dashboard page displays required UI elements

	Scenario: Dashboard page loads successfully
	  When the user navigates to the dashboards page
	  Then the dashboard page should be loaded successfully

	Scenario: Add New Dashboard button is displayed on the page
	  When the user navigates to the dashboards page
	  Then the Add New Dashboard button should be visible

	Scenario: Add New Widget button is displayed on the page
	  When the user navigates to the dashboards page
	  Then the Add New Widget button should be visible

  Rule: Dashboards created via API are reflected in the UI

	Scenario: A dashboard created via API appears in the dashboard list
	  Given a new dashboard is created via the API with name "E2E BDD Dashboard"
	  When the user navigates to the dashboards page
	  Then the dashboard named "E2E BDD Dashboard" should be visible in the list
