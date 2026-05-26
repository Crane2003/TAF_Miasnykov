using OpenQA.Selenium;
using Serilog;

namespace TAF.Core.Pages;

public abstract class BasePage
{
    protected readonly IWebDriver _driver;
    protected readonly string _url;
    protected readonly ILogger _logger;

    protected BasePage(IWebDriver driver, string url)
    {
        _driver = driver;
        _url = url;
        _logger = Log.ForContext(GetType());
    }

    public void Open()
    {
        _logger.Information("Navigating to URL: {Url}", _url);
        _driver.Navigate().GoToUrl(_url);
        _logger.Debug("Navigation completed");
    }

    public string GetCurrentUrl()
    {
        return _driver.Url;
    }

    public string GetPageTitle()
    {
        return _driver.Title;
    }

    public void RefreshPage()
    {
        _logger.Debug("Refreshing page");
        _driver.Navigate().Refresh();
    }

    public void NavigateBack()
    {
        _logger.Debug("Navigating back");
        _driver.Navigate().Back();
    }

    public void NavigateForward()
    {
        _logger.Debug("Navigating forward");
        _driver.Navigate().Forward();
    }

    public abstract bool IsPageLoaded();
}
