using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace TAF.Core.Elements;

public class BaseElement
{
    protected readonly IWebDriver _driver;
    protected readonly By _locator;
    private readonly int _timeoutInSeconds;

    public BaseElement(IWebDriver driver, By locator, int timeoutInSeconds = 10)
    {
        _driver = driver;
        _locator = locator;
        _timeoutInSeconds = timeoutInSeconds;
    }

    protected IWebElement Element => WaitForElement();

    protected IWebElement WaitForElement()
    {
        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(_timeoutInSeconds));
        return wait.Until(ExpectedConditions.ElementExists(_locator));
    }

    protected IWebElement WaitForElementToBeClickable()
    {
        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(_timeoutInSeconds));
        return wait.Until(ExpectedConditions.ElementToBeClickable(_locator));
    }

    public void Click()
    {
        WaitForElementToBeClickable().Click();
    }

    public void SendKeys(string text)
    {
        var element = WaitForElement();
        element.Clear();
        element.SendKeys(text);
    }

    public string GetText()
    {
        return WaitForElement().Text;
    }

    public bool IsDisplayed()
    {
        try
        {
            return Element.Displayed;
        }
        catch (NoSuchElementException)
        {
            return false;
        }
    }

    public bool IsEnabled()
    {
        return Element.Enabled;
    }

    public string GetAttribute(string attributeName)
    {
        return Element.GetDomAttribute(attributeName);
    }
}
