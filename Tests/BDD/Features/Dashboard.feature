@api
Feature: Dashboard API BDD Tests
  As a test automation engineer
  I want to manage dashboards via the API
  So that I can verify the dashboard functionality end-to-end

  Background:
	Given the dashboard API service is configured with valid authentication

  Rule: Dashboards can be created with various input combinations

	Scenario Outline: Create a dashboard with different names and descriptions
	  Given I have a create request with name "<Name>" and description "<Description>"
	  When I submit the create dashboard request
	  Then the response should indicate successful creation
	  And the returned dashboard name should be "<Name>"
	  And the returned dashboard description should be "<Description>"

	  Examples:
		| Name                          | Description                                      |
		| Short Dashboard               | Short desc                                       |
		| Dashboard #007 v2.0           | Dashboard with numeric identifiers               |
		| CI/CD Pipeline & Build Status | Tracks build success/failure and deployment rate |
		| Dashboard Unicode — テスト     | Dashboard with Unicode characters in name        |

  Rule: Dashboards can be updated after creation

	Scenario Outline: Update a dashboard name and description
	  Given I have a create request with name "<InitialName>" and description "Initial description"
	  And I have created the dashboard
	  When I update the dashboard with name "<UpdatedName>" and description "<UpdatedDescription>"
	  Then the dashboard update should succeed
	  And the dashboard name should now be "<UpdatedName>"
	  But the dashboard name should not be "<InitialName>"

	  Examples:
		| InitialName                | UpdatedName                     | UpdatedDescription                |
		| BDD Update Test - Basic    | Renamed BDD Dashboard           | Automated BDD test dashboard      |
		| BDD Update Test - Short    | X                               | Y                                 |
		| BDD Update Test - Special  | Updated: CI/CD & Monitor (v3.0) | Special chars & "quotes" 'apos'   |

  Rule: Widgets can be added to dashboards

	Scenario: Add multiple widgets to a dashboard using a data table
	  Given I have created a dashboard named "BDD Widget Table Dashboard"
	  When I add the following widgets to the dashboard:
		| Name                     | Type              | Width | Height | PositionX | PositionY |
		| Overall Statistics Panel | overallStatistics | 6     | 7      | 0         | 0         |
		| Launch Statistics Chart  | chart             | 6     | 4      | 6         | 0         |
		| Failed Tests Table       | table             | 12    | 6      | 0         | 7         |
	  Then the dashboard should contain 3 widgets
	  And each widget should have the correct name and type

	Scenario: Create and verify dashboards from a list of names
	  Given I want to create dashboards with the following names:
		| Name                      |
		| BDD List Dashboard Alpha  |
		| BDD List Dashboard Beta   |
		| BDD List Dashboard Gamma  |
	  When I create each dashboard from the list
	  Then all dashboards from the list should be created successfully

	Scenario: Add a single widget using comma-separated widget names list
	  Given I have created a dashboard named "BDD Widget List Dashboard"
	  When I add widgets with names "Stats Panel,Trend Chart,Failure Table"
	  Then the dashboard should have widgets matching the provided names
