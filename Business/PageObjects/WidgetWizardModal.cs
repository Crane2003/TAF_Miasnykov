using Core.Elements;
using OpenQA.Selenium;

namespace Business.PageObjects;

public class WidgetWizardModal
{
    private const string ModalContainerXPath = "//div[contains(@class,'widgetWizardModal__modal-window')]";

    private readonly ILogger _logger = Log.ForContext<WidgetWizardModal>();
    private readonly IWebDriver _driver;
    private readonly BaseElement _modalWindow;
    private readonly Button _nextStepButton;

    private readonly TextBox _itemsInput;
    private readonly Button _latestLaunchesToggle;
    private readonly Button _demoFilterRadio;
    private readonly Button _firstAvailableFilterRadio;

    private readonly TextBox _widgetNameInput;
    private readonly TextBox _widgetDescriptionTextArea;
    private readonly Button _addButton;

    public WidgetWizardModal(IWebDriver driver)
    {
        _driver = driver;
        _modalWindow = new BaseElement(driver, By.XPath(ModalContainerXPath));
        _nextStepButton = new Button(driver, By.XPath("//button[.//span[text()='Next step']]"));

        _itemsInput = new TextBox(driver, By.XPath($"{ModalContainerXPath}//div[contains(@class,'modal-field-label')][.//span[text()='Items']]/following-sibling::div//input"));
        _latestLaunchesToggle = new Button(driver, By.XPath($"{ModalContainerXPath}//div[contains(@class,'toggle-button-item')][.//span[text()='Latest launches']]"));
        _demoFilterRadio = new Button(driver, By.XPath($"{ModalContainerXPath}//label[.//input[@name='filterId']][.//span[normalize-space(.)='DEMO_FILTER']]"));
        _firstAvailableFilterRadio = new Button(driver, By.XPath($"({ModalContainerXPath}//label[.//input[@name='filterId']])[1]"));

        _widgetNameInput = new TextBox(driver, By.XPath($"{ModalContainerXPath}//input[@placeholder='Enter widget name']"));
        _widgetDescriptionTextArea = new TextBox(driver, By.XPath($"{ModalContainerXPath}//textarea[@placeholder='Enter widget description']"));
        _addButton = new Button(driver, By.XPath($"{ModalContainerXPath}//button[text()='Add']"));
    }

    public bool IsDisplayed()
    {
        var isDisplayed = _modalWindow.IsDisplayed();
        _logger.Debug("Widget Wizard modal displayed: {IsDisplayed}", isDisplayed);
        return isDisplayed;
    }

    private static By WidgetTypeRadioLocator(string widgetTypeValue)
        => By.CssSelector($"input[name='widget-type'][value='{widgetTypeValue}']");

    public void SelectWidgetTypeByValue(string widgetTypeValue)
    {
        _logger.Information("Selecting widget type by value: {WidgetTypeValue}", widgetTypeValue);
        var radio = new BaseElement(_driver, WidgetTypeRadioLocator(widgetTypeValue));
        radio.ClickViaJs();
    }

    public void ClickNextStep()
    {
        _logger.Information("Clicking Next step in widget wizard");
        _nextStepButton.Click();
    }

    public void SetItemsCount(string itemsCount)
    {
        _logger.Information("Setting widget items count: {ItemsCount}", itemsCount);
        _itemsInput.ClearText();
        _itemsInput.EnterText(itemsCount);
    }

    public void SelectLatestLaunches()
    {
        _logger.Information("Selecting 'Latest launches' toggle in widget wizard");
        _latestLaunchesToggle.Click();
    }

    public void SelectDemoFilterOrFirstAvailable()
    {
        if (_demoFilterRadio.IsDisplayed())
        {
            _logger.Information("Selecting 'DEMO_FILTER' in widget wizard");
            _demoFilterRadio.Click();
            return;
        }

        _logger.Warning("'DEMO_FILTER' not found in widget wizard. Selecting first available filter instead.");
        _firstAvailableFilterRadio.Click();
    }

    public void ConfigureWidgetAndProceed(string? itemsCount = null, bool useLatestLaunches = false)
    {
        SelectDemoFilterOrFirstAvailable();

        if (!string.IsNullOrEmpty(itemsCount))
        {
            SetItemsCount(itemsCount);
        }

        if (useLatestLaunches)
        {
            SelectLatestLaunches();
        }

        ClickNextStep();
    }

    public void EnterWidgetName(string name)
    {
        _logger.Information("Entering widget name: {WidgetName}", name);
        _widgetNameInput.ClearText();
        _widgetNameInput.EnterText(name);
    }

    public void EnterWidgetDescription(string description)
    {
        _logger.Information("Entering widget description");
        _widgetDescriptionTextArea.EnterText(description);
    }

    public void ClickAdd()
    {
        _logger.Information("Clicking Add button to save widget");
        _addButton.Click();
    }

    public void SaveWidget(string? name = null, string? description = null)
    {
        if (!string.IsNullOrEmpty(name))
        {
            EnterWidgetName(name);
        }

        if (!string.IsNullOrEmpty(description))
        {
            EnterWidgetDescription(description);
        }

        ClickAdd();
    }

    public void SelectWidgetTypeAndProceed(string widgetTypeValue)
    {
        SelectWidgetTypeByValue(widgetTypeValue);
        ClickNextStep();
    }

    public void CreateWidget(
        string widgetTypeValue,
        string widgetName,
        string? widgetDescription = null,
        string? itemsCount = null,
        bool useLatestLaunches = false)
    {
        SelectWidgetTypeAndProceed(widgetTypeValue);
        ConfigureWidgetAndProceed(itemsCount, useLatestLaunches);
        SaveWidget(widgetName, widgetDescription);
    }
}
