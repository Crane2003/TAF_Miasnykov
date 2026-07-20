namespace Core.Api;

public class ApiEndpoints
{
    private readonly string _projectName;
    private string Base => $"/{_projectName}/dashboard";

    public ApiEndpoints(string projectName)
    {
        _projectName = projectName;
    }

    // GET/POST /dashboard
    public string Dashboards => Base;

    // GET/PUT/DELETE/PATCH /dashboard/{id}
    public string Dashboard(int dashboardId) => $"{Base}/{dashboardId}";

    // PUT /dashboard/{id}/add
    public string AddWidget(int dashboardId) => $"{Base}/{dashboardId}/add";

    // GET/DELETE /dashboard/{id}/{widgetId}
    public string Widget(int dashboardId, int widgetId) => $"{Base}/{dashboardId}/{widgetId}";

    // GET /dashboard/{id}/config
    public string DashboardConfig(int dashboardId) => $"{Base}/{dashboardId}/config";
}
