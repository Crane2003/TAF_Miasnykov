using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;

namespace TAF.Core.Driver;

public static class DriverFactory
{
    public static IWebDriver CreateDriver(BrowserType browserType)
    {
        return browserType switch
        {
            BrowserType.Chrome => CreateChromeDriver(),
            BrowserType.Firefox => CreateFirefoxDriver(),
            BrowserType.Edge => CreateEdgeDriver(),
            _ => throw new ArgumentException($"Browser type {browserType} is not supported")
        };
    }

    private static ChromeDriver CreateChromeDriver()
    {
        var options = new ChromeOptions();
        options.AddArgument("--start-maximized");
        options.AddArgument("--disable-notifications");
        return new ChromeDriver(options);
    }

    private static FirefoxDriver CreateFirefoxDriver()
    {
        var options = new FirefoxOptions();
        options.AddArgument("--start-maximized");
        return new FirefoxDriver(options);
    }

    private static EdgeDriver CreateEdgeDriver()
    {
        var options = new EdgeOptions();
        options.AddArgument("--start-maximized");
        return new EdgeDriver(options);
    }
}

public enum BrowserType
{
    Chrome,
    Firefox,
    Edge
}
