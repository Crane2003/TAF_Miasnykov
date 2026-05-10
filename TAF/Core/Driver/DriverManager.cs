using OpenQA.Selenium;

namespace TAF.Core.Driver;

public class DriverManager
{
    private static readonly ThreadLocal<IWebDriver> _driver = new();

    public static IWebDriver Driver
    {
        get
        {
            if (_driver.Value == null)
            {
                throw new InvalidOperationException("WebDriver is not initialized. Call InitDriver() first.");
            }
            return _driver.Value;
        }
        private set => _driver.Value = value;
    }

    public static void InitDriver(BrowserType browserType)
    {
        if (_driver.Value == null)
        {
            Driver = DriverFactory.CreateDriver(browserType);
        }
    }

    public static void QuitDriver()
    {
        _driver.Value?.Quit();
        _driver.Value = null;
    }
}
