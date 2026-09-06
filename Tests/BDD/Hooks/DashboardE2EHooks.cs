using Business.Services;
using Core.Configuration;
using Core.Driver;
using Reqnroll;
using Tests.BDD.Context;

namespace Tests.BDD.Hooks;

[Binding]
public sealed class DashboardE2EHooks
{
    private readonly ScenarioContext _scenarioContext;
    private readonly FeatureContext _featureContext;
    private readonly DashboardE2EContext _e2eContext;

    public DashboardE2EHooks(
        ScenarioContext scenarioContext,
        FeatureContext featureContext,
        DashboardE2EContext e2eContext)
    {
        _scenarioContext = scenarioContext;
        _featureContext = featureContext;
        _e2eContext = e2eContext;
    }

    [BeforeFeature("e2e")]
    public static void BeforeFeature(FeatureContext featureContext)
    {
        var logger = Log.ForContext<DashboardE2EHooks>();
        logger.Information("[E2E Feature] Starting feature: {FeatureTitle}", featureContext.FeatureInfo.Title);
    }

    [AfterFeature("e2e")]
    public static void AfterFeature(FeatureContext featureContext)
    {
        var logger = Log.ForContext<DashboardE2EHooks>();
        logger.Information("[E2E Feature] Finished feature: {FeatureTitle}", featureContext.FeatureInfo.Title);
    }

    [BeforeScenario("e2e")]
    public void BeforeScenario()
    {
        var logger = Log.ForContext<DashboardE2EHooks>();
        logger.Information("[E2E Scenario] Starting: [{Feature}] {Scenario}",
            _featureContext.FeatureInfo.Title,
            _scenarioContext.ScenarioInfo.Title);

        var configLoader = ConfigurationLoader.Instance;
        var testConfig = configLoader.GetTestConfiguration();
        var credentials = configLoader.GetCredentials();

        UiTestSupport.InitDriver(testConfig);

        _e2eContext.ApiService = new DashboardApiService(testConfig.ProjectName);
        _e2eContext.UiService = new DashboardUiService(testConfig.BaseUrl);
        _e2eContext.AuthService = new AuthenticationService(testConfig.BaseUrl);

        _e2eContext.AuthService.NavigateToLogin();
        _e2eContext.AuthService.Login(
            credentials.AdminUser.Username,
            credentials.AdminUser.Password);

        logger.Information("[E2E Scenario] Login completed");
    }

    [AfterScenario("e2e")]
    public async Task AfterScenario()
    {
        var logger = Log.ForContext<DashboardE2EHooks>();

        var configLoader = ConfigurationLoader.Instance;
        var testConfig = configLoader.GetTestConfiguration();

        var scenarioFailed = _scenarioContext.ScenarioExecutionStatus
            is ScenarioExecutionStatus.TestError
            or ScenarioExecutionStatus.BindingError;

        UiTestSupport.QuitWithScreenshot(
            DriverManager.CurrentDriver,
            _scenarioContext.ScenarioInfo.Title,
            testFailed: scenarioFailed,
            takeScreenshotOnFailure: testConfig.TakeScreenshotOnFailure,
            logger);

        foreach (var id in _e2eContext.CreatedDashboardIds)
        {
            try
            {
                await _e2eContext.ApiService.DeleteDashboardAsync(id);
                logger.Information("[E2E Scenario] Deleted dashboard {DashboardId}", id);
            }
            catch (Exception ex)
            {
                logger.Warning(ex, "[E2E Scenario] Failed to delete dashboard {DashboardId}", id);
            }
        }

        logger.Information("[E2E Scenario] Finished: {Scenario} — {Status}",
            _scenarioContext.ScenarioInfo.Title,
            _scenarioContext.ScenarioExecutionStatus);
    }
}
