using System.Net;
using System.Net.Http.Json;

namespace TaskApp.Tests;

internal sealed class MockHttpHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;

    public MockHttpHandler(Func<HttpRequestMessage, HttpResponseMessage> responder)
    {
        _responder = responder;
    }

    public List<HttpRequestMessage> Requests { get; } = [];

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        Requests.Add(request);
        return Task.FromResult(_responder(request));
    }

    public static HttpResponseMessage JsonResponse<T>(HttpStatusCode statusCode, T body)
    {
        return new HttpResponseMessage(statusCode)
        {
            Content = JsonContent.Create(body)
        };
    }

    public static HttpResponseMessage Empty(HttpStatusCode statusCode)
    {
        return new HttpResponseMessage(statusCode);
    }
}