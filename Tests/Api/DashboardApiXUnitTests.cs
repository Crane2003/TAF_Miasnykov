using Business.Models;
using Business.Services;
using Tests.Base;
using Tests.Helpers;
using Xunit;
using Assert = Xunit.Assert;

namespace Tests.Api;

public class DashboardApiXUnitTests : XUnitBaseTest, IAsyncLifetime, IClassFixture<XUnitTestFixture>
{
    private readonly DashboardApiService _dashboardApiService = null!;
    private int? _createdDashboardId;

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

    [Fact]
    public async Task UpdateDashboard_ViaPutRequest_ShouldModifyDashboard()
    {
        var request = DashboardCreateRequest.CreateDefault();
        var createResult = await _dashboardApiService.CreateDashboardAsync(request);
        _createdDashboardId = createResult!.Id;

        var updateRequest = new DashboardUpdateRequest
        {
            Name = "Updated Dashboard Name",
            Description = "Updated description"
        };

        var updateResult = await _dashboardApiService.UpdateDashboardAsync(createResult.Id, updateRequest);

        Assert.NotNull(updateResult);
        Assert.Equal("Updated Dashboard Name", updateResult.Name);
        Assert.Equal("Updated description", updateResult.Description);
    }

    [Fact]
    public async Task AddWidget_ViaPutRequest_ShouldAddWidgetToDashboard()
    {
        var request = DashboardCreateRequest.CreateDefault();
        var createResult = await _dashboardApiService.CreateDashboardAsync(request);
        _createdDashboardId = createResult!.Id;

        var widget = Widget.CreateChartWidget("Test Chart Widget", 0, 0);
        var addWidgetResult = await _dashboardApiService.AddWidgetToDashboardAsync(createResult.Id, widget);

        Assert.NotNull(addWidgetResult);
        Assert.Equal(1, addWidgetResult.GetWidgetCount());
        Assert.Equal("Test Chart Widget", addWidgetResult.Widgets[0].Name);
    }

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

    [Fact]
    public async Task GetNonExistentDashboard_ShouldReturnError()
    {
        var result = await _dashboardApiService.GetDashboardAsync(-1);

        Assert.Null(result);
    }
}
