using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace Core.Utilities;

public static class WaitHelper
{
    private static WebDriverWait CreateWait(IWebDriver driver, int timeoutInSeconds)
        => new(driver, TimeSpan.FromSeconds(timeoutInSeconds));

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

