using System.Net;

namespace Core.Api;

public class ApiResponse
{
    public HttpStatusCode StatusCode { get; init; }

    public string? Content { get; init; }

    public bool IsSuccessful { get; init; }
}
