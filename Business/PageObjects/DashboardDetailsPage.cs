using Core.Elements;
using Core.Pages;
using Core.Utilities;
using OpenQA.Selenium;

namespace Business.PageObjects;

public class DashboardDetailsPage : BasePage
{
    private static By AddNewWidgetButtonLocator => By.XPath("//div[contains(@class,'dashboardItemPage__buttons-container')]//button[.//span[normalize-space(.)='Add new widget']]");
    private static By EditButtonLocator => By.XPath("//div[contains(@class,'dashboardItemPage__buttons-container')]//button[.//span[normalize-space(.)='Edit']]");
    private static By DeleteButtonLocator => By.XPath("//div[contains(@class,'dashboardItemPage__buttons-container')]//button[.//span[normalize-space(.)='Delete']]");
    private static By DeleteDashboardModalLocator => By.XPath("//div[contains(@class,'modalLayout__modal-window')][.//span[normalize-space(.)='Delete Dashboard']]");
    private static By DeleteDashboardConfirmButtonLocator => By.XPath("//div[contains(@class,'modalLayout__modal-window')][.//span[normalize-space(.)='Delete Dashboard']]//button[normalize-space(.)='Delete']");

    private readonly Button _addNewWidgetButton;
    private readonly Button _editButton;
    private readonly Button _deleteButton;
    private readonly WidgetWizardModal _widgetWizardModal;

    public DashboardDetailsPage(IWebDriver driver, string baseUrl)
        : base(driver, $"{baseUrl}/ui")
    {
        _addNewWidgetButton = new Button(driver, AddNewWidgetButtonLocator);
        _editButton = new Button(driver, EditButtonLocator);
        _deleteButton = new Button(driver, DeleteButtonLocator);
        _widgetWizardModal = new WidgetWizardModal(driver);
    }

    public void ClickAddNewWidget()
    {
        _addNewWidgetButton.Click();
    }

    public void AddWidget(
        string widgetTypeValue,
        string widgetName,
        string? widgetDescription = null,
        string? itemsCount = null,
        bool useLatestLaunches = false)
    {
        ClickAddNewWidget();
        _widgetWizardModal.CreateWidget(widgetTypeValue, widgetName, widgetDescription, itemsCount, useLatestLaunches);
    }

    public void ClickEdit()
    {
        _editButton.Click();
    }

    public void ClickDelete()
    {
        _deleteButton.Click();

        var deleteModal = new BaseElement(_driver, DeleteDashboardModalLocator, timeoutInSeconds: 3);
        if (!deleteModal.IsDisplayed())
        {
            throw new WebDriverTimeoutException("Delete dashboard confirmation modal did not appear in time.");
        }

        new Button(_driver, DeleteDashboardConfirmButtonLocator, timeoutInSeconds: 3).Click();
    }

    public override bool IsPageLoaded()
    {
        try
        {
            WaitHelper.WaitForUrlContains(_driver, "/dashboard/", timeoutInSeconds: 3);
            return GetCurrentUrl().Contains("/dashboard/");
        }
        catch (WebDriverTimeoutException)
        {
            return false;
        }
    }

    public bool IsDashboardNameVisible(string name)
    {
        try
        {
            return new BaseElement(_driver, DashboardNameOnOpenDashboardPageLocator(name), timeoutInSeconds: 5).IsDisplayed();
        }
        catch
        {
            return false;
        }
    }

    public WidgetCard GetWidget(string widgetName)
    {
        return new WidgetCard(_driver, widgetName);
    }

    public bool IsWidgetDisplayed(string widgetName)
    {
        try
        {
            return GetWidget(widgetName).IsDisplayed();
        }
        catch
        {
            return false;
        }
    }

    public void ReorderWidgets(string sourceWidgetName, string targetWidgetName)
    {
        var source = GetWidget(sourceWidgetName);
        var target = GetWidget(targetWidgetName);
        source.DragAndDropOnto(target);
    }

    public bool IsWidgetBefore(string firstWidgetName, string secondWidgetName, int sameRowTolerancePx = 10)
    {
        var firstPosition = GetWidget(firstWidgetName).GetPosition();
        var secondPosition = GetWidget(secondWidgetName).GetPosition();

        if (Math.Abs(firstPosition.Y - secondPosition.Y) <= sameRowTolerancePx)
        {
            return firstPosition.X < secondPosition.X;
        }

        return firstPosition.Y < secondPosition.Y;
    }

    public bool WaitUntilWidgetIsBefore(string firstWidgetName, string secondWidgetName, int timeoutInSeconds = 5)
    {
        try
        {
            return WaitHelper.CreateFluentWait(
                    _driver,
                    timeoutInSeconds: timeoutInSeconds,
                    pollingIntervalMilliseconds: 200,
                    typeof(NoSuchElementException),
                    typeof(StaleElementReferenceException))
                .Until(_ => IsWidgetBefore(firstWidgetName, secondWidgetName));
        }
        catch (WebDriverTimeoutException)
        {
            return false;
        }
    }

    public void RemoveWidget(string widgetName)
    {
        GetWidget(widgetName).ClickRemove();
    }

    private static By DashboardNameOnOpenDashboardPageLocator(string name)
        => By.XPath($"//ul[contains(@class,'page-breadcrumbs')]//span[@title='{name}' or normalize-space(.)='{name}']");
}
