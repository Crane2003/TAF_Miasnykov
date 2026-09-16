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
    private static bool _extensionLoaded = false;

    static UiOnlyReportPortalFilter()
    {
        _extensionLoaded = true;
        Log.Information("============ UiOnlyReportPortalFilter STATIC CTOR CALLED ============");
    }

    public UiOnlyReportPortalFilter()
    {
        Log.Information("============ UiOnlyReportPortalFilter INSTANCE CREATED ============");
        Log.Information("UiOnlyReportPortalFilter: Initializing extension");

        try
        {
            ReportPortalListener.BeforeTestStarted += ReportPortalListener_BeforeTestStarted;
            ReportPortalListener.BeforeTestFinished += ReportPortalListener_BeforeTestFinished;
            Log.Information("============ UiOnlyReportPortalFilter: Event handlers REGISTERED ============");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "============ UiOnlyReportPortalFilter: FAILED to register event handlers ============");
        }
    }

    private void ReportPortalListener_BeforeTestStarted(object sender, ReportPortal.NUnitExtension.EventArguments.TestItemStartedEventArgs e)
    {
        var isUiTest = IsUiTest(e.Report);
        Log.Information("============ BeforeTestStarted: IsUiTest={IsUiTest}, Report substring: {Report} ============",
            isUiTest, e.Report?.Substring(0, Math.Min(200, e.Report?.Length ?? 0)));

        if (!isUiTest)
        {
            e.Canceled = true;
            Log.Warning("============ BeforeTestStarted: CANCELED non-UI test ============");
        }
    }

    private void ReportPortalListener_BeforeTestFinished(object sender, ReportPortal.NUnitExtension.EventArguments.TestItemFinishedEventArgs e)
    {
        var isUiTest = IsUiTest(e.Report);
        Log.Information("============ BeforeTestFinished: IsUiTest={IsUiTest}, Report substring: {Report} ============",
            isUiTest, e.Report?.Substring(0, Math.Min(200, e.Report?.Length ?? 0)));

        if (!isUiTest)
        {
            e.Canceled = true;
            Log.Warning("============ BeforeTestFinished: CANCELED non-UI test ============");
        }
    }

    private static bool IsUiTest(string? report)
    {
        if (string.IsNullOrWhiteSpace(report))
        {
            Log.Debug("IsUiTest: Report is null or empty");
            return false;
        }

        try
        {
            var element = XElement.Parse(report);
            var fullName = element.Attribute("fullname")?.Value;
            var result = fullName != null && fullName.StartsWith(UiNamespacePrefix, StringComparison.Ordinal);
            Log.Information("IsUiTest: FullName={FullName}, Result={Result}, ElementName={ElementName}", 
                fullName, result, element.Name);
            return result;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "IsUiTest: Error parsing XML report. Content: {Report}", report);
            return false;
        }
    }

    public void OnTestEvent(string report)
    {
        Log.Debug("OnTestEvent called with report: {Report}", report?.Substring(0, Math.Min(100, report?.Length ?? 0)));
    }
}
