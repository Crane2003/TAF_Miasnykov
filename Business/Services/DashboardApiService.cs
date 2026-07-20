using System.Text.Json;
using Business.Models;
using Core.Api;

namespace Business.Services;

public class DashboardApiService
{
    private readonly ApiClient _apiClient;
    private readonly ApiEndpoints _endpoints;
    private readonly ILogger _logger;
    private Dictionary<string, string>? _authHeaders;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public DashboardApiService(string baseUrl, string projectName)
    {
        _apiClient = new ApiClient(baseUrl);
        _endpoints = new ApiEndpoints(projectName);
        _logger = Log.ForContext<DashboardApiService>();
        _logger.Information("DashboardApiService initialized for project: {ProjectName}", projectName);
    }

    public void SetAuthToken(string token)
    {
        _authHeaders = new Dictionary<string, string>
        {
            { "Authorization", $"Bearer {token}" }
        };
        _logger.Debug("Authorization token set");
    }

    public async Task<Dashboard?> CreateDashboardAsync(DashboardCreateRequest request)
    {
        try
        {
            _logger.Information("Creating dashboard: {DashboardName}", request.Name);
            var response = await _apiClient.ExecutePostAsync(_endpoints.Dashboards, request, _authHeaders);

            if (response.IsSuccessful && response.Content != null)
            {
                var created = Deserialize<Dashboard>(response.Content);
                _logger.Information("Dashboard created successfully: {DashboardId}", created?.Id);
                return created;
            }

            _logger.Warning("Failed to create dashboard. Status: {StatusCode}", (int)response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Exception occurred while creating dashboard");
            return null;
        }
    }

    public async Task<Dashboard?> GetDashboardAsync(int dashboardId)
    {
        try
        {
            _logger.Debug("Fetching dashboard: {DashboardId}", dashboardId);
            var response = await _apiClient.ExecuteGetAsync(_endpoints.Dashboard(dashboardId), _authHeaders);

            if (response.IsSuccessful && response.Content != null)
                return Deserialize<Dashboard>(response.Content);

            _logger.Warning("Failed to get dashboard {DashboardId}. Status: {StatusCode}", dashboardId, (int)response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Exception occurred while fetching dashboard {DashboardId}", dashboardId);
            return null;
        }
    }

    public async Task<Dashboard?> UpdateDashboardAsync(int dashboardId, DashboardUpdateRequest request)
    {
        try
        {
            var response = await _apiClient.ExecutePutAsync(_endpoints.Dashboard(dashboardId), request, _authHeaders);

            if (response.IsSuccessful && response.Content != null)
                return Deserialize<Dashboard>(response.Content);

            _logger.Warning("Failed to update dashboard {DashboardId}. Status: {StatusCode}", dashboardId, (int)response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Exception occurred while updating dashboard {DashboardId}", dashboardId);
            return null;
        }
    }

    public async Task<bool> LockDashboardAsync(int dashboardId, bool locked)
    {
        try
        {
            var request = new DashboardLockRequest { Locked = locked };
            var response = await _apiClient.ExecutePatchAsync(_endpoints.Dashboard(dashboardId), request, _authHeaders);

            if (response.IsSuccessful)
                return true;

            _logger.Warning("Failed to lock dashboard {DashboardId}. Status: {StatusCode}", dashboardId, (int)response.StatusCode);
            return false;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Exception occurred while locking dashboard {DashboardId}", dashboardId);
            return false;
        }
    }

    public async Task<Dashboard?> AddWidgetToDashboardAsync(int dashboardId, Widget widget)
    {
        try
        {
            var request = new AddWidgetRequest { AddWidget = widget };
            var response = await _apiClient.ExecutePutAsync(_endpoints.AddWidget(dashboardId), request, _authHeaders);

            if (response.IsSuccessful && response.Content != null)
                return Deserialize<Dashboard>(response.Content);

            _logger.Warning("Failed to add widget to dashboard {DashboardId}. Status: {StatusCode}", dashboardId, (int)response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Exception occurred while adding widget to dashboard {DashboardId}", dashboardId);
            return null;
        }
    }

    public async Task<bool> RemoveWidgetFromDashboardAsync(int dashboardId, int widgetId)
    {
        try
        {
            var response = await _apiClient.ExecuteDeleteAsync(_endpoints.Widget(dashboardId, widgetId), _authHeaders);

            if (response.IsSuccessful)
                return true;

            _logger.Warning("Failed to remove widget {WidgetId} from dashboard {DashboardId}. Status: {StatusCode}", widgetId, dashboardId, (int)response.StatusCode);
            return false;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Exception occurred while removing widget {WidgetId} from dashboard {DashboardId}", widgetId, dashboardId);
            return false;
        }
    }

    public async Task<bool> DeleteDashboardAsync(int dashboardId)
    {
        try
        {
            var response = await _apiClient.ExecuteDeleteAsync(_endpoints.Dashboard(dashboardId), _authHeaders);

            if (response.IsSuccessful)
                return true;

            _logger.Warning("Failed to delete dashboard {DashboardId}. Status: {StatusCode}", dashboardId, (int)response.StatusCode);
            return false;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Exception occurred while deleting dashboard {DashboardId}", dashboardId);
            return false;
        }
    }

    public async Task<List<Dashboard>?> GetAllDashboardsAsync()
    {
        try
        {
            var response = await _apiClient.ExecuteGetAsync(_endpoints.Dashboards, _authHeaders);

            if (response.IsSuccessful && response.Content != null)
                return Deserialize<List<Dashboard>>(response.Content);

            _logger.Warning("Failed to get all dashboards. Status: {StatusCode}", (int)response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Exception occurred while fetching all dashboards");
            return null;
        }
    }

    private T? Deserialize<T>(string content)
    {
        try
        {
            return JsonSerializer.Deserialize<T>(content, JsonOptions);
        }
        catch (JsonException ex)
        {
            _logger.Error(ex, "Failed to deserialize response to {Type}", typeof(T).Name);
            return default;
        }
    }
}
