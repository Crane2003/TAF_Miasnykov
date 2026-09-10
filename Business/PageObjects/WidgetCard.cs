using System.Drawing;
using Core.Elements;
using Core.Utilities;
using OpenQA.Selenium;

namespace Business.PageObjects;

public class WidgetCard
{
    private readonly ILogger _logger = Log.ForContext<WidgetCard>();
    private readonly IWebDriver _driver;
    private readonly string _widgetName;

    public WidgetCard(IWebDriver driver, string widgetName)
    {
        _driver = driver;
        _widgetName = widgetName;
    }

    private static string CardXPathString(string widgetName)
        => $"//div[contains(@class,'react-grid-item')][.//*[contains(@class,'widgetHeader__widget-name-block') and normalize-space(.)='{widgetName}']]";

    private By CardLocator => By.XPath(CardXPathString(_widgetName));

    private By HeaderLocator => By.XPath($"{CardXPathString(_widgetName)}//div[contains(@class,'widget__widget-header') and contains(@class,'draggable-field')]");

    // Control buttons appear in order: [0] rename/edit, [1] refresh, [2] remove/close.
    private By RemoveButtonLocator => By.XPath($"{CardXPathString(_widgetName)}//div[contains(@class,'widgetHeader__controls-block')]/button[3]");

    private static By DeleteWidgetModalLocator => By.XPath("//div[contains(@class,'modalLayout__modal-window')][.//span[normalize-space(.)='Delete widget']]");
    private static By DeleteWidgetConfirmButtonLocator => By.XPath("//div[contains(@class,'modalLayout__modal-window')][.//span[normalize-space(.)='Delete widget']]//button[normalize-space(.)='Delete']");

    public BaseElement Card => new(_driver, CardLocator);
    public BaseElement Header => new(_driver, HeaderLocator);
    public BaseElement RemoveButton => new(_driver, RemoveButtonLocator);

    public bool IsDisplayed()
    {
        var isDisplayed = Card.IsDisplayed();
        _logger.Debug("Widget card {WidgetName} displayed: {IsDisplayed}", _widgetName, isDisplayed);
        return isDisplayed;
    }

    public Point GetPosition()
    {
        var position = WaitHelper.WaitForElementVisible(_driver, CardLocator, timeoutInSeconds: 5).Location;
        _logger.Debug("Widget card {WidgetName} position: ({X}, {Y})", _widgetName, position.X, position.Y);
        return position;
    }

    public void ClickRemove()
    {
        _logger.Information("Removing widget {WidgetName}", _widgetName);
        RemoveButton.Click();

        var deleteModal = new BaseElement(_driver, DeleteWidgetModalLocator, timeoutInSeconds: 10);
        if (!deleteModal.IsDisplayed())
        {
            throw new WebDriverTimeoutException("Delete widget confirmation modal did not appear in time.");
        }

        new Button(_driver, DeleteWidgetConfirmButtonLocator, timeoutInSeconds: 10).Click();
        WaitHelper.WaitForElementInvisible(_driver, CardLocator, timeoutInSeconds: 5);
    }

    public void DragAndDropOnto(WidgetCard target)
    {
        _logger.Information("Reordering widget {SourceWidget} onto {TargetWidget}", _widgetName, target._widgetName);
        Header.DragAndDropTo(target.Header);
    }
}
