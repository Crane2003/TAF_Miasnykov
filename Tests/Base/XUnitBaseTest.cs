using Core.Configuration;
using Xunit;

namespace Tests.Base;

public abstract class XUnitBaseTest : IAsyncLifetime
{
    protected TestConfiguration Configuration { get; }
    protected CredentialsConfiguration Credentials { get; }
    protected ApiConfiguration ApiConfiguration { get; }
    protected ILogger Logger { get; }

    protected XUnitBaseTest(XUnitTestFixture fixture)
    {
        Configuration = fixture.Configuration;
        Credentials = fixture.Credentials;
        ApiConfiguration = fixture.ApiConfiguration;
        Logger = fixture.Logger;

        Logger.Information("[Setup] Test started: {0}", Xunit.TestContext.Current.Test!.TestDisplayName);
    }

    public virtual ValueTask InitializeAsync() => ValueTask.CompletedTask;

    public virtual ValueTask DisposeAsync()
    {
        Logger.Information("[Teardown] Test completed: {0}", Xunit.TestContext.Current.Test!.TestDisplayName);
        return ValueTask.CompletedTask;
    }
}
