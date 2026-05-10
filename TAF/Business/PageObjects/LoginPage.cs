using OpenQA.Selenium;
using TAF.Core.Elements;
using TAF.Core.Pages;

namespace TAF.Business.PageObjects;

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
        _usernameField.EnterText(username);
        _passwordField.EnterText(password);
        _loginButton.Click();
    }

    public bool IsErrorMessageDisplayed()
    {
        return _errorMessage.IsDisplayed();
    }

    public string GetErrorMessage()
    {
        return _errorMessage.GetText();
    }

    public override bool IsPageLoaded()
    {
        return _loginButton.IsDisplayed() && GetCurrentUrl().Contains("#login");
    }
}
