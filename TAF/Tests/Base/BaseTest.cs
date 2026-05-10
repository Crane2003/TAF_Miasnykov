using TAF.Core.Configuration;
using TAF.Core.Driver;
using TAF.Core.Utilities;

namespace TAF.Tests.Base;

public class BaseTest
{
    protected TestConfiguration Configuration { get; private set; } = null!;
    protected CredentialsConfiguration Credentials { get; private set; } = null!;
    protected ApiConfiguration ApiConfiguration { get; private set; } = null!;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        var configLoader = ConfigurationLoader.Instance;
        Configuration = configLoader.GetTestConfiguration();
        Credentials = configLoader.GetCredentials();
        ApiConfiguration = configLoader.GetApiConfiguration();
    }

    [SetUp]
    public void Setup()
    {
        DriverManager.InitDriver(Configuration.GetBrowserType());
        ConfigureTimeouts();
    }

    [TearDown]
    public void TearDown()
    {
        if (TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed)
        {
            if (Configuration.TakeScreenshotOnFailure)
            {
                ScreenshotHelper.TakeScreenshot(DriverManager.Driver, TestContext.CurrentContext.Test.Name);
            }
        }

        DriverManager.QuitDriver();
    }

    private void ConfigureTimeouts()
    {
        DriverManager.Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(Configuration.ImplicitWaitTimeout);
        DriverManager.Driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(Configuration.PageLoadTimeout);
        DriverManager.Driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(Configuration.ScriptTimeout);
    }
}
