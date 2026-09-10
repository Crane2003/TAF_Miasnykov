using Business.Models;
using Business.Services;
using Core.Utilities;
using Tests.Base;
using Tests.TestData;

namespace Tests.Api;

/// <summary>
/// NUnit data-driven tests for Dashboard API operations.
/// </summary>
[TestFixture]
public class DashboardDdtNUnitTests : BaseTest
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

    [TestCaseSource(typeof(NUnitTestDataAdapter), nameof(NUnitTestDataAdapter.DashboardCreateCases))]
    public async Task CreateDashboard_WithVariousInputs_ShouldReturnMatchingData(DashboardCreateRequest request)
    {
        var result = await _dashboardApiService.CreateDashboardAsync(request);

        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.GreaterThan(0));
            Assert.That(result.Name, Is.EqualTo(request.Name));
            Assert.That(result.Description, Is.EqualTo(request.Description));
        }

        _createdDashboardId = result.Id;
    }

    [TestCaseSource(typeof(NUnitTestDataAdapter), nameof(NUnitTestDataAdapter.DashboardUpdateCases))]
    public async Task UpdateDashboard_WithVariousPayloads_ShouldReflectNewValues(DashboardCreateRequest createRequest, DashboardUpdateRequest updateRequest)
    {
        var created = await _dashboardApiService.CreateDashboardAsync(createRequest);
        Assert.That(created, Is.Not.Null);
        _createdDashboardId = created.Id;

        var updated = await _dashboardApiService.UpdateDashboardAsync(created.Id, updateRequest);

        Assert.That(updated, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(updated.Name, Is.EqualTo(updateRequest.Name));
            Assert.That(updated.Description, Is.EqualTo(updateRequest.Description));
        }
    }

    [TestCaseSource(typeof(NUnitTestDataAdapter), nameof(NUnitTestDataAdapter.WidgetCases))]
    public async Task AddWidget_WithVariousConfigurations_ShouldBeStoredCorrectly(Widget widget)
    {
        // Widget names are unique per project, and the case data is shared between
        // fixtures, so work against a uniquely named copy.
        widget = new Widget
        {
            Name = widget.Name.Unique(),
            Type = widget.Type,
            Size = widget.Size,
            Position = widget.Position
        };

        var createRequest = DashboardCreateRequest.CreateDefault();
        var dashboard = await _dashboardApiService.CreateDashboardAsync(createRequest);
        Assert.That(dashboard, Is.Not.Null);
        _createdDashboardId = dashboard.Id;

        var result = await _dashboardApiService.AddWidgetToDashboardAsync(dashboard.Id, widget);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.GetWidgetCount(), Is.EqualTo(1));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Widgets[0].Name, Is.EqualTo(widget.Name));
            Assert.That(result.Widgets[0].Type, Is.EqualTo(widget.Type));
            Assert.That(result.Widgets[0].Size.Width, Is.EqualTo(widget.Size.Width));
            Assert.That(result.Widgets[0].Size.Height, Is.EqualTo(widget.Size.Height));
        }
    }
}
