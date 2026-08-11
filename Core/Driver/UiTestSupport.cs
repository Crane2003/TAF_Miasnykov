using Core.Configuration;
using Core.Utilities;
using OpenQA.Selenium;

namespace Core.Driver;

public static class UiTestSupport
{
    public static IWebDriver InitDriver(TestConfiguration configuration)
    {
        DriverManager.InitDriver(configuration.GetBrowserType(), configuration.Headless);
        var driver = DriverManager.Driver;
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(configuration.ImplicitWaitTimeout);
        driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(configuration.PageLoadTimeout);
        driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(configuration.ScriptTimeout);
        return driver;
    }

    public static void QuitWithScreenshot(
        IWebDriver driver,
        string testName,
        bool testFailed,
        bool takeScreenshotOnFailure,
        ILogger logger)
    {
        try
        {
            if (takeScreenshotOnFailure && testFailed)
            {
                try
                {
                    ScreenshotHelper.TakeScreenshot(driver, testName);
                    logger.Information("Screenshot captured for failed test: {TestName}", testName);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Failed to capture screenshot for test: {TestName}", testName);
                }
            }
        }
        finally
        {
            DriverManager.QuitDriver();
            logger.Information("WebDriver quit");
        }
    }
}
