using Core.Driver;
using Core.Utilities;

namespace Tests.Base;

public class UiBaseTest : BaseTest
{
    [SetUp]
    public void InitDriver()
    {
        DriverManager.InitDriver(Configuration.GetBrowserType(), Configuration.Headless);
        DriverManager.Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(Configuration.ImplicitWaitTimeout);
        DriverManager.Driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(Configuration.PageLoadTimeout);
        DriverManager.Driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(Configuration.ScriptTimeout);
    }

    [TearDown]
    public new void TearDown()
    {
        var testName = TestContext.CurrentContext.Test.Name;
        var testStatus = TestContext.CurrentContext.Result.Outcome.Status;

        if (testStatus == NUnit.Framework.Interfaces.TestStatus.Failed && Configuration.TakeScreenshotOnFailure)
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

        DriverManager.QuitDriver();
    }
}
