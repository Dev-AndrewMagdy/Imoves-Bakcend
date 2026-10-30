namespace Movies.Api.Tests.Infrastructure;

internal sealed class StubHttpMessageHandler(
    Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> responseFactory) : HttpMessageHandler
{
    public Uri? LastRequestUri { get; private set; }

    public string? LastAuthorization { get; private set; }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        LastRequestUri = request.RequestUri;
        LastAuthorization = request.Headers.Authorization?.ToString();

        return responseFactory(request, cancellationToken);
    }
}
