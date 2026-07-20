using Business.Services;

namespace Tests.Helpers;

public static class DashboardApiTestHelpers
{
    public static async Task<bool> VerifyDashboardExistsAsync(DashboardApiService apiService, int dashboardId)
    {
        var dashboard = await apiService.GetDashboardAsync(dashboardId);
        return dashboard != null;
    }

    public static async Task<bool> VerifyWidgetExistsInDashboardAsync(DashboardApiService apiService, int dashboardId, int widgetId)
    {
        var dashboard = await apiService.GetDashboardAsync(dashboardId);
        return dashboard?.Widgets.Any(w => w.Id == widgetId) ?? false;
    }
}
