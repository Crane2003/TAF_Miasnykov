using System.Xml.Linq;
using NUnit.Engine;
using NUnit.Engine.Extensibility;
using ReportPortal.NUnitExtension;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Tests.ReportPortalCustomization;

/// <summary>
/// Restricts ReportPortal reporting to UI tests only (tests in the "Tests.E2E" namespace),
/// while still allowing the full NUnit test suite to run in a single "dotnet test" invocation.
/// Non-UI tests are prevented from being reported to ReportPortal by cancelling the
/// corresponding ReportPortalListener events.
/// </summary>
[Extension]
public class UiOnlyReportPortalFilter : ITestEventListener
{
    private const string UiNamespacePrefix = "Tests.E2E.";
    private static readonly ILogger Logger = Log.ForContext<UiOnlyReportPortalFilter>();

    public UiOnlyReportPortalFilter()
    {
        Logger.Information("UiOnlyReportPortalFilter: Initializing extension");
        ReportPortalListener.BeforeTestStarted += ReportPortalListener_BeforeTestStarted;
        ReportPortalListener.BeforeTestFinished += ReportPortalListener_BeforeTestFinished;
        Logger.Information("UiOnlyReportPortalFilter: Event handlers registered");
    }

    private void ReportPortalListener_BeforeTestStarted(object sender, ReportPortal.NUnitExtension.EventArguments.TestItemStartedEventArgs e)
    {
        var isUiTest = IsUiTest(e.Report);
        Logger.Information("BeforeTestStarted: Report={Report}, IsUiTest={IsUiTest}, Canceled will be set to {ShouldCancel}",
            e.Report?.Substring(0, Math.Min(100, e.Report?.Length ?? 0)), isUiTest, !isUiTest);

        if (!isUiTest)
        {
            e.Canceled = true;
            Logger.Information("BeforeTestStarted: Canceled=true for non-UI test");
        }
    }

    private void ReportPortalListener_BeforeTestFinished(object sender, ReportPortal.NUnitExtension.EventArguments.TestItemFinishedEventArgs e)
    {
        var isUiTest = IsUiTest(e.Report);
        Logger.Information("BeforeTestFinished: Report={Report}, IsUiTest={IsUiTest}, Canceled will be set to {ShouldCancel}",
            e.Report?.Substring(0, Math.Min(100, e.Report?.Length ?? 0)), isUiTest, !isUiTest);

        if (!isUiTest)
        {
            e.Canceled = true;
            Logger.Information("BeforeTestFinished: Canceled=true for non-UI test");
        }
    }

    private static bool IsUiTest(string? report)
    {
        if (string.IsNullOrWhiteSpace(report))
        {
            Logger.Debug("IsUiTest: Report is null or empty");
            return false;
        }

        try
        {
            var element = XElement.Parse(report);
            var fullName = element.Attribute("fullname")?.Value;
            var result = fullName != null && fullName.StartsWith(UiNamespacePrefix, StringComparison.Ordinal);
            Logger.Debug("IsUiTest: FullName={FullName}, Result={Result}", fullName, result);
            return result;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "IsUiTest: Error parsing XML report");
            return false;
        }
    }

    public void OnTestEvent(string report)
    {
        // NUnit engine callback - not used for filtering
    }
}
