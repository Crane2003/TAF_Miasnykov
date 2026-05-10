using TAF.Core.Driver;

namespace TAF.Core.Configuration;

public class TestConfiguration
{
    public string BaseUrl { get; set; } = "http://localhost:8080/";
    public string Browser { get; set; } = "Chrome";
    public int ImplicitWaitTimeout { get; set; } = 10;
    public int PageLoadTimeout { get; set; } = 30;
    public int ScriptTimeout { get; set; } = 30;
    public bool TakeScreenshotOnFailure { get; set; } = true;

    public BrowserType GetBrowserType()
    {
        return Enum.TryParse<BrowserType>(Browser, true, out var browserType)
            ? browserType
            : BrowserType.Chrome;
    }
}
