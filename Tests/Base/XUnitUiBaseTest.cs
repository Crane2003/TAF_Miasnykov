using Core.Driver;
using Xunit;

namespace Tests.Base;

public abstract class XUnitUiBaseTest : XUnitBaseTest
{
    private bool _setupFailed;
    protected bool SetupFailed { get => _setupFailed; set => _setupFailed = value; }

    protected XUnitUiBaseTest(XUnitTestFixture fixture) : base(fixture)
    {
    }

    public override ValueTask InitializeAsync()
    {
        try
        {
            UiTestSupport.InitDriver(Configuration);
        }
        catch
        {
            _setupFailed = true;
            throw;
        }
        return ValueTask.CompletedTask;
    }

    public override ValueTask DisposeAsync()
    {
        var testName = Xunit.TestContext.Current.Test!.TestDisplayName;
        var testFailed = _setupFailed || Xunit.TestContext.Current.TestState?.Result == TestResult.Failed;

        UiTestSupport.QuitWithScreenshot(
            DriverManager.Driver,
            testName,
            testFailed,
            Configuration.TakeScreenshotOnFailure,
            Logger);

        return base.DisposeAsync();
    }
}
