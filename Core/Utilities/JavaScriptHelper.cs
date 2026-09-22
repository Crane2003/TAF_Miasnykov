using OpenQA.Selenium;

namespace Core.Utilities;

public static class JavaScriptHelper
{
    private static readonly ILogger _logger = Log.ForContext(typeof(JavaScriptHelper));

    private static IJavaScriptExecutor GetExecutor(IWebDriver driver)
        => (IJavaScriptExecutor)driver;

    public static object? ExecuteScript(IWebDriver driver, string script, params object[] args)
        => GetExecutor(driver).ExecuteScript(script, args);

    public static void ScrollToElement(IWebDriver driver, IWebElement element)
    {
        _logger.Debug("Scrolling element into view via JS");
        GetExecutor(driver).ExecuteScript(
            "arguments[0].scrollIntoView({block: 'center', inline: 'center'});", element);
    }

    public static bool IsElementScrolledIntoView(IWebDriver driver, IWebElement element)
    {
        const string script = """
            var rect = arguments[0].getBoundingClientRect();
            return (
                rect.top >= 0 &&
                rect.left >= 0 &&
                rect.bottom <= (window.innerHeight || document.documentElement.clientHeight) &&
                rect.right <= (window.innerWidth || document.documentElement.clientWidth)
            );
            """;

        var result = GetExecutor(driver).ExecuteScript(script, element);
        var isInView = result is bool b && b;
        _logger.Debug("Element scrolled into view: {IsInView}", isInView);
        return isInView;
    }

    public static void ClickViaJs(IWebDriver driver, IWebElement element)
    {
        _logger.Debug("Clicking element via JS executor");
        GetExecutor(driver).ExecuteScript("arguments[0].click();", element);
    }
}
