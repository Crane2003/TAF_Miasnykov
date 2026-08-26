using Business.Models;
using Business.Services;
using Core.Utilities;
using Tests.Base;
using Tests.Helpers;
using Xunit;
using Assert = Xunit.Assert;

namespace Tests.Api;

public class DashboardApiXUnitTests : XUnitBaseTest, IAsyncLifetime, IClassFixture<XUnitTestFixture>
{
    private readonly DashboardApiService _dashboardApiService = null!;
    private int? _createdDashboardId;

    // Ids far beyond any sequence value the project will allocate, so the API reports them as missing.
    private const int NonExistentDashboardId = 99999999;
    private const int NonExistentWidgetId = 99999999;

    #region Setup / Teardown

    public DashboardApiXUnitTests(XUnitTestFixture fixture) : base(fixture)
    {
        _dashboardApiService = new DashboardApiService(ApiConfiguration.BaseUrl, Configuration.ProjectName);
        _dashboardApiService.SetAuthToken(ApiConfiguration.AuthToken);
    }

    public override ValueTask InitializeAsync()
    {
        _createdDashboardId = null;
        return ValueTask.CompletedTask;
    }

    public override async ValueTask DisposeAsync()
    {
        if (_createdDashboardId.HasValue)
            await _dashboardApiService.DeleteDashboardAsync(_createdDashboardId.Value);
    }

    #endregion

    #region Journey: Create a dashboard (POST)

    // POSITIVE: a valid payload creates the dashboard and echoes back the submitted values.
    [Fact]
    public async Task CreateDashboard_ViaPostRequest_ShouldSucceed()
    {
        var request = DashboardCreateRequest.CreateDefault();

        var result = await _dashboardApiService.CreateDashboardAsync(request);

        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal(request.Name, result.Name);
        Assert.Equal(request.Description, result.Description);

        _createdDashboardId = result.Id;
    }

    // NEGATIVE: the API rejects blank names with HTTP 400 - name must be 3-128 characters.
    [Fact]
    public async Task CreateDashboard_WithEmptyName_ShouldFail()
    {
        var request = new DashboardCreateRequest { Name = string.Empty, Description = "Invalid name" };

        var result = await _dashboardApiService.CreateDashboardAsync(request);

        Assert.Null(result);
    }

    // NEGATIVE: dashboard names must be unique per project; the second create returns HTTP 409.
    [Fact]
    public async Task CreateDashboard_WithDuplicateName_ShouldFail()
    {
        var request = DashboardCreateRequest.CreateDefault();
        var firstResult = await _dashboardApiService.CreateDashboardAsync(request);
        Assert.NotNull(firstResult);
        _createdDashboardId = firstResult.Id;

        var duplicateResult = await _dashboardApiService.CreateDashboardAsync(request);

        Assert.Null(duplicateResult);
    }

    #endregion

    #region Journey: Get a created dashboard (GET)

    // POSITIVE: a created dashboard is readable and round-trips its stored values.
    [Fact]
    public async Task GetDashboard_ViaGetRequest_ShouldReturnCreatedDashboard()
    {
        var request = DashboardCreateRequest.CreateDefault();
        var createResult = await _dashboardApiService.CreateDashboardAsync(request);
        Assert.NotNull(createResult);
        _createdDashboardId = createResult.Id;

        var result = await _dashboardApiService.GetDashboardAsync(createResult.Id);

        Assert.NotNull(result);
        Assert.Equal(createResult.Id, result.Id);
        Assert.Equal(request.Name, result.Name);
        Assert.Equal(request.Description, result.Description);
    }

    // NEGATIVE: an id that cannot exist is reported as missing instead of returning data.
    [Fact]
    public async Task GetNonExistentDashboard_ShouldReturnError()
    {
        var result = await _dashboardApiService.GetDashboardAsync(-1);

        Assert.Null(result);
    }

    #endregion

    #region Journey: Change a dashboard (PUT)

    // POSITIVE: name and description are both replaced by the update payload.
    [Fact]
    public async Task UpdateDashboard_ViaPutRequest_ShouldModifyDashboard()
    {
        var request = DashboardCreateRequest.CreateDefault();
        var createResult = await _dashboardApiService.CreateDashboardAsync(request);
        _createdDashboardId = createResult!.Id;

        var updateRequest = new DashboardUpdateRequest
        {
            Name = $"Updated Dashboard Name {Guid.NewGuid()}",
            Description = "Updated description"
        };

        var updateResult = await _dashboardApiService.UpdateDashboardAsync(createResult.Id, updateRequest);

        Assert.NotNull(updateResult);
        Assert.Equal(updateRequest.Name, updateResult.Name);
        Assert.Equal("Updated description", updateResult.Description);
    }

    // NEGATIVE: the API returns HTTP 404 when the dashboard does not exist on the project.
    [Fact]
    public async Task UpdateDashboard_WithNonExistentId_ShouldFail()
    {
        var updateRequest = new DashboardUpdateRequest
        {
            Name = "Renamed Dashboard".Unique(),
            Description = "Updated description"
        };

        var result = await _dashboardApiService.UpdateDashboardAsync(NonExistentDashboardId, updateRequest);

        Assert.Null(result);
    }

    // NEGATIVE: the same name validation as create applies on update, so a blank name is rejected.
    [Fact]
    public async Task UpdateDashboard_WithEmptyName_ShouldFail()
    {
        var createResult = await _dashboardApiService.CreateDashboardAsync(DashboardCreateRequest.CreateDefault());
        Assert.NotNull(createResult);
        _createdDashboardId = createResult.Id;

        var updateRequest = new DashboardUpdateRequest { Name = string.Empty, Description = "Updated description" };

        var result = await _dashboardApiService.UpdateDashboardAsync(createResult.Id, updateRequest);

        Assert.Null(result);
    }

