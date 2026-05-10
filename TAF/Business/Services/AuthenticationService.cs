using TAF.Business.PageObjects;
using TAF.Core.Driver;

namespace TAF.Business.Services;

public class AuthenticationService
{
    private readonly LoginPage _loginPage;
    private readonly HomePage _homePage;

    public AuthenticationService(string baseUrl)
    {
        var driver = DriverManager.Driver;
        _loginPage = new LoginPage(driver, baseUrl);
        _homePage = new HomePage(driver, baseUrl);
    }

    public bool LogoutUser()
    {
        _homePage.Logout();
        return _loginPage.IsPageLoaded();
    }

    public bool VerifySuccessfulLogin()
    {
        return _homePage.IsUserAvatarDisplayed();
    }
}
