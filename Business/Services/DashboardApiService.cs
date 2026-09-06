using System.Net;
using System.Text.Json;
using Business.Models;
using Core.Api;
using RestSharp;

namespace Business.Services;

public class DashboardApiService
{
    private readonly ApiClient _apiClient;
    private readonly ApiEndpoints _endpoints;
    private readonly ILogger _logger;
    private int? _cachedFilterId;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    // Parallel fixtures can hit a transient backend locking error; the same payload succeeds on retry.
    private const int MaxWriteAttempts = 2;
    private const int ConcurrencyRetryDelayMs = 250;

    public DashboardApiService(string projectName)
    {
        _apiClient = new ApiClient();
        _endpoints = new ApiEndpoints(projectName);
        _logger = Log.ForContext<DashboardApiService>();
        _logger.Information("DashboardApiService initialized for project: {ProjectName}", projectName);
    }

    public async Task<Dashboard?> CreateDashboardAsync(DashboardCreateRequest request)
    {
        try
        {
            _logger.Information("Creating dashboard: {DashboardName}", request.Name);

            for (var attempt = 1; ; attempt++)
            {
                var response = await _apiClient.ExecutePostAsync(_endpoints.Dashboards, request);

                if (response.IsSuccessful && response.Content != null)
                {
                    var created = Deserialize<Dashboard>(response.Content);
                    _logger.Information("Dashboard created successfully: {DashboardId}", created?.Id);

                    if (created?.Id > 0) return await GetDashboardAsync(created.Id);
                }

                if (IsTransientConcurrencyFailure(response) && attempt < MaxWriteAttempts)
                {
                    _logger.Warning("Dashboard creation hit a transient backend locking error on attempt {Attempt}; retrying.", attempt);
                    await Task.Delay(ConcurrencyRetryDelayMs);

                    continue;
                }

                _logger.Warning("Failed to create dashboard. Status: {StatusCode}. Body: {Body}", (int)response.StatusCode, response.Content);
                return null;
            }
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
            var response = await _apiClient.ExecuteGetAsync(_endpoints.Dashboard(dashboardId));

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
            var response = await _apiClient.ExecutePutAsync(_endpoints.Dashboard(dashboardId), request);

            if (response.IsSuccessful)
                return await GetDashboardAsync(dashboardId);

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
            var response = await _apiClient.ExecutePatchAsync(_endpoints.Dashboard(dashboardId), request);

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

    /// <summary>
    /// Attaching a widget takes two calls: the widget entity is created first via POST /widget,
    /// then linked by id via PUT /dashboard/{id}/add, which only accepts an existing widget and
    /// rejects a null widgetId.
    /// </summary>
    public async Task<Dashboard?> AddWidgetToDashboardAsync(int dashboardId, Widget widget)
    {
        try
        {
            // Reuse the widget when it already exists in the project; otherwise create it to obtain an id.
            var widgetId = widget.Id ?? await CreateWidgetAsync(widget);
            if (widgetId == null)
            {
                _logger.Warning("Could not create widget {WidgetName}; aborting add to dashboard {DashboardId}", widget.Name, dashboardId);
                return null;
            }

            widget.Id = widgetId;

            // Build the payload from a copy: widget instances often come from shared test data,
            // so the request must not hold a reference to a case object other tests reuse.
            var request = new AddWidgetRequest
            {
                AddWidget = new Widget
                {
                    Id = widgetId,
                    Name = widget.Name,
                    Type = widget.Type,
                    Size = widget.Size,
                    Position = widget.Position,
                    Options = new Dictionary<string, JsonElement>(widget.Options)
                }
            };

            for (var attempt = 1; ; attempt++)
            {
                var response = await _apiClient.ExecutePutAsync(_endpoints.AddWidget(dashboardId), request);

                if (response.IsSuccessful)
                    return await GetDashboardAsync(dashboardId);

                if (IsTransientConcurrencyFailure(response) && attempt < MaxWriteAttempts)
                {
                    _logger.Warning("Add widget to dashboard {DashboardId} hit a transient backend locking error on attempt {Attempt}; retrying.", dashboardId, attempt);
                    await Task.Delay(ConcurrencyRetryDelayMs);

                    continue;
                }

                _logger.Warning("Failed to add widget to dashboard {DashboardId}. Status: {StatusCode}. Body: {Body}", dashboardId, (int)response.StatusCode, response.Content);
                return null;
            }
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
            var response = await _apiClient.ExecuteDeleteAsync(_endpoints.Widget(dashboardId, widgetId));

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

    public async Task<int?> CreateWidgetAsync(Widget widget)
    {
        try
        {
            var filterId = await GetFilterIdAsync();
            if (filterId == null)
                return null;

            var request = new WidgetCreateRequest
            {
                Name = widget.Name,
                Description = "Created by automated tests",
                WidgetType = widget.Type,
                Share = false,
                FilterIds = [filterId.Value],
                ContentParameters = WidgetContentParameters.ForWidgetType(widget.Type, widget.Options)
            };

            _logger.Information("Creating widget: {WidgetName} ({WidgetType})", widget.Name, widget.Type);
            var response = await _apiClient.ExecutePostAsync(_endpoints.Widgets, request);

            if (!response.IsSuccessful)
            {
                _logger.Error("Failed to create widget {WidgetName}. Status: {Status}. Body: {Body}",
                    widget.Name, (int)response.StatusCode, response.Content);
                return null;
            }

            var created = Deserialize<EntityCreatedResponse>(response.Content!);
            if (created == null)
                return null;

            _logger.Information("Widget created successfully: {WidgetId}", created.Id);
            return created.Id;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Exception occurred while creating widget {WidgetName}", widget.Name);
            return null;
        }
    }

    public async Task<bool> DeleteDashboardAsync(int dashboardId)
    {
        try
        {
            var response = await _apiClient.ExecuteDeleteAsync(_endpoints.Dashboard(dashboardId));

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

    /// <summary>
    /// Widgets must reference at least one filter. The id is cached because it never changes during a run.
    /// </summary>
    private async Task<int?> GetFilterIdAsync()
    {
        if (_cachedFilterId.HasValue)
            return _cachedFilterId;

        var response = await _apiClient.ExecuteGetAsync(_endpoints.Filters);
        if (!response.IsSuccessful || string.IsNullOrWhiteSpace(response.Content))
        {
            _logger.Error("Failed to load filters. Status: {Status}", (int)response.StatusCode);
            return null;
        }

        using var document = JsonDocument.Parse(response.Content);
        var filterId = document.RootElement.GetProperty("content")[0].GetProperty("id").GetInt32();

        _cachedFilterId = filterId;
        _logger.Debug("Using filter {FilterId} for widget creation", filterId);
        return filterId;
    }

    /// <summary>
    /// Detects the backend's HTTP 500 optimistic-locking clash, which succeeds when the same payload is retried.
    /// </summary>
    private static bool IsTransientConcurrencyFailure(ApiResponse response)
        => response.StatusCode == HttpStatusCode.InternalServerError
           && response.Content?.Contains("updated or deleted by another transaction", StringComparison.OrdinalIgnoreCase) is true;
}
