using OpenQA.Selenium;

namespace TAF.Core.Driver;

public class DriverManager
{
    private static readonly ThreadLocal<IWebDriver> _driver = new();
    private static readonly ILogger _logger = Log.ForContext<DriverManager>();

    public static IWebDriver Driver
    {
        get
        {
            if (_driver.Value == null)
            {
                _logger.Error("WebDriver is not initialized");
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
            _logger.Information("Initializing WebDriver for browser: {BrowserType}", browserType);
            Driver = DriverFactory.CreateDriver(browserType);
            _logger.Debug("WebDriver initialized successfully");
        }
        else
        {
            _logger.Debug("WebDriver already initialized, skipping initialization");
        }
    }

    public static void QuitDriver()
    {
        if (_driver.Value != null)
        {
            _logger.Debug("Quitting WebDriver");
            _driver.Value?.Quit();
            _driver.Value = null;
            _logger.Debug("WebDriver quit successfully");
        }
    }
}
