using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Core.Elements;

public class Dropdown : BaseElement
{
    public Dropdown(IWebDriver driver, By locator, int timeoutInSeconds = 10)
        : base(driver, locator, timeoutInSeconds)
    {
    }

    public void SelectByText(string text)
    {
        _logger.Information("Selecting dropdown {Locator} option by text: {Text}", _locator, text);
        var selectElement = new SelectElement(Element);
        selectElement.SelectByText(text);
    }

    public void SelectByValue(string value)
    {
        _logger.Information("Selecting dropdown {Locator} option by value: {Value}", _locator, value);
        var selectElement = new SelectElement(Element);
        selectElement.SelectByValue(value);
    }

    public void SelectByIndex(int index)
    {
        _logger.Information("Selecting dropdown {Locator} option by index: {Index}", _locator, index);
        var selectElement = new SelectElement(Element);
        selectElement.SelectByIndex(index);
    }

    public string GetSelectedText()
    {
        var selectElement = new SelectElement(Element);
        var text = selectElement.SelectedOption.Text;
        _logger.Debug("Dropdown {Locator} selected text: {Text}", _locator, text);
        return text;
    }
}
