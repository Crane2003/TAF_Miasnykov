using Core.Elements;
using OpenQA.Selenium;

namespace Business.PageObjects;

public class AddDashboardModal
{
    private const string ModalContainerXPath = "//div[contains(@class,'modal-window')]";

    private readonly ILogger _logger = Log.ForContext<AddDashboardModal>();
    private readonly TextBox _nameInput;
    private readonly TextBox _descriptionTextArea;
    private readonly Button _submitButton;

    public AddDashboardModal(IWebDriver driver)
    {
        _nameInput = new TextBox(driver, By.XPath($"{ModalContainerXPath}//input[@placeholder='Enter dashboard name']"));
        _descriptionTextArea = new TextBox(driver, By.XPath($"{ModalContainerXPath}//textarea[@placeholder='Enter dashboard description']"));
        _submitButton = new Button(driver, By.XPath($"{ModalContainerXPath}//button[text()='Add' or text()='Update']"));
    }

    public void EnterName(string name)
    {
        _logger.Information("Entering dashboard name: {Name}", name);
        _nameInput.ClearText();
        _nameInput.EnterText(name);
    }

    public void EnterDescription(string description)
    {
        _logger.Information("Entering dashboard description");
        _descriptionTextArea.ClearText();
        _descriptionTextArea.EnterText(description);
    }

    public void ClickSubmit()
    {
        _logger.Information("Clicking Add/Update button in Add Dashboard modal");
        _submitButton.Click();
    }

    public void CreateDashboard(string name, string? description = null)
    {
        EnterName(name);
        if (!string.IsNullOrEmpty(description))
        {
            EnterDescription(description);
        }
        ClickSubmit();
    }
}
