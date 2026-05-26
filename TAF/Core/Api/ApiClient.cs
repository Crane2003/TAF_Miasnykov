using RestSharp;
using Serilog;

namespace TAF.Core.Api;

public class ApiClient
{
    private readonly RestClient _client;
    private readonly string _baseUrl;
    private readonly ILogger _logger;

    public ApiClient(string baseUrl)
    {
        _baseUrl = baseUrl;
        _logger = Log.ForContext<ApiClient>();

        var options = new RestClientOptions(_baseUrl)
        {
            Timeout = TimeSpan.FromSeconds(30)
        };
        _client = new RestClient(options);

        _logger.Information("ApiClient initialized with base URL: {BaseUrl}", _baseUrl);
    }

    public async Task<RestResponse<T>> ExecuteGetAsync<T>(string endpoint, Dictionary<string, string>? headers = null)
    {
        _logger.Debug("Executing GET request to: {Endpoint}", endpoint);
        var request = new RestRequest(endpoint, Method.Get);
        AddHeaders(request, headers);
        var response = await _client.ExecuteAsync<T>(request);
        LogResponse(endpoint, "GET", response.StatusCode, response.IsSuccessful);
        return response;
    }

    public async Task<RestResponse<T>> ExecutePostAsync<T>(string endpoint, object? body = null, Dictionary<string, string>? headers = null)
    {
        _logger.Debug("Executing POST request to: {Endpoint}", endpoint);
        var request = new RestRequest(endpoint, Method.Post);
        AddHeaders(request, headers);
        if (body != null)
        {
            request.AddJsonBody(body);
            _logger.Debug("Request body: {@Body}", body);
        }
        var response = await _client.ExecuteAsync<T>(request);
        LogResponse(endpoint, "POST", response.StatusCode, response.IsSuccessful);
        return response;
    }

    public async Task<RestResponse<T>> ExecutePutAsync<T>(string endpoint, object? body = null, Dictionary<string, string>? headers = null)
    {
        _logger.Debug("Executing PUT request to: {Endpoint}", endpoint);
        var request = new RestRequest(endpoint, Method.Put);
        AddHeaders(request, headers);
        if (body != null)
        {
            request.AddJsonBody(body);
            _logger.Debug("Request body: {@Body}", body);
        }
        var response = await _client.ExecuteAsync<T>(request);
        LogResponse(endpoint, "PUT", response.StatusCode, response.IsSuccessful);
        return response;
    }

    public async Task<RestResponse> ExecuteDeleteAsync(string endpoint, Dictionary<string, string>? headers = null)
    {
        _logger.Debug("Executing DELETE request to: {Endpoint}", endpoint);
        var request = new RestRequest(endpoint, Method.Delete);
        AddHeaders(request, headers);
        var response = await _client.ExecuteAsync(request);
        LogResponse(endpoint, "DELETE", response.StatusCode, response.IsSuccessful);
        return response;
    }

    public async Task<RestResponse> ExecuteGetAsync(string endpoint, Dictionary<string, string>? headers = null)
    {
        _logger.Debug("Executing GET request to: {Endpoint}", endpoint);
        var request = new RestRequest(endpoint, Method.Get);
        AddHeaders(request, headers);
        var response = await _client.ExecuteAsync(request);
        LogResponse(endpoint, "GET", response.StatusCode, response.IsSuccessful);
        return response;
    }

    public async Task<RestResponse> ExecutePostAsync(string endpoint, object? body = null, Dictionary<string, string>? headers = null)
    {
        _logger.Debug("Executing POST request to: {Endpoint}", endpoint);
        var request = new RestRequest(endpoint, Method.Post);
        AddHeaders(request, headers);
        if (body != null)
        {
            request.AddJsonBody(body);
        }
        var response = await _client.ExecuteAsync(request);
        LogResponse(endpoint, "POST", response.StatusCode, response.IsSuccessful);
        return response;
    }

    public async Task<RestResponse> ExecutePutAsync(string endpoint, object? body = null, Dictionary<string, string>? headers = null)
    {
        _logger.Debug("Executing PUT request to: {Endpoint}", endpoint);
        var request = new RestRequest(endpoint, Method.Put);
        AddHeaders(request, headers);
        if (body != null)
        {
            request.AddJsonBody(body);
        }
        var response = await _client.ExecuteAsync(request);
        LogResponse(endpoint, "PUT", response.StatusCode, response.IsSuccessful);
        return response;
    }

    private static void AddHeaders(RestRequest request, Dictionary<string, string>? headers)
    {
        if (headers == null) return;

        foreach (var header in headers)
        {
            request.AddHeader(header.Key, header.Value);
        }
    }

    private void LogResponse(string endpoint, string method, System.Net.HttpStatusCode statusCode, bool isSuccessful)
    {
        if (isSuccessful)
        {
            _logger.Information("{Method} {Endpoint} - Status: {StatusCode}", method, endpoint, (int)statusCode);
        }
        else
        {
            _logger.Warning("{Method} {Endpoint} - Status: {StatusCode} (Failed)", method, endpoint, (int)statusCode);
        }
    }
}
