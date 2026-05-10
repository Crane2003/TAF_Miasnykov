using OpenQA.Selenium;

namespace TAF.Core.Elements;

public class Button : BaseElement
{
    public Button(IWebDriver driver, By locator, int timeoutInSeconds = 10)
        : base(driver, locator, timeoutInSeconds)
    {
    }
}
