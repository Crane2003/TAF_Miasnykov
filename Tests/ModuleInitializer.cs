using ReportPortal.NUnitExtension;
using Serilog;

namespace Tests;

internal static class ModuleInitializer
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    internal static void Initialize()
    {
        Log.Information("============ ModuleInitializer: RUNNING - attempting to hook ReportPortalListener ============");

        try
        {
            ReportPortalListener.BeforeTestStarted += (sender, e) =>
            {
                Log.Information("============ ModuleInitializer BeforeTestStarted fired ============");
                if (!IsUiTest(e.Report))
                {
                    e.Canceled = true;
                    Log.Warning("============ ModuleInitializer: Canceled non-UI test ============");
                }
            };

            ReportPortalListener.BeforeTestFinished += (sender, e) =>
            {
                Log.Information("============ ModuleInitializer BeforeTestFinished fired ============");
                if (!IsUiTest(e.Report))
                {
                    e.Canceled = true;
                    Log.Warning("============ ModuleInitializer: Canceled non-UI test ============");
                }
            };

            Log.Information("============ ModuleInitializer: Successfully hooked ReportPortalListener events ============");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "============ ModuleInitializer: FAILED to hook events ============");
        }
    }

    private static bool IsUiTest(string? report)
    {
        if (string.IsNullOrWhiteSpace(report))
            return false;

        try
        {
            var element = System.Xml.Linq.XElement.Parse(report);
            var fullName = element.Attribute("fullname")?.Value;
            return fullName != null && fullName.StartsWith("Tests.E2E.", StringComparison.Ordinal);
        }
        catch
        {
            return false;
        }
    }
}
