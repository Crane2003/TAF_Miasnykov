using Core.Configuration;
using RestSharp;
using RestSharp.Authenticators;

namespace Core.Api;

public class ApiClient
{
    private readonly RestClient _client;
    private readonly ILogger _logger;

    public ApiClient()
    {
        _logger = Log.ForContext<ApiClient>();

        var apiConfig = ConfigurationLoader.Instance.GetApiConfiguration();
        var baseUrl = apiConfig.BaseUrl;

        var options = new RestClientOptions(baseUrl)
        {
            Timeout = TimeSpan.FromSeconds(30)
        };

        if (!string.IsNullOrEmpty(apiConfig.AuthToken))
        {
            options.Authenticator = new JwtAuthenticator(apiConfig.AuthToken);
        }

        if (IsLoopback(baseUrl))
        {
            options.RemoteCertificateValidationCallback = (_, _, _, _) => true;
        }

        _client = new RestClient(options);

        _logger.Information("ApiClient initialized with base URL: {BaseUrl}", baseUrl);
    }

    private static bool IsLoopback(string baseUrl)
        => Uri.TryCreate(baseUrl, UriKind.Absolute, out var uri) && uri.IsLoopback;

    public async Task<RestResponse> ExecuteGetAsync(string endpoint, IDictionary<string, string>? headers = null)
        => await ExecuteAsync(endpoint, Method.Get, null, headers);

    public async Task<RestResponse> ExecutePostAsync(string endpoint, object? body = null, IDictionary<string, string>? headers = null)
        => await ExecuteAsync(endpoint, Method.Post, body, headers);

    public async Task<RestResponse> ExecutePutAsync(string endpoint, object? body = null, IDictionary<string, string>? headers = null)
        => await ExecuteAsync(endpoint, Method.Put, body, headers);

    public async Task<RestResponse> ExecutePatchAsync(string endpoint, object? body = null, IDictionary<string, string>? headers = null)
        => await ExecuteAsync(endpoint, Method.Patch, body, headers);

    public async Task<RestResponse> ExecuteDeleteAsync(string endpoint, IDictionary<string, string>? headers = null)
        => await ExecuteAsync(endpoint, Method.Delete, null, headers);

    private async Task<RestResponse> ExecuteAsync(string endpoint, Method method, object? body, IDictionary<string, string>? headers = null)
    {
        _logger.Debug("Executing {Method} request to: {Endpoint}", method, endpoint);
        var request = new RestRequest(endpoint, method);

        if (headers != null)
            foreach (var header in headers)
                request.AddHeader(header.Key, header.Value);

        if (body != null)
            request.AddJsonBody(body);

        var response = await _client.ExecuteAsync(request);
        _logger.Information("{Method} {Endpoint} - Status: {StatusCode}", method, endpoint, response.StatusCode);
        return response;
    }
}

