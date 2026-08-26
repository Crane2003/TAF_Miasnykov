using Business.Models;
using Business.Services;
using Core.Utilities;
using Tests.Base;
using Tests.TestData;
using Xunit;
using Assert = Xunit.Assert;
using TheoryAttribute = Xunit.TheoryAttribute;

namespace Tests.Api;

public class DashboardDdtXUnitTests : XUnitBaseTest, IAsyncLifetime, IClassFixture<XUnitTestFixture>
{
    private readonly DashboardApiService _dashboardApiService = null!;
    private int? _createdDashboardId;

    public DashboardDdtXUnitTests(XUnitTestFixture fixture) : base(fixture)
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

    [Theory]
    [MemberData(nameof(XUnitTestDataAdapter.DashboardCreateCases), MemberType = typeof(XUnitTestDataAdapter))]
    public async Task CreateDashboard_WithVariousInputs_ShouldReturnMatchingData(DashboardCreateRequest request)
    {
        var result = await _dashboardApiService.CreateDashboardAsync(request);

        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal(request.Name, result.Name);
        Assert.Equal(request.Description, result.Description);

        _createdDashboardId = result.Id;
    }

    [Theory]
    [MemberData(nameof(XUnitTestDataAdapter.DashboardUpdateCases), MemberType = typeof(XUnitTestDataAdapter))]
    public async Task UpdateDashboard_WithVariousPayloads_ShouldReflectNewValues(DashboardCreateRequest createRequest, DashboardUpdateRequest updateRequest)
    {
        var created = await _dashboardApiService.CreateDashboardAsync(createRequest);
        Assert.NotNull(created);
        _createdDashboardId = created.Id;

        var updated = await _dashboardApiService.UpdateDashboardAsync(created.Id, updateRequest);

        Assert.NotNull(updated);
        Assert.Equal(updateRequest.Name, updated.Name);
        Assert.Equal(updateRequest.Description, updated.Description);
    }

    [Theory]
    [MemberData(nameof(XUnitTestDataAdapter.WidgetCases), MemberType = typeof(XUnitTestDataAdapter))]
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
        Assert.NotNull(dashboard);
        _createdDashboardId = dashboard.Id;

        var result = await _dashboardApiService.AddWidgetToDashboardAsync(dashboard.Id, widget);

        Assert.NotNull(result);
        Assert.Equal(1, result.GetWidgetCount());
        Assert.Equal(widget.Name, result.Widgets[0].Name);
        Assert.Equal(widget.Type, result.Widgets[0].Type);
        Assert.Equal(widget.Size.Width, result.Widgets[0].Size.Width);
        Assert.Equal(widget.Size.Height, result.Widgets[0].Size.Height);
    }
}