    #endregion

    #region Journey: Lock a dashboard (PATCH)

    // POSITIVE: patching the lock flag is persisted and visible on a subsequent read.
    [Fact]
    public async Task LockDashboard_ViaPatchRequest_ShouldMarkDashboardLocked()
    {
        var createResult = await _dashboardApiService.CreateDashboardAsync(DashboardCreateRequest.CreateDefault());
        Assert.NotNull(createResult);
        _createdDashboardId = createResult.Id;
        Assert.False(createResult.Locked);

        var locked = await _dashboardApiService.LockDashboardAsync(createResult.Id, true);

        Assert.True(locked);

        var result = await _dashboardApiService.GetDashboardAsync(createResult.Id);
        Assert.NotNull(result);
        Assert.True(result.Locked);
    }

    // NEGATIVE: patching an id beyond the allocated range is reported as missing.
    [Fact]
    public async Task LockDashboard_WithNonExistentId_ShouldFail()
    {
        var result = await _dashboardApiService.LockDashboardAsync(NonExistentDashboardId, true);

        Assert.False(result);
    }

    // NEGATIVE: a negative id is never a valid dashboard reference.
    [Fact]
    public async Task LockDashboard_WithNegativeId_ShouldFail()
    {
        var result = await _dashboardApiService.LockDashboardAsync(-1, true);

        Assert.False(result);
    }

    #endregion

    #region Journey: Add a widget to a dashboard (PUT)

    // POSITIVE: the widget is created and linked, and the dashboard reports the requested name.
    [Fact]
    public async Task AddWidget_ViaPutRequest_ShouldAddWidgetToDashboard()
    {
        var request = DashboardCreateRequest.CreateDefault();
        var createResult = await _dashboardApiService.CreateDashboardAsync(request);
        _createdDashboardId = createResult!.Id;

        var widget = Widget.CreateChartWidget($"Test Chart Widget {Guid.NewGuid()}", 0, 0);
        var addWidgetResult = await _dashboardApiService.AddWidgetToDashboardAsync(createResult.Id, widget);

        Assert.NotNull(addWidgetResult);
        Assert.Equal(1, addWidgetResult.GetWidgetCount());
        Assert.Equal(widget.Name, addWidgetResult.Widgets[0].Name);
    }

    #endregion

    #region Journey: Remove a widget from a dashboard (DELETE)

    // POSITIVE: only the targeted widget is detached; the remaining widget stays on the dashboard.
    [Fact]
    public async Task RemoveWidget_ViaDeleteRequest_ShouldRemoveWidgetFromDashboard()
    {
        var request = DashboardCreateRequest.CreateDefault();
        var createResult = await _dashboardApiService.CreateDashboardAsync(request);
        _createdDashboardId = createResult!.Id;

        var widget1 = Widget.CreateChartWidget("Widget 1", 0, 0);
        var widget2 = Widget.CreateChartWidget("Widget 2", 1, 0);
        await _dashboardApiService.AddWidgetToDashboardAsync(createResult.Id, widget1);
        var addResult = await _dashboardApiService.AddWidgetToDashboardAsync(createResult.Id, widget2);

        var widgetToRemove = addResult!.Widgets[0];
        var removed = await _dashboardApiService.RemoveWidgetFromDashboardAsync(createResult.Id, widgetToRemove.Id!.Value);

        Assert.True(removed);

        var verifyResult = await _dashboardApiService.GetDashboardAsync(createResult.Id);
        Assert.Equal(1, verifyResult!.GetWidgetCount());
        Assert.Null(verifyResult.GetWidgetById(widgetToRemove.Id!.Value));
    }

    // NEGATIVE: removing a widget that was never linked to the dashboard is rejected.
    [Fact]
    public async Task RemoveWidget_WithNonExistentWidgetId_ShouldFail()
    {
        var createResult = await _dashboardApiService.CreateDashboardAsync(DashboardCreateRequest.CreateDefault());
        Assert.NotNull(createResult);
        _createdDashboardId = createResult.Id;

        var result = await _dashboardApiService.RemoveWidgetFromDashboardAsync(createResult.Id, NonExistentWidgetId);

        Assert.False(result);
    }

    #endregion

    #region Journey: Remove a dashboard (DELETE)

    // POSITIVE: the dashboard is deleted and no longer resolvable afterwards.
    [Fact]
    public async Task DeleteDashboard_ViaDeleteRequest_ShouldRemoveDashboard()
    {
        var request = DashboardCreateRequest.CreateDefault();
        var createResult = await _dashboardApiService.CreateDashboardAsync(request);
        var dashboardId = createResult!.Id;

        var deleted = await _dashboardApiService.DeleteDashboardAsync(dashboardId);
        Assert.True(deleted);

        var exists = await DashboardApiTestHelpers.VerifyDashboardExistsAsync(_dashboardApiService, dashboardId);
        Assert.False(exists);
    }

    // NEGATIVE: deleting an id that was never allocated is reported as missing.
    [Fact]
    public async Task DeleteDashboard_WithNonExistentId_ShouldFail()
    {
        var result = await _dashboardApiService.DeleteDashboardAsync(NonExistentDashboardId);

        Assert.False(result);
    }

    #endregion
}
