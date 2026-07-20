using Business.PageObjects;
using Core.Driver;

namespace Business.Services;

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

    public void NavigateToLogin()
    {
        _loginPage.Open();
    }

    public void Login(string username, string password)
    {
        _loginPage.Login(username, password);
    }

    public bool IsLoginSuccessful()
    {
        return _homePage.IsPageLoaded();
    }

    public bool IsLoginErrorDisplayed()
    {
        return _loginPage.IsErrorMessageDisplayed();
    }

    public string GetLoginErrorMessage()
    {
        return _loginPage.GetErrorMessage();
    }

    public bool Logout()
    {
        _homePage.Logout();
        return _loginPage.IsPageLoaded();
    }

    public bool IsUserAvatarDisplayed()
    {
        return _homePage.IsUserAvatarDisplayed();
    }
}
