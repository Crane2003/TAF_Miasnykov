using Business.PageObjects;
using Core.Driver;

namespace Business.Services;

public class DashboardUiService
{
    private readonly DashboardHomePage _dashboardHomePage;
    private readonly DashboardDetailsPage _dashboardDetailsPage;

    public DashboardUiService(string baseUrl)
    {
        var driver = DriverManager.Driver;
        _dashboardHomePage = new DashboardHomePage(driver, baseUrl);
        _dashboardDetailsPage = new DashboardDetailsPage(driver, baseUrl);
    }

    public void NavigateToDashboards()
    {
        _dashboardHomePage.NavigateViaSidebar();
    }

    public void CreateDashboard(string name, string? description = null)
    {
        _dashboardHomePage.CreateDashboard(name, description);
    }

    public AddDashboardModal GetAddDashboardModal()
    {
        return _dashboardHomePage.GetAddDashboardModal();
    }

    public void AddWidget(
        string widgetTypeValue,
        string widgetName,
        string? widgetDescription = null,
        string? itemsCount = null,
        bool useLatestLaunches = false)
    {
        _dashboardDetailsPage.AddWidget(widgetTypeValue, widgetName, widgetDescription, itemsCount, useLatestLaunches);
    }

    public void ClickEdit()
    {
        _dashboardDetailsPage.ClickEdit();
    }

    public void ClickDelete()
    {
        _dashboardDetailsPage.ClickDelete();
    }

    public bool IsDashboardNameVisibleOnHomePage(string dashboardName)
    {
        return _dashboardHomePage.IsDashboardNameVisible(dashboardName);
    }

    public bool IsDashboardNameVisibleOnDetailsPage(string dashboardName)
    {
        return _dashboardDetailsPage.IsDashboardNameVisible(dashboardName);
    }

    public bool IsDashboardNameVisible(string dashboardName)
    {
        return _dashboardDetailsPage.IsPageLoaded()
            ? IsDashboardNameVisibleOnDetailsPage(dashboardName)
            : IsDashboardNameVisibleOnHomePage(dashboardName);
    }

    public void OpenDashboard(string name)
    {
        _dashboardHomePage.OpenDashboard(name);
    }

    public bool IsWidgetDisplayed(string widgetName)
    {
        return _dashboardDetailsPage.IsWidgetDisplayed(widgetName);
    }

    public void ReorderWidgets(string sourceWidgetName, string targetWidgetName)
    {
        _dashboardDetailsPage.ReorderWidgets(sourceWidgetName, targetWidgetName);
    }

    public bool IsWidgetBefore(string firstWidgetName, string secondWidgetName)
    {
        return _dashboardDetailsPage.IsWidgetBefore(firstWidgetName, secondWidgetName);
    }

    public bool WaitUntilWidgetIsBefore(string firstWidgetName, string secondWidgetName, int timeoutInSeconds = 5)
    {
        return _dashboardDetailsPage.WaitUntilWidgetIsBefore(firstWidgetName, secondWidgetName, timeoutInSeconds);
    }

    public void RemoveWidget(string widgetName)
    {
        _dashboardDetailsPage.RemoveWidget(widgetName);
    }
}
