using TAF.Business.PageObjects;
using TAF.Core.Driver;
using TAF.Tests.Base;

namespace TAF.Tests.Functional;

[TestFixture]
public class LoginTests : BaseTest
{
    private LoginPage _loginPage = null!;
    private HomePage _homePage = null!;

    [SetUp]
    public new void Setup()
    {
        base.Setup();
        _loginPage = new LoginPage(DriverManager.Driver, Configuration.BaseUrl);
        _homePage = new HomePage(DriverManager.Driver, Configuration.BaseUrl);
    }

    [Test]
    [Category("Negative")]
    [TestCase("", "password123", Description = "Empty username")]
    [TestCase("username", "", Description = "Empty password")]
    [TestCase("", "", Description = "Empty both fields")]
    public void LoginWithEmptyFields_ShouldShowError(string username, string password)
    {
        _loginPage.Open();
        _loginPage.Login(username, password);

        Assert.That(_loginPage.IsErrorMessageDisplayed(), Is.True);
    }

    [Test]
    public void LoginWithValidCredentials_ShouldSucceed()
    {
        _loginPage.Open();
        _loginPage.Login(Credentials.DefaultUser.Username, Credentials.DefaultUser.Password);

        Assert.That(_homePage.IsPageLoaded(), Is.True);
    }
}
