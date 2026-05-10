using TAF.Business.PageObjects;
using TAF.Core.Driver;

namespace TAF.Business.Services;

public class DashboardUiService
{
    private readonly DashboardPage _dashboardPage;

    public DashboardUiService(string baseUrl)
    {
        var driver = DriverManager.Driver;
        _dashboardPage = new DashboardPage(driver, baseUrl);
    }

    public void NavigateToDashboards()
    {
        _dashboardPage.Open();
    }

    public void ClickAddNewDashboard()
    {
        _dashboardPage.ClickAddNewDashboard();
    }

    public void ClickAddNewWidget()
    {
        _dashboardPage.ClickAddNewWidget();
    }

    public void ClickEdit()
    {
        _dashboardPage.ClickEdit();
    }

    public void ClickDelete()
    {
        _dashboardPage.ClickDelete();
    }

    public void ClickFullScreen()
    {
        _dashboardPage.ClickFullScreen();
    }

    public string GetDashboardTitle()
    {
        return _dashboardPage.GetDashboardTitle();
    }

    public bool IsAddNewDashboardButtonVisible()
    {
        return _dashboardPage.IsAddNewDashboardButtonDisplayed();
    }

    public bool IsAddNewWidgetButtonVisible()
    {
        return _dashboardPage.IsAddNewWidgetButtonDisplayed();
    }

    public bool IsPageLoaded()
    {
        return _dashboardPage.IsPageLoaded();
    }
}
