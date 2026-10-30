using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using Movies.Application.Models;
using Movies.Infrastructure.Configuration;

namespace Movies.Infrastructure.Http;

/// <summary>
/// Low-level typed client for TMDB. It owns authentication, transport, and
/// response mapping only; endpoint composition belongs to the service layer.
/// </summary>
public sealed class TmdbApiClient(HttpClient httpClient, IOptions<TmdbOptions> options) : ITmdbApiClient
{
    private static readonly HashSet<string> ForwardedResponseHeaders = new(StringComparer.OrdinalIgnoreCase)
    {
        "Cache-Control",
        "Content-Language",
        "ETag",
        "Expires",
        "Last-Modified",
        "Retry-After",
        "Vary"
    };

    private readonly string _readAccessToken = GetReadAccessToken(options.Value);

    public async Task<ProviderResponse> GetAsync(
        string relativeUri,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(relativeUri);

        using var request = new HttpRequestMessage(HttpMethod.Get, relativeUri);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _readAccessToken);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var response = await httpClient.SendAsync(
            request,
            HttpCompletionOption.ResponseContentRead,
            cancellationToken);

        // Do not call EnsureSuccessStatusCode: TMDB status codes and error bodies
        // must be returned to our caller unchanged.
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        var headers = response.Headers
            .Concat(response.Content.Headers)
            .Where(header => ForwardedResponseHeaders.Contains(header.Key))
            .ToDictionary(
                header => header.Key,
                header => header.Value.ToArray(),
                StringComparer.OrdinalIgnoreCase);

        return new ProviderResponse(
            (int)response.StatusCode,
            response.Content.Headers.ContentType?.ToString(),
            body,
            headers);
    }

    private static string GetReadAccessToken(TmdbOptions options)
    {
        return string.IsNullOrWhiteSpace(options.ReadAccessToken)
            ? throw new InvalidOperationException(
                "TMDB read access token is not configured. Set Tmdb:ReadAccessToken in user secrets or the Tmdb__ReadAccessToken environment variable.")
            : options.ReadAccessToken.Trim();
    }
}
