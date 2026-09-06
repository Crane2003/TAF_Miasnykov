using Business.Models;
using Core.Utilities;
using Reqnroll;
using Tests.BDD.Context;
using Assert = Xunit.Assert;

namespace Tests.BDD.Steps;

[Binding]
public sealed class DashboardE2EStepDefinitions
{
    private readonly DashboardE2EContext _ctx;

    public DashboardE2EStepDefinitions(DashboardE2EContext ctx)
    {
        _ctx = ctx;
    }

    [Given("the user is authenticated and logged in to the application")]
    public void GivenTheUserIsAuthenticatedAndLoggedIn()
    {
        Assert.NotNull(_ctx.AuthService);
        Assert.NotNull(_ctx.UiService);
        Assert.NotNull(_ctx.ApiService);
    }

    [Given("a unique dashboard name is prepared with base {string}")]
    public void GivenAUniqueDashboardNameIsPreparedWithBase(string baseName)
    {
        _ctx.LastCreatedDashboardName = baseName.Unique();
    }

    [When("the user creates the dashboard via UI with description {string}")]
    public async Task WhenTheUserCreatesTheDashboardViaUiWithDescription(string description)
    {
        Assert.NotNull(_ctx.LastCreatedDashboardName);

        _ctx.UiService.NavigateToDashboards();
        _ctx.UiService.CreateDashboard(_ctx.LastCreatedDashboardName, description);

        _ctx.LastCreatedDashboardId = await _ctx.ApiService.GetDashboardIdByNameAsync(_ctx.LastCreatedDashboardName);
        if (_ctx.LastCreatedDashboardId.HasValue)
            _ctx.TrackDashboard(_ctx.LastCreatedDashboardId.Value);
    }

    [Then("the prepared dashboard should be visible")]
    public void ThenThePreparedDashboardShouldBeVisible()
    {
        Assert.NotNull(_ctx.LastCreatedDashboardName);
        Assert.True(_ctx.UiService.IsDashboardNameVisible(_ctx.LastCreatedDashboardName));
    }

    [Given("a dashboard is created via API with base name {string}")]
    public async Task GivenADashboardIsCreatedViaApiWithBaseName(string baseName)
    {
        var dashboardName = baseName.Unique();
        var dashboard = await _ctx.ApiService.CreateDashboardAsync(DashboardCreateRequest.CreateWithName(dashboardName));

        Assert.NotNull(dashboard);
        Assert.True(dashboard!.Id > 0);

        _ctx.LastCreatedDashboardName = dashboardName;
        _ctx.LastCreatedDashboardId = dashboard.Id;
        _ctx.TrackDashboard(dashboard.Id);
    }

    [When("the user removes the prepared dashboard via UI")]
    public void WhenTheUserRemovesThePreparedDashboardViaUi()
    {
        Assert.NotNull(_ctx.LastCreatedDashboardName);

        _ctx.UiService.NavigateToDashboards();
        _ctx.UiService.OpenDashboard(_ctx.LastCreatedDashboardName);
        _ctx.UiService.ClickDelete();
    }

    [Then("the prepared dashboard should not be visible on home page")]
    public void ThenThePreparedDashboardShouldNotBeVisibleOnHomePage()
    {
        Assert.NotNull(_ctx.LastCreatedDashboardName);
        Assert.False(_ctx.UiService.IsDashboardNameVisibleOnHomePage(_ctx.LastCreatedDashboardName));

        if (_ctx.LastCreatedDashboardId.HasValue)
        {
            _ctx.CreatedDashboardIds.Remove(_ctx.LastCreatedDashboardId.Value);
            _ctx.LastCreatedDashboardId = null;
        }
    }

    [Given("an updated dashboard name is prepared from the current dashboard name")]
    public void GivenAnUpdatedDashboardNameIsPreparedFromTheCurrentDashboardName()
    {
        Assert.NotNull(_ctx.LastCreatedDashboardName);
        _ctx.UpdatedDashboardName = $"{_ctx.LastCreatedDashboardName} Updated";
    }

    [When("the user edits the prepared dashboard via UI with description {string}")]
    public void WhenTheUserEditsThePreparedDashboardViaUiWithDescription(string description)
    {
        Assert.NotNull(_ctx.LastCreatedDashboardName);
        Assert.NotNull(_ctx.UpdatedDashboardName);

        _ctx.UiService.NavigateToDashboards();
        _ctx.UiService.OpenDashboard(_ctx.LastCreatedDashboardName);
        _ctx.UiService.ClickEdit();

        var editModal = _ctx.UiService.GetAddDashboardModal();
        editModal.CreateDashboard(_ctx.UpdatedDashboardName, description);
    }

    [Then("the updated dashboard should be visible on details page")]
    public void ThenTheUpdatedDashboardShouldBeVisibleOnDetailsPage()
    {
        Assert.NotNull(_ctx.UpdatedDashboardName);
        Assert.True(_ctx.UiService.IsDashboardNameVisibleOnDetailsPage(_ctx.UpdatedDashboardName));
    }

