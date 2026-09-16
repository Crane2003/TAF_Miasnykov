using System.Xml.Linq;
using NUnit.Engine;
using NUnit.Engine.Extensibility;
using ReportPortal.NUnitExtension;

namespace Tests.ReportPortalCustomization;

/// <summary>
/// Restricts ReportPortal reporting to UI tests only (tests in the "Tests.E2E" namespace),
/// while still allowing the full NUnit test suite to run in a single "dotnet test" invocation.
/// </summary>
[Extension]
public class UiOnlyReportPortalFilter : ITestEventListener
{
    private const string UiNamespacePrefix = "Tests.E2E.";

    public UiOnlyReportPortalFilter()
    {
        ReportPortalListener.BeforeTestStarted += ReportPortalListener_BeforeTestStarted;
        ReportPortalListener.BeforeTestFinished += ReportPortalListener_BeforeTestFinished;
    }

    private void ReportPortalListener_BeforeTestStarted(object sender, ReportPortal.NUnitExtension.EventArguments.TestItemStartedEventArgs e)
    {
        if (!IsUiTest(e.Report))
        {
            e.Canceled = true;
        }
    }

    private void ReportPortalListener_BeforeTestFinished(object sender, ReportPortal.NUnitExtension.EventArguments.TestItemFinishedEventArgs e)
    {
        if (!IsUiTest(e.Report))
        {
            e.Canceled = true;
        }
    }

    private static bool IsUiTest(string? report)
    {
        if (string.IsNullOrWhiteSpace(report))
        {
            return false;
        }

        try
        {
            var element = XElement.Parse(report);
            var fullName = element.Attribute("fullname")?.Value;
            return fullName != null && fullName.StartsWith(UiNamespacePrefix, StringComparison.Ordinal);
        }
        catch
        {
            return false;
        }
    }

    public void OnTestEvent(string report)
    {
    }
}
