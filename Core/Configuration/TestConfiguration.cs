using Core.Driver;

namespace Core.Configuration;

public class TestConfiguration
{
    public string BaseUrl { get; set; } = "http://localhost:8080/";
    public string ProjectName { get; set; } = "superadmin_personal";
    public string Browser { get; set; } = "Chrome";
    public int ImplicitWaitTimeout { get; set; } = 10;
    public int PageLoadTimeout { get; set; } = 30;
    public int ScriptTimeout { get; set; } = 30;
    public bool TakeScreenshotOnFailure { get; set; } = true;
    public bool Headless { get; set; } = true;

    public BrowserType GetBrowserType()
    {
        return Enum.TryParse<BrowserType>(Browser, true, out var browserType)
            ? browserType
            : BrowserType.Chrome;
    }
}
