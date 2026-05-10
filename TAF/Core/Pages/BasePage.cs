using OpenQA.Selenium;

namespace TAF.Core.Pages;

public abstract class BasePage
{
    protected readonly IWebDriver _driver;
    protected readonly string _url;

    protected BasePage(IWebDriver driver, string url)
    {
        _driver = driver;
        _url = url;
    }

    public void Open()
    {
        _driver.Navigate().GoToUrl(_url);
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
        _driver.Navigate().Refresh();
    }

    public void NavigateBack()
    {
        _driver.Navigate().Back();
    }

    public void NavigateForward()
    {
        _driver.Navigate().Forward();
    }

    public abstract bool IsPageLoaded();
}
