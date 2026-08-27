using Business.Models;
using Business.Services;
using Tests.Base;
using Tests.Helpers;

namespace Tests.Api;

[TestFixture]
public class DashboardApiTests : BaseTest
{
    private DashboardApiService _dashboardApiService = null!;
    private int? _createdDashboardId;

    [SetUp]
    public void InitApiService()
    {
        _dashboardApiService = new DashboardApiService(Configuration.ProjectName);
        _createdDashboardId = null;
    }

    [TearDown]
    public async Task CleanupAsync()
    {
        if (_createdDashboardId.HasValue)
            await _dashboardApiService.DeleteDashboardAsync(_createdDashboardId.Value);
    }

    [Test]
    public async Task CreateDashboard_ViaPostRequest_ShouldSucceed()
    {
        var request = DashboardCreateRequest.CreateDefault();

        var result = await _dashboardApiService.CreateDashboardAsync(request);

        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.GreaterThan(0));
            Assert.That(result.Name, Is.EqualTo(request.Name));
            Assert.That(result.Description, Is.EqualTo(request.Description));
        }

        _createdDashboardId = result!.Id;
    }

    [Test]
    public async Task UpdateDashboard_ViaPutRequest_ShouldModifyDashboard()
    {
        var request = DashboardCreateRequest.CreateDefault();
        var createResult = await _dashboardApiService.CreateDashboardAsync(request);
        _createdDashboardId = createResult!.Id;

        var updatedName = $"Updated Dashboard Name {Guid.NewGuid()}";
        var updateRequest = new DashboardUpdateRequest
        {
            Name = updatedName,
            Description = "Updated description"
        };

        var updateResult = await _dashboardApiService.UpdateDashboardAsync(createResult.Id, updateRequest);

        Assert.That(updateResult, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(updateResult.Name, Is.EqualTo(updatedName));
            Assert.That(updateResult.Description, Is.EqualTo("Updated description"));
        }
    }

    [Test]
    public async Task AddWidget_ViaPutRequest_ShouldAddWidgetToDashboard()
    {
        var request = DashboardCreateRequest.CreateDefault();
        var createResult = await _dashboardApiService.CreateDashboardAsync(request);
        _createdDashboardId = createResult!.Id;

        var widget = Widget.CreateChartWidget($"Test Chart Widget {Guid.NewGuid()}", 0, 0);
        var addWidgetResult = await _dashboardApiService.AddWidgetToDashboardAsync(createResult.Id, widget);

        Assert.That(addWidgetResult, Is.Not.Null);
        Assert.That(addWidgetResult!.GetWidgetCount(), Is.EqualTo(1));
        Assert.That(addWidgetResult.Widgets[0].Name, Is.EqualTo(widget.Name));
    }

    [Test]
    public async Task RemoveWidget_ViaDeleteRequest_ShouldRemoveWidgetFromDashboard()
    {
        var request = DashboardCreateRequest.CreateDefault();
        var createResult = await _dashboardApiService.CreateDashboardAsync(request);
        _createdDashboardId = createResult!.Id;

        var widget1 = Widget.CreateChartWidget($"Widget 1 {Guid.NewGuid()}", 0, 0);
        var widget2 = Widget.CreateChartWidget($"Widget 2 {Guid.NewGuid()}", 1, 0);
        await _dashboardApiService.AddWidgetToDashboardAsync(createResult.Id, widget1);
        var addResult = await _dashboardApiService.AddWidgetToDashboardAsync(createResult.Id, widget2);

        var widgetToRemove = addResult!.Widgets[0];
        var removed = await _dashboardApiService.RemoveWidgetFromDashboardAsync(
            createResult.Id, widgetToRemove.Id!.Value);

        Assert.That(removed, Is.True);

        var verifyResult = await _dashboardApiService.GetDashboardAsync(createResult.Id);
        Assert.That(verifyResult!.GetWidgetCount(), Is.EqualTo(1));
        Assert.That(verifyResult.GetWidgetById(widgetToRemove.Id!.Value), Is.Null);
    }

    [Test]
    public async Task DeleteDashboard_ViaDeleteRequest_ShouldRemoveDashboard()
    {
        var request = DashboardCreateRequest.CreateDefault();
        var createResult = await _dashboardApiService.CreateDashboardAsync(request);
        Assert.That(createResult, Is.Not.Null);
        var dashboardId = createResult.Id;

        var deleted = await _dashboardApiService.DeleteDashboardAsync(dashboardId);
        Assert.That(deleted, Is.True);

        var exists = await DashboardApiTestHelpers.VerifyDashboardExistsAsync(_dashboardApiService, dashboardId);
        Assert.That(exists, Is.False);
    }

    [Test]
    public async Task GetNonExistentDashboard_ShouldReturnError()
    {
        var result = await _dashboardApiService.GetDashboardAsync(-1);

        Assert.That(result, Is.Null);
    }
}
