using Core.Elements;
using Core.Pages;
using OpenQA.Selenium;

namespace Business.PageObjects;

public class LoginPage : BasePage
{
    private readonly TextBox _usernameField;
    private readonly TextBox _passwordField;
    private readonly Button _loginButton;
    private readonly BaseElement _errorMessage;

    public LoginPage(IWebDriver driver, string baseUrl)
        : base(driver, $"{baseUrl}/ui/#login")
    {
        _usernameField = new TextBox(driver, By.Name("login"));
        _passwordField = new TextBox(driver, By.Name("password"));
        _loginButton = new Button(driver, By.CssSelector("button[type='submit']"));
        _errorMessage = new BaseElement(driver, By.CssSelector("div[class*='field-error-hint']"));
    }

    public void Login(string username, string password)
    {
        _logger.Information("Attempting login with username: {Username}", username);
        _usernameField.EnterText(username);
        _passwordField.EnterText(password);
        _loginButton.Click();
        _logger.Debug("Login credentials submitted");
    }

    public bool IsErrorMessageDisplayed()
    {
        var isDisplayed = _errorMessage.IsDisplayed();
        if (isDisplayed)
        {
            _logger.Warning("Login error message displayed");
        }
        return isDisplayed;
    }

    public string GetErrorMessage()
    {
        var message = _errorMessage.GetText();
        _logger.Debug("Error message text: {ErrorMessage}", message);
        return message;
    }

    public override bool IsPageLoaded()
    {
        var isLoaded = _loginButton.IsDisplayed() && GetCurrentUrl().Contains("#login");
        _logger.Debug("Login page loaded: {IsLoaded}", isLoaded);
        return isLoaded;
    }
}
