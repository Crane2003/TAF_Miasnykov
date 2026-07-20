using Core.Elements;
using Core.Pages;
using OpenQA.Selenium;

namespace Business.PageObjects;

public class HomePage : BasePage
{
    private readonly BaseElement _dashboardsLink;
    private readonly BaseElement _logoutLink;
    private readonly BaseElement _userAvatar;
    private readonly BaseElement _projectSelector;

    public HomePage(IWebDriver driver, string baseUrl)
        : base(driver, $"{baseUrl}/ui")
    {
        _dashboardsLink = new BaseElement(driver, By.CssSelector("a[href*='/dashboard']"));
        _logoutLink = new BaseElement(driver, By.CssSelector("a[href='#login']"));
        _userAvatar = new BaseElement(driver, By.CssSelector("div[class*='user-block'] img[alt='avatar']"));
        _projectSelector = new BaseElement(driver, By.CssSelector("div[class*='current-project-name']"));
    }

    public string GetCurrentProjectName()
    {
        return _projectSelector.GetText();
    }

    public void Logout()
    {
        _logoutLink.Click();
    }

    public void NavigateToDashboards()
    {
        _dashboardsLink.Click();
    }

    public bool IsUserAvatarDisplayed()
    {
        return _userAvatar.IsDisplayed();
    }

    public override bool IsPageLoaded()
    {
        return _userAvatar.IsDisplayed() && _logoutLink.IsDisplayed();
    }
}
