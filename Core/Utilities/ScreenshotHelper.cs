using OpenQA.Selenium;

namespace Core.Utilities;

public static class ScreenshotHelper
{
    public static void TakeScreenshot(IWebDriver driver, string testName)
    {
        var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
        var screenshotDir = Path.Combine(Directory.GetCurrentDirectory(), "Screenshots");

        if (!Directory.Exists(screenshotDir))
        {
            Directory.CreateDirectory(screenshotDir);
        }

        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var fileName = $"{testName}_{timestamp}.png";
        var filePath = Path.Combine(screenshotDir, fileName);

        screenshot.SaveAsFile(filePath);
    }
}
