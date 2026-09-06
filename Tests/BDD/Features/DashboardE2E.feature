@e2e
Feature: Dashboard E2E Tests
  As a test automation engineer
  I want to verify dashboard functionality through the UI
  So that end-to-end user flows are covered

  Background:
	Given the user is authenticated and logged in to the application

  Rule: Dashboard lifecycle via UI

	Scenario: User can create a dashboard via UI
	  Given a unique dashboard name is prepared with base "E2E Dashboard"
	  When the user creates the dashboard via UI with description "Created via UI E2E test"
	  Then the prepared dashboard should be visible

	Scenario: User can remove a dashboard via UI
	  Given a dashboard is created via API with base name "E2E Dashboard"
	  When the user removes the prepared dashboard via UI
	  Then the prepared dashboard should not be visible on home page

	Scenario: User can edit a dashboard via UI
	  Given a dashboard is created via API with base name "E2E Dashboard"
	  And an updated dashboard name is prepared from the current dashboard name
	  When the user edits the prepared dashboard via UI with description "Updated via UI E2E test"
	  Then the updated dashboard should be visible on details page

  Rule: Widget lifecycle via UI

	Scenario: User can add a widget to a dashboard
	  Given a default dashboard is created via API
	  And a unique widget name is prepared with base "E2E Widget"
	  When the user adds widget type "overallStatistics" with the prepared widget name and description "Widget added via UI E2E test"
	  Then the prepared widget should be visible

	Scenario: User can change widget order on a dashboard
	  Given a default dashboard is created via API
	  And two widgets are created via API for reorder with bases "E2E Widget A" and "E2E Widget B"
	  When the user reorders the second prepared widget before the first prepared widget
	  Then the second prepared widget should appear before the first prepared widget

	Scenario: User can remove a widget from a dashboard
	  Given a default dashboard is created via API
	  And a default widget is created via API with base name "E2E Widget"
	  When the user removes the prepared widget via UI
	  Then the prepared widget should not be visible
