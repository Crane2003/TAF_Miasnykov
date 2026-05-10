namespace TAF.Core.Api;

public class ApiEndpoints
{
    public const string Dashboards = "/api/dashboard";

    public static string GetDashboardById(string dashboardId) => $"{Dashboards}/{dashboardId}";

    public static string UpdateDashboard(string dashboardId) => $"{Dashboards}/{dashboardId}";

    public static string DeleteDashboard(string dashboardId) => $"{Dashboards}/{dashboardId}";

    public static string AddWidget(string dashboardId) => $"{Dashboards}/{dashboardId}/widget";

    public static string RemoveWidget(string dashboardId, string widgetId) => $"{Dashboards}/{dashboardId}/widget/{widgetId}";
}
