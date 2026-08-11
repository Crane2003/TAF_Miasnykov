using Core.Configuration;
using Core.Logging;

namespace Tests.Base;

public sealed class XUnitTestFixture : IDisposable
{
    public TestConfiguration Configuration { get; }
    public CredentialsConfiguration Credentials { get; }
    public ApiConfiguration ApiConfiguration { get; }
    public ILogger Logger { get; }

    public XUnitTestFixture()
    {
        LoggerSetup.Initialize();
        Logger = Log.ForContext<XUnitTestFixture>();

        var configLoader = ConfigurationLoader.Instance;
        Configuration = configLoader.GetTestConfiguration();
        Credentials = configLoader.GetCredentials();
        ApiConfiguration = configLoader.GetApiConfiguration();

        if (string.IsNullOrWhiteSpace(Configuration.ProjectName))
            throw new InvalidOperationException("ProjectName must be configured in appsettings.json under TestConfiguration.");

        Logger.Information("XUnit test fixture initialized");
    }

    public void Dispose()
    {
        Logger.Information("XUnit test fixture disposed");
    }
}
