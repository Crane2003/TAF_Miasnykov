using Core.Configuration;
using Core.Logging;

namespace Tests.Base;

public class BaseTest
{
    protected TestConfiguration Configuration { get; private set; } = null!;
    protected CredentialsConfiguration Credentials { get; private set; } = null!;
    protected ApiConfiguration ApiConfiguration { get; private set; } = null!;
    protected ILogger Logger { get; private set; } = null!;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        LoggerSetup.Initialize();
        Logger = Log.ForContext<BaseTest>();

        var configLoader = ConfigurationLoader.Instance;
        Configuration = configLoader.GetTestConfiguration();
        Credentials = configLoader.GetCredentials();
        ApiConfiguration = configLoader.GetApiConfiguration();

        if (string.IsNullOrWhiteSpace(Configuration.ProjectName))
            throw new InvalidOperationException("ProjectName must be configured in appsettings.json under TestConfiguration.");
    }

    [SetUp]
    public void Setup()
    {
        Logger.Information("Starting test: {TestName}", TestContext.CurrentContext.Test.Name);
    }

    [TearDown]
    public virtual void TearDown()
    {
        var testName = TestContext.CurrentContext.Test.Name;
        var testStatus = TestContext.CurrentContext.Result.Outcome.Status;
        Logger.Information("Test completed: {TestName} - Status: {TestStatus}", testName, testStatus);

        if (testStatus == NUnit.Framework.Interfaces.TestStatus.Failed)
        {
            Logger.Error("Test failed: {TestName} - Message: {ErrorMessage}",
                testName, TestContext.CurrentContext.Result.Message);
        }
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        Logger.Information("Test execution completed");
    }
}
