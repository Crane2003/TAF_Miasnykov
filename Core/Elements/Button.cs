using OpenQA.Selenium;

namespace Core.Elements;

public class Button : BaseElement
{
    public Button(IWebDriver driver, By locator, int timeoutInSeconds = 10)
        : base(driver, locator, timeoutInSeconds)
    {
    }
}
