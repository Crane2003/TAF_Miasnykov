using TAF.Business.Services;

namespace TAF.Tests.Helpers;

public static class DashboardApiTestHelpers
{
    public static async Task<bool> VerifyDashboardExistsAsync(DashboardApiService apiService, string dashboardId)
    {
        var (Success, Dashboard, _) = await apiService.GetDashboardAsync(dashboardId);
        return Success && Dashboard != null;
    }

    public static async Task<bool> VerifyWidgetExistsInDashboardAsync(DashboardApiService apiService, string dashboardId, string widgetId)
    {
        var (Success, Dashboard, _) = await apiService.GetDashboardAsync(dashboardId);
        if (Success && Dashboard != null)
        {
            return Dashboard.Widgets.Any(w => w.Id == widgetId);
        }
        return false;
    }
}
