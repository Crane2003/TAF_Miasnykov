using TAF.Core.Configuration;
using TAF.Core.Driver;
using TAF.Core.Logging;
using TAF.Core.Utilities;

namespace TAF.Tests.Base;

public class BaseTest
{
    protected TestConfiguration Configuration { get; private set; } = null!;
    protected CredentialsConfiguration Credentials { get; private set; } = null!;
    protected ApiConfiguration ApiConfiguration { get; private set; } = null!;
    protected ILogger Logger { get; private set; } = null!;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        LoggerSetup.Initialize();
        Logger = Log.ForContext<BaseTest>();

        var configLoader = ConfigurationLoader.Instance;
        Configuration = configLoader.GetTestConfiguration();
        Credentials = configLoader.GetCredentials();
        ApiConfiguration = configLoader.GetApiConfiguration();

        Logger.Information("Test configuration loaded: BaseUrl={BaseUrl}, Browser={Browser}",
            Configuration.BaseUrl, Configuration.Browser);
    }

    [SetUp]
    public void Setup()
    {
        Logger.Information("Starting test: {TestName}", TestContext.CurrentContext.Test.Name);
        DriverManager.InitDriver(Configuration.GetBrowserType());
        ConfigureTimeouts();
    }

    [TearDown]
    public void TearDown()
    {
        var testName = TestContext.CurrentContext.Test.Name;
        var testStatus = TestContext.CurrentContext.Result.Outcome.Status;

        Logger.Information("Test completed: {TestName} - Status: {TestStatus}", testName, testStatus);

        if (TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed)
        {
            Logger.Error("Test failed: {TestName} - Message: {ErrorMessage}",
                testName, TestContext.CurrentContext.Result.Message);

            if (Configuration.TakeScreenshotOnFailure)
            {
                try
                {
                    ScreenshotHelper.TakeScreenshot(DriverManager.Driver, testName);
                    Logger.Information("Screenshot captured for failed test: {TestName}", testName);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Failed to capture screenshot for test: {TestName}", testName);
                }
            }
        }

        DriverManager.QuitDriver();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        Logger.Information("Test execution completed");
        LoggerSetup.CloseAndFlush();
    }

    private void ConfigureTimeouts()
    {
        DriverManager.Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(Configuration.ImplicitWaitTimeout);
        DriverManager.Driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(Configuration.PageLoadTimeout);
        DriverManager.Driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(Configuration.ScriptTimeout);
    }
}
