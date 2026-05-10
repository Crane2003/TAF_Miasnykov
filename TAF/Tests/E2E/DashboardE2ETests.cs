using TAF.Business.Services;
using TAF.Tests.Base;

namespace TAF.Tests.E2E;

[TestFixture]
public class DashboardE2ETests : BaseTest
{
    private DashboardApiService _apiService = null!;
    private DashboardUiService _uiService = null!;
    private List<string> _createdDashboardIds = null!;

    [SetUp]
    public new void Setup()
    {
        base.Setup();
        _apiService = new DashboardApiService(Configuration.BaseUrl);
        _apiService.SetAuthToken("test-auth-token");
        _uiService = new DashboardUiService(Configuration.BaseUrl);
        _createdDashboardIds = new List<string>();
    }

    [TearDown]
    public async Task CleanupAsync()
    {
        foreach (var dashboardId in _createdDashboardIds)
        {
            await _apiService.DeleteDashboardAsync(dashboardId);
        }
        _createdDashboardIds.Clear();
    }

    [Test]
    public void NavigateToDashboardPage_ShouldLoadSuccessfully()
    {
        _uiService.NavigateToDashboards();

        Assert.That(_uiService.IsPageLoaded(), Is.True);
    }

    [Test]
    public void DashboardPage_ShouldHaveAddNewDashboardButton()
    {
        _uiService.NavigateToDashboards();

        Assert.That(_uiService.IsAddNewDashboardButtonVisible(), Is.True);
    }
}
