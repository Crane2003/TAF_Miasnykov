using Core.Utilities;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace Core.Elements;

public class BaseElement
{
    protected readonly IWebDriver _driver;
    protected readonly By _locator;
    protected readonly ILogger _logger;
    private readonly int _timeoutInSeconds;

    public BaseElement(IWebDriver driver, By locator, int timeoutInSeconds = 10)
    {
        _driver = driver;
        _locator = locator;
        _timeoutInSeconds = timeoutInSeconds;
        _logger = Log.ForContext(GetType());
    }

    protected IWebElement Element => WaitForElement();

    protected IWebElement WaitForElement()
        => WaitHelper.WaitForElementExists(_driver, _locator, _timeoutInSeconds);

    protected IWebElement WaitForElementToBeClickable()
        => WaitHelper.WaitForElementClickable(_driver, _locator, _timeoutInSeconds);

    protected IWebElement WaitForElementFluently(int pollingIntervalMilliseconds = 500)
        => WaitHelper.FluentWaitForElement(_driver, _locator, _timeoutInSeconds, pollingIntervalMilliseconds);

    public void Click()
    {
        _logger.Information("Clicking element {Locator}", _locator);
        try
        {
            WaitForElementToBeClickable().Click();
            _logger.Debug("Clicked element {Locator}", _locator);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to click element {Locator}", _locator);
            throw;
        }
    }

    public void SendKeys(string text)
    {
        _logger.Information("Sending keys to element {Locator}", _locator);
        try
        {
            var element = WaitForElement();
            element.Clear();
            element.SendKeys(text);
            _logger.Debug("Sent keys to element {Locator}", _locator);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to send keys to element {Locator}", _locator);
            throw;
        }
    }

    public string GetText()
    {
        var text = WaitForElement().Text;
        _logger.Debug("Retrieved text from element {Locator}: {Text}", _locator, text);
        return text;
    }

    public bool IsDisplayed()
    {
        try
        {
            var isDisplayed = Element.Displayed;
            _logger.Debug("Element {Locator} displayed: {IsDisplayed}", _locator, isDisplayed);
            return isDisplayed;
        }
        catch (NoSuchElementException)
        {
            _logger.Debug("Element {Locator} not found, treating as not displayed", _locator);
            return false;
        }
        catch (WebDriverTimeoutException)
        {
            _logger.Debug("Timed out waiting for element {Locator}, treating as not displayed", _locator);
            return false;
        }
    }

    public void ScrollIntoView()
    {
        _logger.Information("Scrolling element {Locator} into view via JS", _locator);
        JavaScriptHelper.ScrollToElement(_driver, Element);
    }

    public bool IsScrolledIntoView()
    {
        var isInView = JavaScriptHelper.IsElementScrolledIntoView(_driver, Element);
        _logger.Debug("Element {Locator} is scrolled into view: {IsInView}", _locator, isInView);
        return isInView;
    }

    public void ClickViaJs()
    {
        _logger.Information("Clicking element {Locator} via JS executor", _locator);
        try
        {
            JavaScriptHelper.ClickViaJs(_driver, Element);
            _logger.Debug("Clicked element {Locator} via JS executor", _locator);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to click element {Locator} via JS executor", _locator);
            throw;
        }
    }

    public void DragAndDropTo(BaseElement target)
    {
        _logger.Information("Dragging element {SourceLocator} to {TargetLocator}", _locator, target._locator);
        try
        {
            var actions = new Actions(_driver);
            actions.DragAndDrop(Element, target.Element).Perform();
            _logger.Debug("Dragged element {SourceLocator} to {TargetLocator}", _locator, target._locator);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to drag element {SourceLocator} to {TargetLocator}", _locator, target._locator);
            throw;
        }
    }

    /// <summary>
    /// Performs a drag-and-drop of this element by the given pixel offset using the Actions API.
    /// </summary>
    public void DragAndDropBy(int offsetX, int offsetY)
    {
        _logger.Information("Dragging element {Locator} by offset ({OffsetX}, {OffsetY})", _locator, offsetX, offsetY);
        try
        {
            var actions = new Actions(_driver);
            actions.DragAndDropToOffset(Element, offsetX, offsetY).Perform();
            _logger.Debug("Dragged element {Locator} by offset ({OffsetX}, {OffsetY})", _locator, offsetX, offsetY);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to drag element {Locator} by offset ({OffsetX}, {OffsetY})", _locator, offsetX, offsetY);
            throw;
        }
    }

    public void ResizeBy(int offsetX, int offsetY)
    {
        _logger.Information("Resizing element {Locator} by offset ({OffsetX}, {OffsetY})", _locator, offsetX, offsetY);
        try
        {
            var element = Element;
            var actions = new Actions(_driver);
            actions
                .ClickAndHold(element)
                .MoveByOffset(offsetX, offsetY)
                .Release()
                .Perform();
            _logger.Debug("Resized element {Locator} by offset ({OffsetX}, {OffsetY})", _locator, offsetX, offsetY);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to resize element {Locator} by offset ({OffsetX}, {OffsetY})", _locator, offsetX, offsetY);
            throw;
        }
    }
}
