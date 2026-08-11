using Business.Models;
using Business.Services;
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
            _apiService = new DashboardApiService(ApiConfiguration.BaseUrl, Configuration.ProjectName);
            _apiService.SetAuthToken(ApiConfiguration.AuthToken);
            _uiService = new DashboardUiService(Configuration.BaseUrl);
            _authService = new AuthenticationService(Configuration.BaseUrl);
            _createdDashboardId = null;

            _authService.NavigateToLogin();
            _authService.Login(Credentials.DefaultUser.Username, Credentials.DefaultUser.Password);
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
    public void DashboardPage_ShouldLoadSuccessfully()
    {
        _uiService.NavigateToDashboards();

        Assert.True(_uiService.IsPageLoaded());
    }

    [Fact]
    public void DashboardPage_ShouldDisplayAddNewDashboardButton()
    {
        _uiService.NavigateToDashboards();

        Assert.True(_uiService.IsAddNewDashboardButtonVisible());
    }

    [Fact]
    public void DashboardPage_ShouldDisplayAddNewWidgetButton()
    {
        _uiService.NavigateToDashboards();

        Assert.True(_uiService.IsAddNewWidgetButtonVisible());
    }

    [Fact]
    public async Task CreateDashboardViaApi_ThenVerifyItAppearsInUi()
    {
        var request = DashboardCreateRequest.CreateWithName($"E2E Dashboard {DateTime.Now:HHmmss}");
        var createResult = await _apiService.CreateDashboardAsync(request);

        Assert.NotNull(createResult);
        _createdDashboardId = createResult.Id;

        _uiService.NavigateToDashboards();

        Assert.True(_uiService.IsPageLoaded());
        Assert.True(_uiService.IsDashboardNameVisible(request.Name));
    }
}
