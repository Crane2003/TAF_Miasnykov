using RestSharp;

namespace TAF.Core.Api;

public class ApiClient
{
    private readonly RestClient _client;
    private readonly string _baseUrl;

    public ApiClient(string baseUrl)
    {
        _baseUrl = baseUrl;
        var options = new RestClientOptions(_baseUrl)
        {
            Timeout = TimeSpan.FromSeconds(30)
        };
        _client = new RestClient(options);
    }

    public async Task<RestResponse<T>> ExecuteGetAsync<T>(string endpoint, Dictionary<string, string>? headers = null)
    {
        var request = new RestRequest(endpoint, Method.Get);
        AddHeaders(request, headers);
        return await _client.ExecuteAsync<T>(request);
    }

    public async Task<RestResponse<T>> ExecutePostAsync<T>(string endpoint, object? body = null, Dictionary<string, string>? headers = null)
    {
        var request = new RestRequest(endpoint, Method.Post);
        AddHeaders(request, headers);
        if (body != null)
        {
            request.AddJsonBody(body);
        }
        return await _client.ExecuteAsync<T>(request);
    }

    public async Task<RestResponse<T>> ExecutePutAsync<T>(string endpoint, object? body = null, Dictionary<string, string>? headers = null)
    {
        var request = new RestRequest(endpoint, Method.Put);
        AddHeaders(request, headers);
        if (body != null)
        {
            request.AddJsonBody(body);
        }
        return await _client.ExecuteAsync<T>(request);
    }

    public async Task<RestResponse> ExecuteDeleteAsync(string endpoint, Dictionary<string, string>? headers = null)
    {
        var request = new RestRequest(endpoint, Method.Delete);
        AddHeaders(request, headers);
        return await _client.ExecuteAsync(request);
    }

    public async Task<RestResponse> ExecuteGetAsync(string endpoint, Dictionary<string, string>? headers = null)
    {
        var request = new RestRequest(endpoint, Method.Get);
        AddHeaders(request, headers);
        return await _client.ExecuteAsync(request);
    }

    public async Task<RestResponse> ExecutePostAsync(string endpoint, object? body = null, Dictionary<string, string>? headers = null)
    {
        var request = new RestRequest(endpoint, Method.Post);
        AddHeaders(request, headers);
        if (body != null)
        {
            request.AddJsonBody(body);
        }
        return await _client.ExecuteAsync(request);
    }

    public async Task<RestResponse> ExecutePutAsync(string endpoint, object? body = null, Dictionary<string, string>? headers = null)
    {
        var request = new RestRequest(endpoint, Method.Put);
        AddHeaders(request, headers);
        if (body != null)
        {
            request.AddJsonBody(body);
        }
        return await _client.ExecuteAsync(request);
    }

    private static void AddHeaders(RestRequest request, Dictionary<string, string>? headers)
    {
        if (headers == null) return;

        foreach (var header in headers)
        {
            request.AddHeader(header.Key, header.Value);
        }
    }
}
