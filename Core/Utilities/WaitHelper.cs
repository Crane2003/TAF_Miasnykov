using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace Core.Utilities;

public static class WaitHelper
{
    private static readonly ILogger _logger = Log.ForContext(typeof(WaitHelper));

    private static WebDriverWait CreateWait(IWebDriver driver, int timeoutInSeconds)
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
        wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));
        return wait;
    }

    public static WebDriverWait CreateFluentWait(
        IWebDriver driver,
        int timeoutInSeconds = 10,
        int pollingIntervalMilliseconds = 500,
        params Type[] exceptionsToIgnore)
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds))
        {
            PollingInterval = TimeSpan.FromMilliseconds(pollingIntervalMilliseconds)
        };

        var ignoredExceptions = exceptionsToIgnore.Length > 0
            ? exceptionsToIgnore
            : [typeof(NoSuchElementException), typeof(StaleElementReferenceException)];

        wait.IgnoreExceptionTypes(ignoredExceptions);
        return wait;
    }

    public static IWebElement FluentWaitForElement(
        IWebDriver driver,
        By locator,
        int timeoutInSeconds = 10,
        int pollingIntervalMilliseconds = 500)
    {
        _logger.Debug("Fluent waiting for element {Locator} (timeout: {Timeout}s, polling: {Polling}ms)",
            locator, timeoutInSeconds, pollingIntervalMilliseconds);
        return CreateFluentWait(driver, timeoutInSeconds, pollingIntervalMilliseconds)
            .Until(d => d.FindElement(locator));
    }

    public static IWebElement WaitForElementExists(IWebDriver driver, By locator, int timeoutInSeconds = 10)
        => CreateWait(driver, timeoutInSeconds).Until(ExpectedConditions.ElementExists(locator));

    public static IWebElement WaitForElementVisible(IWebDriver driver, By locator, int timeoutInSeconds = 10)
        => CreateWait(driver, timeoutInSeconds).Until(ExpectedConditions.ElementIsVisible(locator));

    public static IWebElement WaitForElementClickable(IWebDriver driver, By locator, int timeoutInSeconds = 10)
        => CreateWait(driver, timeoutInSeconds).Until(ExpectedConditions.ElementToBeClickable(locator));

    public static void WaitForElementInvisible(IWebDriver driver, By locator, int timeoutInSeconds = 10)
        => CreateWait(driver, timeoutInSeconds).Until(ExpectedConditions.InvisibilityOfElementLocated(locator));

    public static void WaitForUrlContains(IWebDriver driver, string urlPart, int timeoutInSeconds = 10)
        => CreateWait(driver, timeoutInSeconds).Until(ExpectedConditions.UrlContains(urlPart));

    public static void WaitForTitleContains(IWebDriver driver, string title, int timeoutInSeconds = 10)
        => CreateWait(driver, timeoutInSeconds).Until(ExpectedConditions.TitleContains(title));
}
