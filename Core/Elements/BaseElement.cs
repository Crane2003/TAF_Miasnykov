using Core.Utilities;
using OpenQA.Selenium;

namespace Core.Elements;

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
        => WaitHelper.WaitForElementExists(_driver, _locator, _timeoutInSeconds);

    protected IWebElement WaitForElementToBeClickable()
        => WaitHelper.WaitForElementClickable(_driver, _locator, _timeoutInSeconds);

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
