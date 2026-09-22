using Business.Models;
using Business.Services;
using Core.Utilities;
using Tests.Base;

namespace Tests.E2E;

[TestFixture]
public class DashboardE2ETests : UiBaseTest
{
    private DashboardApiService _apiService = null!;
    private DashboardUiService _uiService = null!;
    private AuthenticationService _authService = null!;
    private int? _createdDashboardId;

    [SetUp]
    public void InitServices()
    {
        _apiService = new DashboardApiService(Configuration.ProjectName);
        _uiService = new DashboardUiService(Configuration.BaseUrl);
        _authService = new AuthenticationService(Configuration.BaseUrl);
        _createdDashboardId = null;

        _authService.NavigateToLogin();
        _authService.Login(Credentials.AdminUser.Username, Credentials.AdminUser.Password);
    }

    [TearDown]
    public async Task CleanupAsync()
    {
        if (_createdDashboardId.HasValue)
            await _apiService.DeleteDashboardAsync(_createdDashboardId.Value);
    }

    [Test]
    public async Task User_ShouldBeAbleToCreateDashboardViaUi()
    {
        var dashboardName = "E2E Dashboard".Unique();

        _uiService.NavigateToDashboards();
        _uiService.CreateDashboard(dashboardName, "Created via UI E2E test");

        Assert.That(_uiService.IsDashboardNameVisible(dashboardName), Is.True);

        _createdDashboardId = await _apiService.GetDashboardIdByNameAsync(dashboardName);
    }

    [Test]
    public async Task User_ShouldBeAbleToRemoveDashboardViaUi()
    {
        var dashboardName = "E2E Dashboard".Unique();
        var dashboard = await _apiService.CreateDashboardAsync(DashboardCreateRequest.CreateWithName(dashboardName));
        Assert.That(dashboard, Is.Not.Null);
        _createdDashboardId = dashboard.Id;

        _uiService.NavigateToDashboards();
        Assert.That(_uiService.IsDashboardNameVisibleOnHomePage(dashboardName), Is.True);

        _uiService.OpenDashboard(dashboardName);
        _uiService.ClickDelete();

        var deletedFromUi = !_uiService.IsDashboardNameVisibleOnHomePage(dashboardName);
        Assert.That(deletedFromUi, Is.True);

        if (deletedFromUi)
        {
            _createdDashboardId = null;
        }
    }

    [Test]
    public async Task User_ShouldBeAbleToEditDashboardViaUi()
    {
        var dashboardName = "E2E Dashboard".Unique();
        var updatedName = $"{dashboardName} Updated";
        var dashboard = await _apiService.CreateDashboardAsync(DashboardCreateRequest.CreateWithName(dashboardName));
        Assert.That(dashboard, Is.Not.Null);
        _createdDashboardId = dashboard.Id;

        _uiService.NavigateToDashboards();
        Assert.That(_uiService.IsDashboardNameVisibleOnHomePage(dashboardName), Is.True);

        _uiService.OpenDashboard(dashboardName);
        _uiService.ClickEdit();
        var editModal = _uiService.GetAddDashboardModal();
        editModal.CreateDashboard(updatedName, "Updated via UI E2E test");

        Assert.That(_uiService.IsDashboardNameVisibleOnDetailsPage(updatedName), Is.True);
    }

    [Test]
    public async Task User_ShouldBeAbleToAddWidgetToDashboard()
    {
        var dashboard = await _apiService.CreateDashboardAsync(DashboardCreateRequest.CreateDefault());
        Assert.That(dashboard, Is.Not.Null);
        _createdDashboardId = dashboard.Id;

        var widgetName = "E2E Widget".Unique();

        _uiService.NavigateToDashboards();
        _uiService.OpenDashboard(dashboard.Name);
        _uiService.AddWidget("overallStatistics", widgetName, "Widget added via UI E2E test");

        Assert.That(_uiService.IsWidgetDisplayed(widgetName), Is.True);
    }

    [Test]
    public async Task User_ShouldBeAbleToChangeWidgetsOrderOnDashboard()
    {
        var dashboard = await _apiService.CreateDashboardAsync(DashboardCreateRequest.CreateDefault());
        Assert.That(dashboard, Is.Not.Null);
        _createdDashboardId = dashboard.Id;

        var firstWidgetName = "E2E Widget A".Unique();
        var secondWidgetName = "E2E Widget B".Unique();

        var firstWidget = Widget.CreateDefault(firstWidgetName);
        var secondWidget = Widget.CreateChartWidget(secondWidgetName, 6, 0);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await _apiService.AddWidgetToDashboardAsync(dashboard.Id, firstWidget), Is.Not.Null);
            Assert.That(await _apiService.AddWidgetToDashboardAsync(dashboard.Id, secondWidget), Is.Not.Null);
        }

        _uiService.NavigateToDashboards();
        _uiService.OpenDashboard(dashboard.Name);

        Assert.That(_uiService.IsWidgetBefore(firstWidgetName, secondWidgetName), Is.True);

        _uiService.ReorderWidgets(secondWidgetName, firstWidgetName);

        Assert.That(_uiService.WaitUntilWidgetIsBefore(secondWidgetName, firstWidgetName), Is.True);
    }

    [Test]
    public async Task User_ShouldBeAbleToRemoveWidgetFromDashboard()
    {
        var dashboard = await _apiService.CreateDashboardAsync(DashboardCreateRequest.CreateDefault());
        Assert.That(dashboard, Is.Not.Null);
        _createdDashboardId = dashboard.Id;

        var widgetName = "E2E Widget".Unique();
        var widget = Widget.CreateDefault(widgetName);

        Assert.That(await _apiService.AddWidgetToDashboardAsync(dashboard.Id, widget), Is.Not.Null);

        _uiService.NavigateToDashboards();
        _uiService.OpenDashboard(dashboard.Name);
        Assert.That(_uiService.IsWidgetDisplayed(widgetName), Is.True);

        _uiService.RemoveWidget(widgetName);

        Assert.That(_uiService.IsWidgetDisplayed(widgetName), Is.False);
    }
}
