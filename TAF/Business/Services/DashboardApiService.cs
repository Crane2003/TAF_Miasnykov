using Newtonsoft.Json;
using TAF.Business.Models;
using TAF.Core.Api;

namespace TAF.Business.Services;

public class DashboardApiService
{
    private readonly ApiClient _apiClient;
    private Dictionary<string, string>? _authHeaders;

    public DashboardApiService(string baseUrl)
    {
        _apiClient = new ApiClient(baseUrl);
    }

    public void SetAuthToken(string token)
    {
        _authHeaders = new Dictionary<string, string>
        {
            { "Authorization", $"Bearer {token}" }
        };
    }

    public async Task<(bool Success, Dashboard? Dashboard, string Message)> CreateDashboardAsync(Dashboard dashboard)
    {
        try
        {
            var response = await _apiClient.ExecutePostAsync(ApiEndpoints.Dashboards, dashboard, _authHeaders);

            if (response.IsSuccessful && response.Content != null)
            {
                var createdDashboard = JsonConvert.DeserializeObject<Dashboard>(response.Content);
                return (true, createdDashboard, "Dashboard created successfully");
            }

            return (false, null, response.ErrorMessage ?? "Failed to create dashboard");
        }
        catch (Exception ex)
        {
            return (false, null, ex.Message);
        }
    }

    public async Task<(bool Success, Dashboard? Dashboard, string Message)> GetDashboardAsync(string dashboardId)
    {
        try
        {
            var response = await _apiClient.ExecuteGetAsync(ApiEndpoints.GetDashboardById(dashboardId), _authHeaders);

            if (response.IsSuccessful && response.Content != null)
            {
                var dashboard = JsonConvert.DeserializeObject<Dashboard>(response.Content);
                return (true, dashboard, "Dashboard retrieved successfully");
            }

            return (false, null, response.ErrorMessage ?? "Failed to get dashboard");
        }
        catch (Exception ex)
        {
            return (false, null, ex.Message);
        }
    }

    public async Task<(bool Success, Dashboard? Dashboard, string Message)> UpdateDashboardAsync(string dashboardId, Dashboard dashboard)
    {
        try
        {
            var response = await _apiClient.ExecutePutAsync(ApiEndpoints.UpdateDashboard(dashboardId), dashboard, _authHeaders);

            if (response.IsSuccessful && response.Content != null)
            {
                var updatedDashboard = JsonConvert.DeserializeObject<Dashboard>(response.Content);
                return (true, updatedDashboard, "Dashboard updated successfully");
            }

            return (false, null, response.ErrorMessage ?? "Failed to update dashboard");
        }
        catch (Exception ex)
        {
            return (false, null, ex.Message);
        }
    }

    public async Task<(bool Success, Dashboard? Dashboard, string Message)> AddWidgetToDashboardAsync(string dashboardId, Widget widget)
    {
        try
        {
            var response = await _apiClient.ExecutePutAsync(ApiEndpoints.AddWidget(dashboardId), widget, _authHeaders);

            if (response.IsSuccessful && response.Content != null)
            {
                var updatedDashboard = JsonConvert.DeserializeObject<Dashboard>(response.Content);
                return (true, updatedDashboard, "Widget added successfully");
            }

            return (false, null, response.ErrorMessage ?? "Failed to add widget");
        }
        catch (Exception ex)
        {
            return (false, null, ex.Message);
        }
    }

    public async Task<(bool Success, string Message)> RemoveWidgetFromDashboardAsync(string dashboardId, string widgetId)
    {
        try
        {
            var response = await _apiClient.ExecuteDeleteAsync(ApiEndpoints.RemoveWidget(dashboardId, widgetId), _authHeaders);

            if (response.IsSuccessful)
            {
                return (true, "Widget removed successfully");
            }

            return (false, response.ErrorMessage ?? "Failed to remove widget");
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public async Task<(bool Success, string Message)> DeleteDashboardAsync(string dashboardId)
    {
        try
        {
            var response = await _apiClient.ExecuteDeleteAsync(ApiEndpoints.DeleteDashboard(dashboardId), _authHeaders);

            if (response.IsSuccessful)
            {
                return (true, "Dashboard deleted successfully");
            }

            return (false, response.ErrorMessage ?? "Failed to delete dashboard");
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public async Task<(bool Success, List<Dashboard>? Dashboards, string Message)> GetAllDashboardsAsync()
    {
        try
        {
            var response = await _apiClient.ExecuteGetAsync(ApiEndpoints.Dashboards, _authHeaders);

            if (response.IsSuccessful && response.Content != null)
            {
                var dashboards = JsonConvert.DeserializeObject<List<Dashboard>>(response.Content);
                return (true, dashboards, "Dashboards retrieved successfully");
            }

            return (false, null, response.ErrorMessage ?? "Failed to get dashboards");
        }
        catch (Exception ex)
        {
            return (false, null, ex.Message);
        }
    }
}
