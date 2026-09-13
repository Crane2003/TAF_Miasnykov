using Business.Models;
using Business.Services;
using Core.Utilities;
using Tests.Base;
using Xunit;
using Assert = Xunit.Assert;

namespace Tests.E2E;

public class DashboardE2EXUnitTests : XUnitUiBaseTest, IAsyncLifetime, IClassFixture<XUnitTestFixture>
{
    private DashboardApiService _apiService = null!;
    private DashboardUiService _uiService = null!;
    private AuthenticationService _authService = null!;
    private int? _createdDashboardId;

    public DashboardE2EXUnitTests(XUnitTestFixture fixture) : base(fixture)
    {
    }

    public override async ValueTask InitializeAsync()
    {
        await base.InitializeAsync();
        try
        {
            _apiService = new DashboardApiService(Configuration.ProjectName);
            _uiService = new DashboardUiService(Configuration.BaseUrl);
            _authService = new AuthenticationService(Configuration.BaseUrl);
            _createdDashboardId = null;

            _authService.NavigateToLogin();
            _authService.Login(Credentials.AdminUser.Username, Credentials.AdminUser.Password);
        }
        catch
        {
            SetupFailed = true;
            await base.DisposeAsync();
            throw;
        }
    }

    public override async ValueTask DisposeAsync()
    {
        try
        {
            if (_createdDashboardId.HasValue)
                await _apiService.DeleteDashboardAsync(_createdDashboardId.Value);
        }
        finally
        {
            await base.DisposeAsync();
        }
    }

    [Fact]
    public async Task User_ShouldBeAbleToCreateDashboardViaUi()
    {
        var dashboardName = "E2E Dashboard".Unique();

        _uiService.NavigateToDashboards();
        _uiService.CreateDashboard(dashboardName, "Created via UI E2E test");

        Assert.True(_uiService.IsDashboardNameVisible(dashboardName));

        _createdDashboardId = await _apiService.GetDashboardIdByNameAsync(dashboardName);
    }

    [Fact]
    public async Task User_ShouldBeAbleToRemoveDashboardViaUi()
    {
        var dashboardName = "E2E Dashboard".Unique();
        var dashboard = await _apiService.CreateDashboardAsync(DashboardCreateRequest.CreateWithName(dashboardName));
        Assert.NotNull(dashboard);
        _createdDashboardId = dashboard.Id;

        _uiService.NavigateToDashboards();
        Assert.True(_uiService.IsDashboardNameVisibleOnHomePage(dashboardName));

        _uiService.OpenDashboard(dashboardName);
        _uiService.ClickDelete();

        var deletedFromUi = !_uiService.IsDashboardNameVisibleOnHomePage(dashboardName);
        Assert.True(deletedFromUi);

        if (deletedFromUi)
        {
            _createdDashboardId = null;
        }
        Assert.Fail("Intentionally failing the test to verify screenshot capture and reporting");
    }

    [Fact]
    public async Task User_ShouldBeAbleToEditDashboardViaUi()
    {
        var dashboardName = "E2E Dashboard".Unique();
        var updatedName = $"{dashboardName} Updated";
        var dashboard = await _apiService.CreateDashboardAsync(DashboardCreateRequest.CreateWithName(dashboardName));
        Assert.NotNull(dashboard);
        _createdDashboardId = dashboard.Id;

        _uiService.NavigateToDashboards();
        Assert.True(_uiService.IsDashboardNameVisibleOnHomePage(dashboardName));

        _uiService.OpenDashboard(dashboardName);
        _uiService.ClickEdit();
        var editModal = _uiService.GetAddDashboardModal();
        editModal.CreateDashboard(updatedName, "Updated via UI E2E test");

        Assert.True(_uiService.IsDashboardNameVisibleOnDetailsPage(updatedName));
    }

    [Fact]
    public async Task User_ShouldBeAbleToAddWidgetToDashboard()
    {
        var dashboard = await _apiService.CreateDashboardAsync(DashboardCreateRequest.CreateDefault());
        Assert.NotNull(dashboard);
        _createdDashboardId = dashboard.Id;

        var widgetName = "E2E Widget".Unique();

        _uiService.NavigateToDashboards();
        _uiService.OpenDashboard(dashboard.Name);
        _uiService.AddWidget("overallStatistics", widgetName, "Widget added via UI E2E test");

        Assert.True(_uiService.IsWidgetDisplayed(widgetName));
    }

    [Fact]
    public async Task User_ShouldBeAbleToChangeWidgetsOrderOnDashboard()
    {
        var dashboard = await _apiService.CreateDashboardAsync(DashboardCreateRequest.CreateDefault());
        Assert.NotNull(dashboard);
        _createdDashboardId = dashboard.Id;

        var firstWidgetName = "E2E Widget A".Unique();
        var secondWidgetName = "E2E Widget B".Unique();

        var firstWidget = Widget.CreateDefault(firstWidgetName);
        var secondWidget = Widget.CreateChartWidget(secondWidgetName, 6, 0);

        Assert.NotNull(await _apiService.AddWidgetToDashboardAsync(dashboard.Id, firstWidget));
        Assert.NotNull(await _apiService.AddWidgetToDashboardAsync(dashboard.Id, secondWidget));

        _uiService.NavigateToDashboards();
        _uiService.OpenDashboard(dashboard.Name);

        Assert.True(_uiService.IsWidgetBefore(firstWidgetName, secondWidgetName));

        _uiService.ReorderWidgets(secondWidgetName, firstWidgetName);

        Assert.True(_uiService.WaitUntilWidgetIsBefore(secondWidgetName, firstWidgetName));
    }

    [Fact]
    public async Task User_ShouldBeAbleToRemoveWidgetFromDashboard()
    {
        var dashboard = await _apiService.CreateDashboardAsync(DashboardCreateRequest.CreateDefault());
        Assert.NotNull(dashboard);
        _createdDashboardId = dashboard.Id;

        var widgetName = "E2E Widget".Unique();
        var widget = Widget.CreateDefault(widgetName);

        Assert.NotNull(await _apiService.AddWidgetToDashboardAsync(dashboard.Id, widget));

        _uiService.NavigateToDashboards();
        _uiService.OpenDashboard(dashboard.Name);
        Assert.True(_uiService.IsWidgetDisplayed(widgetName));

        _uiService.RemoveWidget(widgetName);

        Assert.False(_uiService.IsWidgetDisplayed(widgetName));
    }
}
