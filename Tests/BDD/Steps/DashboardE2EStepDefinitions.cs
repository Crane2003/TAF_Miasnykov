using Business.Models;
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
        // Login is performed by DashboardE2EHooks.BeforeScenario.
        // This step verifies the authentication service is ready.
        Assert.NotNull(_ctx.AuthService);
        Assert.NotNull(_ctx.UiService);
    }

    [When("the user navigates to the dashboards page")]
    public void WhenTheUserNavigatesToTheDashboardsPage()
    {
        _ctx.UiService.NavigateToDashboards();
    }

    [Then("the dashboard page should be loaded successfully")]
    public void ThenTheDashboardPageShouldBeLoadedSuccessfully()
    {
        Assert.True(_ctx.UiService.IsPageLoaded());
    }

    [Then("the Add New Dashboard button should be visible")]
    public void ThenTheAddNewDashboardButtonShouldBeVisible()
    {
        Assert.True(_ctx.UiService.IsAddNewDashboardButtonVisible());
    }

    [Then("the Add New Widget button should be visible")]
    public void ThenTheAddNewWidgetButtonShouldBeVisible()
    {
        Assert.True(_ctx.UiService.IsAddNewWidgetButtonVisible());
    }

    [Given("a new dashboard is created via the API with name {string}")]
    public async Task GivenANewDashboardIsCreatedViaTheApiWithName(string name)
    {
        var uniqueName = $"{name} {DateTime.Now:HHmmss}";

        var request = DashboardCreateRequest.CreateWithName(uniqueName);
        var result = await _ctx.ApiService.CreateDashboardAsync(request);

        Assert.NotNull(result);
        Assert.True(result.Id > 0);

        _ctx.LastCreatedDashboardName = uniqueName;
        _ctx.TrackDashboard(result.Id);
    }

    [Then("the dashboard named {string} should be visible in the list")]
    public void ThenTheDashboardNamedShouldBeVisibleInTheList(string name)
    {
        var actualName = _ctx.LastCreatedDashboardName ?? name;

        Assert.True(_ctx.UiService.IsDashboardNameVisible(actualName));
    }
}
