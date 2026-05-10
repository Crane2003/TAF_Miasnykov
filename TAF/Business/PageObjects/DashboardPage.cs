using OpenQA.Selenium;
using TAF.Core.Elements;
using TAF.Core.Pages;

namespace TAF.Business.PageObjects;

public class DashboardPage : BasePage
{
    private readonly Button _addNewDashboardButton;
    private readonly Button _addNewWidgetButton;
    private readonly Button _editButton;
    private readonly Button _deleteButton;
    private readonly Button _fullScreenButton;
    private readonly BaseElement _dashboardBreadcrumb;
    private readonly BaseElement _dashboardTitle;

    public DashboardPage(IWebDriver driver, string baseUrl)
        : base(driver, $"{baseUrl}/ui")
    {
        _addNewDashboardButton = new Button(driver, By.XPath("//button[.//span[text()='Add New Dashboard']]"));
        _addNewWidgetButton = new Button(driver, By.XPath("//button[.//span[text()='Add new widget']]"));
        _editButton = new Button(driver, By.XPath("//button[.//span[text()='Edit']]"));
        _deleteButton = new Button(driver, By.XPath("//button[.//span[text()='Delete']]"));
        _fullScreenButton = new Button(driver, By.XPath("//button[.//span[text()='Full screen']]"));
        _dashboardBreadcrumb = new BaseElement(driver, By.CssSelector("ul[class*='page-breadcrumbs']"));
        _dashboardTitle = new BaseElement(driver, By.CssSelector("ul[class*='page-breadcrumbs'] li span[title]"));
    }

    public void ClickAddNewDashboard()
    {
        _addNewDashboardButton.Click();
    }

    public void ClickAddNewWidget()
    {
        _addNewWidgetButton.Click();
    }

    public void ClickEdit()
    {
        _editButton.Click();
    }

    public void ClickDelete()
    {
        _deleteButton.Click();
    }

    public void ClickFullScreen()
    {
        _fullScreenButton.Click();
    }

    public string GetDashboardTitle()
    {
        return _dashboardTitle.GetText();
    }

    public bool IsAddNewDashboardButtonDisplayed()
    {
        return _addNewDashboardButton.IsDisplayed();
    }

    public bool IsAddNewWidgetButtonDisplayed()
    {
        return _addNewWidgetButton.IsDisplayed();
    }

    public override bool IsPageLoaded()
    {
        return _dashboardBreadcrumb.IsDisplayed() && GetCurrentUrl().Contains("/dashboard");
    }
}
