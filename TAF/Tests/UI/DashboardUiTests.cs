using TAF.Business.PageObjects;
using TAF.Core.Driver;
using TAF.Tests.Base;

namespace TAF.Tests.UI;

[TestFixture]
public class DashboardUiTests : BaseTest
{
    private LoginPage _loginPage = null!;
    private DashboardPage _dashboardPage = null!;

    [SetUp]
    public new void Setup()
    {
        base.Setup();
        _loginPage = new LoginPage(DriverManager.Driver, Configuration.BaseUrl);
        _dashboardPage = new DashboardPage(DriverManager.Driver, Configuration.BaseUrl);
    }

    [Test]
    public void NavigateToDashboard_AfterLogin_ShouldDisplayDashboardPage()
    {
        _loginPage.Open();
        _loginPage.Login(Credentials.DefaultUser.Username, Credentials.DefaultUser.Password);

        Assert.That(_dashboardPage.IsPageLoaded(), Is.True);
    }

    [Test]
    public void DashboardPage_ShouldDisplayAddNewDashboardButton()
    {
        _loginPage.Open();
        _loginPage.Login(Credentials.DefaultUser.Username, Credentials.DefaultUser.Password);

        Assert.That(_dashboardPage.IsAddNewDashboardButtonDisplayed(), Is.True);
    }

    [Test]
    public void DashboardPage_ShouldDisplayAddNewWidgetButton()
    {
        _loginPage.Open();
        _loginPage.Login(Credentials.DefaultUser.Username, Credentials.DefaultUser.Password);

        Assert.That(_dashboardPage.IsAddNewWidgetButtonDisplayed(), Is.True);
    }
}
