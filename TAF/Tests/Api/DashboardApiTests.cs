using TAF.Business.Models;
using TAF.Business.Services;
using TAF.Tests.Base;
using TAF.Tests.Helpers;

namespace TAF.Tests.Api;

[TestFixture]
public class DashboardApiTests : BaseTest
{
    private DashboardApiService _dashboardApiService = null!;
    private List<string> _createdDashboardIds = null!;

    [SetUp]
    public new void Setup()
    {
        base.Setup();
        _dashboardApiService = new DashboardApiService(Configuration.BaseUrl);
        _dashboardApiService.SetAuthToken("test-auth-token");
        _createdDashboardIds = new List<string>();
    }

    [TearDown]
    public async Task CleanupAsync()
    {
        foreach (var dashboardId in _createdDashboardIds)
        {
            await _dashboardApiService.DeleteDashboardAsync(dashboardId);
        }
        _createdDashboardIds.Clear();
    }

    [Test]
    public async Task CreateDashboard_ViaPostRequest_ShouldSucceed()
    {
        var dashboard = Dashboard.CreateDefault();

        var result = await _dashboardApiService.CreateDashboardAsync(dashboard);

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.True);
            Assert.That(result.Dashboard, Is.Not.Null);
            Assert.That(result.Dashboard!.Id, Is.Not.Null.And.Not.Empty);
            Assert.That(result.Dashboard.Name, Is.EqualTo(dashboard.Name));
            Assert.That(result.Dashboard.Description, Is.EqualTo(dashboard.Description));
        });

        _createdDashboardIds.Add(result.Dashboard!.Id!);
    }

    [Test]
    public async Task GetDashboard_ViaGetRequest_ShouldReturnDashboard()
    {
        var dashboard = Dashboard.CreateDefault();
        var createResult = await _dashboardApiService.CreateDashboardAsync(dashboard);
        _createdDashboardIds.Add(createResult.Dashboard!.Id!);

        var getResult = await _dashboardApiService.GetDashboardAsync(createResult.Dashboard.Id!);

        Assert.Multiple(() =>
        {
            Assert.That(getResult.Success, Is.True);
            Assert.That(getResult.Dashboard, Is.Not.Null);
            Assert.That(getResult.Dashboard!.Id, Is.EqualTo(createResult.Dashboard.Id));
            Assert.That(getResult.Dashboard.Name, Is.EqualTo(dashboard.Name));
        });
    }

    [Test]
    public async Task UpdateDashboard_ViaPutRequest_ShouldModifyDashboard()
    {
        var dashboard = Dashboard.CreateDefault();
        var createResult = await _dashboardApiService.CreateDashboardAsync(dashboard);
        _createdDashboardIds.Add(createResult.Dashboard!.Id!);

        var updatedDashboard = createResult.Dashboard;
        updatedDashboard!.Name = "Updated Dashboard Name";
        updatedDashboard.Description = "Updated description";

        var updateResult = await _dashboardApiService.UpdateDashboardAsync(updatedDashboard.Id!, updatedDashboard);

        Assert.Multiple(() =>
        {
            Assert.That(updateResult.Success, Is.True);
            Assert.That(updateResult.Dashboard!.Name, Is.EqualTo("Updated Dashboard Name"));
            Assert.That(updateResult.Dashboard.Description, Is.EqualTo("Updated description"));
        });
    }

    [Test]
    public async Task AddWidget_ViaPutRequest_ShouldAddWidgetToDashboard()
    {
        var dashboard = Dashboard.CreateDefault();
        var createResult = await _dashboardApiService.CreateDashboardAsync(dashboard);
        _createdDashboardIds.Add(createResult.Dashboard!.Id!);

        var widget = Widget.CreateChartWidget("Test Chart Widget", 0);
        var addWidgetResult = await _dashboardApiService.AddWidgetToDashboardAsync(createResult.Dashboard.Id!, widget);

        Assert.Multiple(() =>
        {
            Assert.That(addWidgetResult.Success, Is.True);
            Assert.That(addWidgetResult.Dashboard, Is.Not.Null);
            Assert.That(addWidgetResult.Dashboard!.GetWidgetCount(), Is.EqualTo(1));
            Assert.That(addWidgetResult.Dashboard.Widgets[0].Name, Is.EqualTo("Test Chart Widget"));
        });
    }

    [Test]
    public async Task RemoveWidget_ViaDeleteRequest_ShouldRemoveWidgetFromDashboard()
    {
        var dashboard = Dashboard.CreateWithWidgets(2);
        var createResult = await _dashboardApiService.CreateDashboardAsync(dashboard);
        _createdDashboardIds.Add(createResult.Dashboard!.Id!);

        var widgetToRemove = createResult.Dashboard.Widgets[0];
        var (Success, Message) = await _dashboardApiService.RemoveWidgetFromDashboardAsync(createResult.Dashboard.Id!, widgetToRemove.Id!);

        Assert.That(Success, Is.True, "Removing widget should succeed");

        var verifyResult = await _dashboardApiService.GetDashboardAsync(createResult.Dashboard.Id!);
        Assert.Multiple(() =>
        {
            Assert.That(verifyResult.Dashboard!.GetWidgetCount(), Is.EqualTo(1));
            Assert.That(verifyResult.Dashboard.GetWidgetById(widgetToRemove.Id!), Is.Null);
        });
    }

    [Test]
    public async Task DeleteDashboard_ViaDeleteRequest_ShouldRemoveDashboard()
    {
        var dashboard = Dashboard.CreateDefault();
        var createResult = await _dashboardApiService.CreateDashboardAsync(dashboard);
        var dashboardId = createResult.Dashboard!.Id!;
        var (Success, _) = await _dashboardApiService.DeleteDashboardAsync(dashboardId);
        Assert.That(Success, Is.True);

        var verifyResult = await DashboardApiTestHelpers.VerifyDashboardExistsAsync(_dashboardApiService, dashboardId);
        Assert.That(verifyResult, Is.False);
    }

    [Test]
    [TestCase(1)]
    [TestCase(3)]
    [TestCase(5)]
    public async Task AddMultipleWidgets_ShouldSucceed(int widgetCount)
    {
        var dashboard = Dashboard.CreateDefault();
        var createResult = await _dashboardApiService.CreateDashboardAsync(dashboard);
        _createdDashboardIds.Add(createResult.Dashboard!.Id!);

        for (int i = 0; i < widgetCount; i++)
        {
            var widget = Widget.CreateChartWidget($"Widget {i + 1}", i);
            await _dashboardApiService.AddWidgetToDashboardAsync(createResult.Dashboard.Id!, widget);
        }

        var getResult = await _dashboardApiService.GetDashboardAsync(createResult.Dashboard.Id!);
        Assert.That(getResult.Dashboard!.GetWidgetCount(), Is.EqualTo(widgetCount));
    }

    [Test]
    public async Task GetNonExistentDashboard_ShouldReturnError()
    {
        var nonExistentId = Guid.NewGuid().ToString();

        var result = await _dashboardApiService.GetDashboardAsync(nonExistentId);

        Assert.That(result.Success, Is.False);
    }
}
