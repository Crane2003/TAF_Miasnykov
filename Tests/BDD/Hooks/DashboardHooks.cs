using Business.Services;
using Core.Configuration;
using Core.Logging;
using Reqnroll;
using Tests.BDD.Context;

namespace Tests.BDD.Hooks;

[Binding]
public sealed class DashboardHooks
{
    private readonly ScenarioContext _scenarioContext;
    private readonly FeatureContext _featureContext;
    private readonly DashboardContext _dashboardContext;

    public DashboardHooks(
        ScenarioContext scenarioContext,
        FeatureContext featureContext,
        DashboardContext dashboardContext)
    {
        _scenarioContext = scenarioContext;
        _featureContext = featureContext;
        _dashboardContext = dashboardContext;
    }

    [BeforeTestRun]
    public static void BeforeTestRun()
    {
        LoggerSetup.Initialize();
        var logger = Log.ForContext<DashboardHooks>();
        logger.Information("[BDD TestRun] Test run started");
    }

    [AfterTestRun]
    public static void AfterTestRun()
    {
        var logger = Log.ForContext<DashboardHooks>();
        logger.Information("[BDD TestRun] Test run finished");
    }

    [BeforeFeature("api")]
    public static void BeforeFeature(FeatureContext featureContext)
    {
        var logger = Log.ForContext<DashboardHooks>();
        logger.Information("[BDD Feature] Starting feature: {FeatureTitle}", featureContext.FeatureInfo.Title);
    }

    [AfterFeature("api")]
    public static void AfterFeature(FeatureContext featureContext)
    {
        var logger = Log.ForContext<DashboardHooks>();
        logger.Information("[BDD Feature] Finished feature: {FeatureTitle}", featureContext.FeatureInfo.Title);
    }

    [BeforeScenario("api")]
    public void BeforeScenario()
    {
        var logger = Log.ForContext<DashboardHooks>();
        logger.Information("[BDD Scenario] Starting: [{Feature}] {Scenario}",
            _featureContext.FeatureInfo.Title,
            _scenarioContext.ScenarioInfo.Title);

        var configLoader = ConfigurationLoader.Instance;
        var testConfig = configLoader.GetTestConfiguration();

        _dashboardContext.ApiService = new DashboardApiService(testConfig.ProjectName);
    }

    [AfterScenario("api")]
    public async Task AfterScenario()
    {
        var logger = Log.ForContext<DashboardHooks>();

        foreach (var id in _dashboardContext.CreatedDashboardIds)
        {
            try
            {
                await _dashboardContext.ApiService.DeleteDashboardAsync(id);
                logger.Information("[BDD Scenario] Deleted dashboard {DashboardId}", id);
            }
            catch (Exception ex)
            {
                logger.Warning(ex, "[BDD Scenario] Failed to delete dashboard {DashboardId}", id);
            }
        }

        var status = _scenarioContext.ScenarioExecutionStatus;
        logger.Information("[BDD Scenario] Finished: {Scenario} — {Status}", _scenarioContext.ScenarioInfo.Title, status);
    }
}
