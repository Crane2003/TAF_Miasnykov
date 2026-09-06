using Core.Driver;

namespace Tests.Base;

public class UiBaseTest : BaseTest
{
    [SetUp]
    public void InitDriver()
    {
        UiTestSupport.InitDriver(Configuration);
    }

    [TearDown]
    public override void TearDown()
    {
        var testName = TestContext.CurrentContext.Test.Name;
        var testFailed = TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed;

        UiTestSupport.QuitWithScreenshot(
            DriverManager.CurrentDriver,
            testName,
            testFailed,
            Configuration.TakeScreenshotOnFailure,
            Logger);

        base.TearDown();
    }
}
