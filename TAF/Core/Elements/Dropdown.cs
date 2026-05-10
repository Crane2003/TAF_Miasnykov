using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace TAF.Core.Elements;

public class Dropdown : BaseElement
{
    public Dropdown(IWebDriver driver, By locator, int timeoutInSeconds = 10)
        : base(driver, locator, timeoutInSeconds)
    {
    }

    public void SelectByText(string text)
    {
        var selectElement = new SelectElement(Element);
        selectElement.SelectByText(text);
    }

    public void SelectByValue(string value)
    {
        var selectElement = new SelectElement(Element);
        selectElement.SelectByValue(value);
    }

    public void SelectByIndex(int index)
    {
        var selectElement = new SelectElement(Element);
        selectElement.SelectByIndex(index);
    }

    public string GetSelectedText()
    {
        var selectElement = new SelectElement(Element);
        return selectElement.SelectedOption.Text;
    }
}
