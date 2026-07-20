using OpenQA.Selenium;

namespace Core.Elements;

public class TextBox : BaseElement
{
    public TextBox(IWebDriver driver, By locator, int timeoutInSeconds = 10)
        : base(driver, locator, timeoutInSeconds)
    {
    }

    public void EnterText(string text)
    {
        SendKeys(text);
    }

    public void ClearText()
    {
        Element.Clear();
    }
}
