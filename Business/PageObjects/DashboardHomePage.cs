using Core.Elements;
using Core.Pages;
using OpenQA.Selenium;

namespace Business.PageObjects;

public class DashboardHomePage : BasePage
{
    private static By DashboardsSidebarLinkLocator => By.XPath("//aside[contains(@class,'sidebar')]//a[contains(@href,'/dashboard')][.//span[normalize-space(.)='Dashboards']]");
    private static By AddNewDashboardButtonLocator => By.XPath("//div[contains(@class,'addDashboardButton')]//button[.//span[normalize-space(.)='Add New Dashboard']]");
    private static By DashboardBreadcrumbLocator => By.CssSelector("ul[class*='page-breadcrumbs']");

    private readonly Button _dashboardsSidebarButton;
    private readonly Button _addNewDashboardButton;
    private readonly AddDashboardModal _addDashboardModal;

    public DashboardHomePage(IWebDriver driver, string baseUrl)
        : base(driver, $"{baseUrl}/ui")
    {
        _dashboardsSidebarButton = new Button(driver, DashboardsSidebarLinkLocator);
        _addNewDashboardButton = new Button(driver, AddNewDashboardButtonLocator);
        _addDashboardModal = new AddDashboardModal(driver);
    }

    public void NavigateViaSidebar()
    {
        _dashboardsSidebarButton.Click();
    }

    public void ClickAddNewDashboard()
    {
        _addNewDashboardButton.Click();
    }

    public void CreateDashboard(string name, string? description = null)
    {
        ClickAddNewDashboard();
        _addDashboardModal.CreateDashboard(name, description);
    }

    public AddDashboardModal GetAddDashboardModal()
    {
        return _addDashboardModal;
    }

    public override bool IsPageLoaded()
    {
        return GetCurrentUrl().Contains("/dashboard");
    }

    public bool IsDashboardNameVisible(string name)
    {
        try
        {
            return new BaseElement(_driver, DashboardNameOnHomePageLocator(name), timeoutInSeconds: 5).IsDisplayed();
        }
        catch
        {
            return false;
        }
    }

    public void OpenDashboard(string name)
    {
        new Button(_driver, DashboardRowNameLinkLocator(name)).Click();
    }

    private static By DashboardNameOnHomePageLocator(string name)
        => By.XPath($"//div[contains(@class,'gridRow__grid-row-wrapper')]//a[contains(@class,'dashboardTable__name')][normalize-space(.)='{name}']");

    private static string DashboardRowXPath(string name)
        => $"//div[contains(@class,'gridRow__grid-row-wrapper')][.//a[contains(@class,'dashboardTable__name')][@title='{name}' or normalize-space(.)='{name}']]";

    private static By DashboardRowNameLinkLocator(string name)
        => By.XPath($"{DashboardRowXPath(name)}//div[contains(@class,'dashboardTable__name-container')]//a[contains(@class,'dashboardTable__name')]");
}
