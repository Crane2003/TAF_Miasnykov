using Business.Models;
using Business.Services;
using Tests.Base;

namespace Tests.E2E;

[TestFixture]
public class DashboardE2ETests : UiBaseTest
{
    private DashboardApiService _apiService = null!;
    private DashboardUiService _uiService = null!;
    private AuthenticationService _authService = null!;
    private int? _createdDashboardId;

    [SetUp]
    public void InitServices()
    {
        _apiService = new DashboardApiService(Configuration.ProjectName);
        _uiService = new DashboardUiService(Configuration.BaseUrl);
        _authService = new AuthenticationService(Configuration.BaseUrl);
        _createdDashboardId = null;

        _authService.NavigateToLogin();
        _authService.Login(Credentials.DefaultUser.Username, Credentials.DefaultUser.Password);
    }

    [TearDown]
    public async Task CleanupAsync()
    {
        if (_createdDashboardId.HasValue)
            await _apiService.DeleteDashboardAsync(_createdDashboardId.Value);
    }

    [Test]
    public void DashboardPage_ShouldLoadSuccessfully()
    {
        _uiService.NavigateToDashboards();

        Assert.That(_uiService.IsPageLoaded(), Is.True);
    }

    [Test]
    public void DashboardPage_ShouldDisplayAddNewDashboardButton()
    {
        _uiService.NavigateToDashboards();

        Assert.That(_uiService.IsAddNewDashboardButtonVisible(), Is.True);
    }

    [Test]
    public void DashboardPage_ShouldDisplayAddNewWidgetButton()
    {
        _uiService.NavigateToDashboards();

        Assert.That(_uiService.IsAddNewWidgetButtonVisible(), Is.True);
    }

    [Test]
    public async Task CreateDashboardViaApi_ThenVerifyItAppearsInUi()
    {
        var request = DashboardCreateRequest.CreateWithName($"E2E Dashboard {DateTime.Now:HHmmss}");
        var createResult = await _apiService.CreateDashboardAsync(request);

        Assert.That(createResult, Is.Not.Null);
        _createdDashboardId = createResult!.Id;

        _uiService.NavigateToDashboards();

        Assert.Multiple(() =>
        {
            Assert.That(_uiService.IsPageLoaded(), Is.True);
            Assert.That(_uiService.IsDashboardNameVisible(request.Name), Is.True);
        });
    }
}
