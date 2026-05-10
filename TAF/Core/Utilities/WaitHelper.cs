using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace TAF.Core.Utilities;

public static class WaitHelper
{
    public static void WaitForElementVisible(IWebDriver driver, By locator, int timeoutInSeconds = 10)
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
        wait.Until(ExpectedConditions.ElementIsVisible(locator));
    }

    public static void WaitForElementClickable(IWebDriver driver, By locator, int timeoutInSeconds = 10)
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
        wait.Until(ExpectedConditions.ElementToBeClickable(locator));
    }

    public static void WaitForElementInvisible(IWebDriver driver, By locator, int timeoutInSeconds = 10)
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
        wait.Until(ExpectedConditions.InvisibilityOfElementLocated(locator));
    }

    public static void WaitForUrlContains(IWebDriver driver, string urlPart, int timeoutInSeconds = 10)
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
        wait.Until(ExpectedConditions.UrlContains(urlPart));
    }

    public static void WaitForTitleContains(IWebDriver driver, string title, int timeoutInSeconds = 10)
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
        wait.Until(ExpectedConditions.TitleContains(title));
    }
}