    [Given("a default dashboard is created via API")]
    public async Task GivenADefaultDashboardIsCreatedViaApi()
    {
        var dashboard = await _ctx.ApiService.CreateDashboardAsync(DashboardCreateRequest.CreateDefault());
        Assert.NotNull(dashboard);
        Assert.True(dashboard!.Id > 0);

        _ctx.LastCreatedDashboardName = dashboard.Name;
        _ctx.LastCreatedDashboardId = dashboard.Id;
        _ctx.TrackDashboard(dashboard.Id);
    }

    [Given("a unique widget name is prepared with base {string}")]
    public void GivenAUniqueWidgetNameIsPreparedWithBase(string baseName)
    {
        _ctx.LastCreatedWidgetName = baseName.Unique();
    }

    [When("the user adds widget type {string} with the prepared widget name and description {string}")]
    public void WhenTheUserAddsWidgetTypeWithThePreparedWidgetNameAndDescription(string widgetType, string description)
    {
        Assert.NotNull(_ctx.LastCreatedDashboardName);
        Assert.NotNull(_ctx.LastCreatedWidgetName);

        _ctx.UiService.NavigateToDashboards();
        _ctx.UiService.OpenDashboard(_ctx.LastCreatedDashboardName);
        _ctx.UiService.AddWidget(widgetType, _ctx.LastCreatedWidgetName, description);
    }

    [Then("the prepared widget should be visible")]
    public void ThenThePreparedWidgetShouldBeVisible()
    {
        Assert.NotNull(_ctx.LastCreatedWidgetName);
        Assert.True(_ctx.UiService.IsWidgetDisplayed(_ctx.LastCreatedWidgetName));
    }

    [Given("two widgets are created via API for reorder with bases {string} and {string}")]
    public async Task GivenTwoWidgetsAreCreatedViaApiForReorderWithBasesAnd(string firstBase, string secondBase)
    {
        Assert.NotNull(_ctx.LastCreatedDashboardId);

        _ctx.FirstWidgetName = firstBase.Unique();
        _ctx.SecondWidgetName = secondBase.Unique();

        var firstWidget = Widget.CreateDefault(_ctx.FirstWidgetName);
        var secondWidget = Widget.CreateChartWidget(_ctx.SecondWidgetName, 6, 0);

        Assert.NotNull(await _ctx.ApiService.AddWidgetToDashboardAsync(_ctx.LastCreatedDashboardId.Value, firstWidget));
        Assert.NotNull(await _ctx.ApiService.AddWidgetToDashboardAsync(_ctx.LastCreatedDashboardId.Value, secondWidget));
    }

    [When("the user reorders the second prepared widget before the first prepared widget")]
    public void WhenTheUserReordersTheSecondPreparedWidgetBeforeTheFirstPreparedWidget()
    {
        Assert.NotNull(_ctx.LastCreatedDashboardName);
        Assert.NotNull(_ctx.FirstWidgetName);
        Assert.NotNull(_ctx.SecondWidgetName);

        _ctx.UiService.NavigateToDashboards();
        _ctx.UiService.OpenDashboard(_ctx.LastCreatedDashboardName);

        Assert.True(_ctx.UiService.IsWidgetBefore(_ctx.FirstWidgetName, _ctx.SecondWidgetName));
        _ctx.UiService.ReorderWidgets(_ctx.SecondWidgetName, _ctx.FirstWidgetName);
    }

    [Then("the second prepared widget should appear before the first prepared widget")]
    public void ThenTheSecondPreparedWidgetShouldAppearBeforeTheFirstPreparedWidget()
    {
        Assert.NotNull(_ctx.FirstWidgetName);
        Assert.NotNull(_ctx.SecondWidgetName);
        Assert.True(_ctx.UiService.WaitUntilWidgetIsBefore(_ctx.SecondWidgetName, _ctx.FirstWidgetName));
    }

    [Given("a default widget is created via API with base name {string}")]
    public async Task GivenADefaultWidgetIsCreatedViaApiWithBaseName(string baseName)
    {
        Assert.NotNull(_ctx.LastCreatedDashboardId);

        _ctx.LastCreatedWidgetName = baseName.Unique();
        var widget = Widget.CreateDefault(_ctx.LastCreatedWidgetName);
        Assert.NotNull(await _ctx.ApiService.AddWidgetToDashboardAsync(_ctx.LastCreatedDashboardId.Value, widget));
    }

    [When("the user removes the prepared widget via UI")]
    public void WhenTheUserRemovesThePreparedWidgetViaUi()
    {
        Assert.NotNull(_ctx.LastCreatedDashboardName);
        Assert.NotNull(_ctx.LastCreatedWidgetName);

        _ctx.UiService.NavigateToDashboards();
        _ctx.UiService.OpenDashboard(_ctx.LastCreatedDashboardName);
        _ctx.UiService.RemoveWidget(_ctx.LastCreatedWidgetName);
    }

    [Then("the prepared widget should not be visible")]
    public void ThenThePreparedWidgetShouldNotBeVisible()
    {
        Assert.NotNull(_ctx.LastCreatedWidgetName);
        Assert.False(_ctx.UiService.IsWidgetDisplayed(_ctx.LastCreatedWidgetName));
    }
}
