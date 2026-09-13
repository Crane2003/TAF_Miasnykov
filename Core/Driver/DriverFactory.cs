using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;

namespace Core.Driver;

public static class DriverFactory
{
    public static IWebDriver CreateDriver(BrowserType browserType, bool headless = true)
    {
        return browserType switch
        {
            BrowserType.Chrome => CreateChromeDriver(headless),
            BrowserType.Firefox => CreateFirefoxDriver(headless),
            BrowserType.Edge => CreateEdgeDriver(headless),
            _ => throw new ArgumentException($"Browser type {browserType} is not supported")
        };
    }

    private static ChromeDriver CreateChromeDriver(bool headless)
    {
        var options = new ChromeOptions();

        // For CI/Linux environments, specify Chrome binary location
        var chromePathFromEnv = Environment.GetEnvironmentVariable("CHROME_PATH");
        var chromePaths = new[]
        {
            chromePathFromEnv,
            "/usr/bin/google-chrome",
            "/usr/bin/chromium-browser",
            "/snap/bin/chromium"
        };

        foreach (var path in chromePaths)
        {
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                options.BinaryLocation = path;
                break;
            }
        }

        options.AddArgument("--start-maximized");
        options.AddArgument("--ignore-certificate-errors");
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-gpu");
        options.AddArgument("--disable-dev-shm-usage");
        options.AcceptInsecureCertificates = true;
        if (headless)
        {
            options.AddArgument("--headless=new");
            options.AddArgument("--window-size=1920,1080");
        }
        return new ChromeDriver(options);
    }

    private static FirefoxDriver CreateFirefoxDriver(bool headless)
    {
        var options = new FirefoxOptions();
        options.AddArgument("--start-maximized");
        options.AcceptInsecureCertificates = true;
        if (headless)
            options.AddArgument("--headless");
        return new FirefoxDriver(options);
    }

    private static EdgeDriver CreateEdgeDriver(bool headless)
    {
        var options = new EdgeOptions();
        options.AddArgument("--start-maximized");
        options.AddArgument("--ignore-certificate-errors");
        options.AcceptInsecureCertificates = true;
        if (headless)
        {
            options.AddArgument("--headless=new");
            options.AddArgument("--window-size=1920,1080");
        }
        return new EdgeDriver(options);
    }
}

public enum BrowserType
{
    Chrome,
    Firefox,
    Edge
}
